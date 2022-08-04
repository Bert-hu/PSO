using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO
{
    public class PSO
    {
        /// <summary>
        /// 粒子个数
        /// </summary>
        public int ParticleNum { get; private set; } = 5;
        /// <summary>
        /// 惯性权重
        /// </summary>
        public decimal w { get; private set; } = new Decimal(0.8);
        /// <summary>
        /// 个体权重
        /// </summary>
        public decimal c1 { get; private set; } = 2;
        /// <summary>
        /// 群体权重
        /// </summary>
        public decimal c2 { get; private set; } = 2;
        /// <summary>
        /// 参数模型
        /// </summary>
        public List<Parameter> ParameterModels { get; private set; }
        /// <summary>
        /// 种群粒子分布的标准差
        /// </summary>
        public double Stdev { get; private set; } = double.MaxValue;
        /// <summary>
        /// 群体最优
        /// </summary>
        public Particle BestParticle { get; private set; }
        /// <summary>
        /// 群体最优
        /// </summary>
        public decimal BestGroupFitness { get; private set; } = decimal.MaxValue;
        /// <summary>
        /// 最小界限，达到此值时将停止迭代
        /// </summary>
        public decimal FitnessLimit { get; private set; } = 0;
        /// <summary>
        /// 所有参数的平均方差
        /// </summary>
        public decimal MeanStDev { get; private set; } = decimal.MaxValue;
        /// <summary>
        /// 粒子群集合
        /// </summary>
        public List<Particle> Particles { get; private set; }
        /// <summary>
        /// 最大迭代次数，达到此值时将停止迭代
        /// </summary>
        public int MaxIteration { get; set; } = 50000;
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitiated { get; private set; } = false;
        /// <summary>
        /// 运行过程中的log
        /// </summary>
        public StringBuilder Report { get; private set; }

        public delegate decimal Function(List<Parameter> parameters);
        /// <summary>
        /// 寻优函数
        /// </summary>
        public Function Func { get; private set; } = null;
        public void SetConfiguration(Function _Func, List<Parameter> _ParameterModels, int _ParticleNum = 5, decimal _w = (decimal)0.8, decimal _c1 = 2, decimal _c2 = 2, int _MaxIteration = 50000)
        {
            Func = _Func;
            ParameterModels = _ParameterModels;
            ParticleNum = _ParticleNum;
            w = _w;
            c1 = _c1;
            c2 = _c2;
            MaxIteration = _MaxIteration;
        }
        /// <summary>
        /// 初始化粒子群
        /// </summary>
        public void Initialize()
        {
            try
            {
                if (ParameterModels != null && ParameterModels.Count > 0 && Func != null)
                {
                    Particles = new List<Particle>();
                    for (int i = 0; i < ParticleNum; i++)
                    {
                        var particle = CreatePatical(ParameterModels);
                        particle.Index = i;
                        Particles.Add(particle);
                    }
                    MeanStDev = CalculateGroupMeanStDev(Particles);
                    IsInitiated = true;
                }
            }
            catch { }
        }


        public Task<List<Parameter>> RunAsync()
        {
            var result = new List<Parameter>();
            if (!IsInitiated) return Task.FromResult(result); //未初始化或初始化失败
            Report = new StringBuilder();
            var initdesc = GetParticalsDesc(Particles);
            Report.Append(initdesc);
            int count = 0;
            //while (count < MaxIteration && MeanStDev < new decimal(0.0001) && BestGroupFitness <= FitnessLimit)
            while (count < MaxIteration  && BestGroupFitness > FitnessLimit)
            //while (count < MaxIteration && MeanStDev > new decimal(0.0001))
            {
                foreach (var paticle in Particles)
                {
                    Random rand = new Random(GetRandomSeed());

                    decimal rand1 = new decimal(rand.NextDouble());
                    decimal rand2 = new decimal(rand.NextDouble());
                    //粒子群更新迭代
                    foreach (var para in paticle.Parameters)
                    {
                        var groupBestValue = BestParticle.BestParameters.First(it => it.Name == para.Name).Value;
                        var paticleBestValue = paticle.BestParameters.First(it => it.Name == para.Name).Value;
                        var newVelocity = w * para.Velocity + c1 * rand1 * (paticleBestValue - para.Value) + c2 * rand2 * (groupBestValue - para.Value);

                        if (para.IsInteger)
                        {
                            newVelocity = Math.Round(newVelocity);
                        }

                        if (newVelocity < para.MinVelocity)
                        {
                            newVelocity = para.MinVelocity;
                        }
                        else if (newVelocity > para.MaxVelocity)
                        {
                            newVelocity = para.MaxVelocity;
                        }
                        var newValue = para.Value + newVelocity;
                        if (newValue < para.MinValue)
                        {
                            newValue = para.MinValue;
                        }
                        else if (newValue > para.MaxValue)
                        {
                            newValue = para.MaxValue;
                        }
                        para.Velocity = newVelocity;
                        para.Value = newValue;
                    }
                    //计算迭代后的值
                    var resultFitness = Math.Abs(Func(paticle.Parameters));
                    //计算迭代后平均标准差
                    MeanStDev = CalculateGroupMeanStDev(Particles);

                    //结果优于当前粒子最好值
                    if (resultFitness < paticle.BestFitness)
                    {
                        paticle.SetBestParameters(paticle.Parameters);
                        paticle.BestFitness = resultFitness;
                    }
                    //结果优于种群最好值
                    if (resultFitness < BestGroupFitness)
                    {
                        BestParticle = paticle.Clone();
                        BestGroupFitness = resultFitness;
                    }
                    paticle.Iteration++;
                    var iterationReport = GetParticalDesc(paticle, resultFitness);
                    Report.Append(iterationReport);
                }
               
                count++;
            }
            return Task.FromResult(BestParticle.BestParameters);
        }

        private Particle CreatePatical(List<Parameter> ParameterModels)
        {
            var particle = new Particle();
            var paramters = new List<Parameter>();
            var bestparamters = new List<Parameter>();


            foreach (var models in ParameterModels)
            {
                Random rand = new Random(GetRandomSeed());

                var para = models.Clone();
                if (para.IsInteger)
                {
                    para.Value = new Decimal(rand.Next((int)Math.Round(para.MinValue), (int)Math.Round(para.MaxValue)));
                    para.Velocity = new Decimal(rand.Next((int)Math.Round(para.MinVelocity, 0), (int)Math.Round(para.MaxVelocity, 0)));
                }
                else
                {
                    para.Value = para.MinValue + (new decimal(rand.NextDouble()) * (para.MaxValue - para.MinValue));
                    para.Velocity = para.MinVelocity + (new decimal(rand.NextDouble()) * (para.MaxVelocity - para.MinVelocity));
                }
                paramters.Add(para);
                bestparamters.Add(para.Clone());
            }
            particle.Parameters = paramters;
            particle.SetBestParameters(bestparamters);
            particle.Iteration = 0;
            var resultFitness = Math.Abs(Func(bestparamters));
            particle.BestFitness = resultFitness;

            if (resultFitness < BestGroupFitness)
            {
                BestParticle = particle.Clone();
                BestGroupFitness = resultFitness;
            }

            return particle;
        }

        private decimal CalculateGroupMeanStDev(List<Particle> particles)
        {
            List<double> stdevs = new List<double>();
            foreach (var para in particles.First().Parameters)
            {
                double[] paraArrData = particles.Select(x => (double)x.Parameters.First(y => y.Name == para.Name).Value).ToArray();
                var stdev = CalculateStDev(paraArrData);
                stdevs.Add(stdev);
            }
            return new Decimal(stdevs.Sum() / stdevs.Count);
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="arrData"></param>
        /// <returns></returns>
        private double CalculateStDev(double[] arrData)
        {
            double xSum = 0;
            double xAvg = 0;
            double sSum = 0;
            double tmpStDev = 0;
            int arrNum = arrData.Length;
            for (int i = 0; i < arrNum; i++)
            {
                xSum += arrData[i];
            }
            xAvg = xSum / arrNum;
            for (int j = 0; j < arrNum; j++)
            {
                sSum += ((arrData[j] - xAvg) * (arrData[j] - xAvg));
            }
            tmpStDev = Convert.ToSingle(Math.Sqrt((sSum / (arrNum - 1))).ToString());
            return tmpStDev;
        }

        private string GetParticalsDesc(List<Particle> particles)
        {
            string desc = string.Empty;
            foreach (var particle in particles)
            {
                desc += GetParticalDesc(particle);
            }
            return desc;
        }
        private string GetParticalDesc(Particle particle,decimal? fitness = null)
        {
            string desc = string.Empty;
            string parasdesc = string.Empty;
            if (fitness == null)
            {
                fitness = Func(particle.Parameters);
            }
            foreach (var para in particle.Parameters)
            {
                parasdesc += $"[{para.Name}:{para.Value}],";
            }
            desc = $"Particle Index:[{particle.Index}],Iteration:[{particle.Iteration}],Fitness:[{fitness}],Parameters:[{parasdesc}].\n";

            return desc;
        }

        /// <summary>
        /// 产生随机种子，避免Random生成相同的数
        /// </summary>
        /// <returns></returns>
        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

    }
}

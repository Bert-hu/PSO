using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO
{
    public class Particle
    {
        /// <summary>
        /// 粒子的索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 粒子的参数信息
        /// </summary>
        public List<Parameter> Parameters { get; set; }
        /// <summary>
        /// 迭代次数
        /// </summary>
        public int Iteration { get; set; }

        public List<Parameter> BestParameters { get; private set; }
        public decimal BestFitness { get; set; } = decimal.MaxValue;


        public Particle Clone()
        {
            var dest = new Particle();
            dest.Index = this.Index;
            var paras = new List<Parameter>();
            foreach (var para in this.Parameters)
            {
                paras.Add(para.Clone());
            }
            dest.Parameters = paras;
            dest.Iteration = this.Iteration;
            dest.SetBestParameters(BestParameters);
            dest.BestFitness = BestFitness;
            return dest;
        }

        public void SetBestParameters(List<Parameter> parameters)
        {
            var paras = new List<Parameter>();
            foreach (var para in parameters)
            {
                paras.Add(para.Clone());
            }
            BestParameters = paras;
        }
    }
}

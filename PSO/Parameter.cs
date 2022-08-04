using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSO
{
    public class Parameter
    {


        /// <summary>
        /// 参数名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数是否为整数
        /// </summary>
        public bool IsInteger { get; set; } = false;
        /// <summary>
        /// 参数值
        /// </summary>
        public decimal Value { get; set; } 
        /// <summary>
        /// 参数最小值
        /// </summary>
        public decimal MinValue { get; set; } = 0;
        /// <summary>
        /// 参数最大值
        /// </summary>
        public decimal MaxValue { get; set; } = 100;
        /// <summary>
        /// 粒子速度
        /// </summary>
        public decimal Velocity { get; set; } 
        /// <summary>
        /// 粒子最小速度
        /// </summary>
        public decimal MinVelocity { get; set; } = -2;
        /// <summary>
        /// 粒子最大速度
        /// </summary>
        public decimal MaxVelocity { get; set; } = 2;

        public Parameter Clone()
        {
            var dest = new Parameter();
            dest.Name = this.Name;
            dest.IsInteger = this.IsInteger;
            dest.Value = this.Value;
            dest.MinValue = this.MinValue;
            dest.MaxValue = this.MaxValue;
            dest.Velocity = this.Velocity;
            dest.MinVelocity = this.MinVelocity;
            dest.MaxVelocity = this.MaxVelocity;
            return dest;
        }

        public static Parameter CreateParameter(string name, bool isInteger,  decimal minValue, decimal maxValue, decimal minVelocity, decimal maxVelocity)
        {
            Parameter para = new Parameter();
            para.Name = name;
            para.IsInteger = isInteger;
            para.MinValue = minValue;
            para.MaxValue = maxValue;
            para.MinVelocity = minVelocity;
            para.MaxVelocity = maxVelocity;
            return para;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSO.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PSO pso = new PSO();


            List<Parameter> parameters = new List<Parameter>();
            var para = Parameter.CreateParameter("AA", true, 0, 10, -5, 5);
            var para1 = Parameter.CreateParameter("BB", true, 0, 10, -5, 5);
            var para2 = Parameter.CreateParameter("CC", true, 0, 10, -5, 5);


            parameters.Add(para);
            parameters.Add(para1);
            parameters.Add(para2);


            pso.SetConfiguration(Funtion, parameters,_MaxIteration : 10000);
            pso.Initialize();
            var result = pso.RunAsync();
            List<Parameter> aa = result.Result;
            string report = pso.Report.ToString();
        }

        private decimal Funtion(List<Parameter> parameters)
        {
            var aa = parameters.First(it => it.Name == "AA").Value;
            var bb = parameters.First(it => it.Name == "BB").Value;
            var cc = parameters.First(it => it.Name == "CC").Value;



            //var result = (aa.Value - 500) * (aa.Value - 500) ;
            double e = Math.E;
            decimal yy =( aa + bb +  cc) - 32/5;
            var result = 1000 * aa + 3000 * bb + 5000 * cc -32000 - (decimal) (320/ Math.Pow(e,(double)yy)) + 320;


            return result;
        }

    }
}

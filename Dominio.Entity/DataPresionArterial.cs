using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    public class DataPresionArterial
    {

        public DataPresionArterial()
        {
            periodos = new List<string>();
            valoresSistole = new List<double>();
            valoresDiastole = new List<double>();
            valoresMetaMinSistole = new List<double>();
            valoresMetaMinDiastole = new List<double>();
            valoresMetaMaxSistole = new List<double>();
            valoresMetaMaxDiastole = new List<double>();
        }

        public List<string> periodos { get; set; }
        public List<double> valoresSistole { get; set; }
        public List<double> valoresDiastole { get; set; }
        public List<double> valoresMetaMinSistole { get; set; }
        public List<double> valoresMetaMinDiastole { get; set; }
        public List<double> valoresMetaMaxSistole { get; set; }
        public List<double> valoresMetaMaxDiastole { get; set; }

    }
}

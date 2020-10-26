using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    public class DataSleepStatus
    {

        public DataSleepStatus()
        {
            periodos = new List<string>();
            colorDespierto = new List<string>();
            colorSuenioLigero = new List<string>();
            colorSuenioProfundo = new List<string>();

            valoresDespierto = new List<double>();
            valoresSuenioLigero = new List<double>();
            valoresSuenioProfundo = new List<double>();
        }

        public List<string> periodos { get; set; }
        public List<string> colorDespierto { get; set; }
        public List<string> colorSuenioLigero { get; set; }
        public List<string> colorSuenioProfundo { get; set; }
        public List<double> valoresDespierto { get; set; }
        public List<double> valoresSuenioLigero { get; set; }
        public List<double> valoresSuenioProfundo { get; set; }
    }
}

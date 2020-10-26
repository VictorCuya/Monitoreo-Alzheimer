using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    public class DataOxigenacion
    {
        public DataOxigenacion()
        {
            periodos = new List<string>();
            valoresOxigenacion = new List<double>();
            valoresLimiteOxigenacion = new List<double>();

        }

        public List<string> periodos { get; set; }
        public List<double> valoresOxigenacion { get; set; }
        public List<double> valoresLimiteOxigenacion { get; set; }
    }
}

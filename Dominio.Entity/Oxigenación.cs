using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    public class Oxigenación
    {
        public Object _id { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string DNIPaciente { get; set; }
        public Double Resultado { get; set; }

    }
}

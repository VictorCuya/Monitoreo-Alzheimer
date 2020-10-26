using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    public class DataGames
    {
        public DataGames()
        {
            periodos = new List<string>();
            valoresPuntaje = new List<int>();
            valoresMeta = new List<int>();
        }

        public List<string> periodos { get; set; }
        public List<int> valoresPuntaje { get; set; }
        public List<int> valoresMeta { get; set; }

    }
}

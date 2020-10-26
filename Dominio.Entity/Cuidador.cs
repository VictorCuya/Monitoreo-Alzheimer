using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class Cuidador
    {
        public Cuidador()
        {
            _id = new Object();
            nombreCuidador = "";
            apellidoCuidador = "";
            edadCuidador = 0;
            DNICuidador = "";
            distritoCuidador = "";
            correoCuidador = "";
            FechaCumpleCuidador = new DateTime();


        }

        public Object _id { get; set; }
        public string nombreCuidador { get; set; }
        public string apellidoCuidador { get; set; }
        public int? edadCuidador { get; set; }
        public string DNICuidador { get; set; }
        public string distritoCuidador { get; set; }    
        public string correoCuidador { get; set; }

        public DateTime? FechaCumpleCuidador { get; set; }

    }
}

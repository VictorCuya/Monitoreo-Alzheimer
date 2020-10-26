using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{

    [BsonIgnoreExtraElements]
    public class Alerts
    {
        public string DNI { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int edad { get; set; }
        public string message { get; set; }
        public DateTime created_at { get; set; }
        public string patient_id { get; set; } 
    }

}

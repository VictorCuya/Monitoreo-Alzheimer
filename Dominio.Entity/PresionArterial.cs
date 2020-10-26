using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class PresionArterial
    {

        public Object _id { get; set; }
        public int sistole { get; set; }
        public int diastole { get; set; }
        public int oxigeno { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string DNIPaciente { get; set; }



    }
}

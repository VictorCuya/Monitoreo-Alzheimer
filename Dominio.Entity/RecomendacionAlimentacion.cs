using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class RecomendacionAlimentacion
    {
        public Object _id { get; set; }
        public string DNI { get; set; }
        public string dieta { get; set; }
        public DateTime fecRegRecomendacionAlimentacion { get; set; }
    }

    public class RecomendacionAlimentacionData
    {
        public List<RecomendacionAlimentacion> data { get; set; }
    }

}

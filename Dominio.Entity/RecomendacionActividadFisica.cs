using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class RecomendacionActividadFisica
    {
        public Object _id { get; set; }
        public string DNI { get; set; }
        public string actividadFisica { get; set; }
        public int duracionActividadFisica { get; set; }
        public DateTime fecRegRecomendacionAfisica { get; set; }

    }

    public class RecomendacionActividadFisicaData
    {
        public List<RecomendacionActividadFisica> data { get; set; }
    }

}

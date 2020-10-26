using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class RecomendacionMedicacion
    {

        public Object _id { get; set; }
        public string DNI { get; set; }
        public string medicamento { get; set; }
        public int frecuencia { get; set; }
        public int duracionMedicacion { get; set; }
        public string notaAdicional { get; set; }
        public DateTime fecRegRecomendacionMedicacion { get; set; }



    }

    public class RecomendacionMedicacionData
    {
       public List<RecomendacionMedicacion> data { get; set; }
    }

}

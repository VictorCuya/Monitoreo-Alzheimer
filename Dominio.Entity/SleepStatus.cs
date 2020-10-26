using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class SleepStatus
    {

        public Object _id { get; set; }
        public int step { get; set; }
        public DateTime date { get; set; }
        public string patient_id { get; set; }

        public string day { get; set; }

    }
}

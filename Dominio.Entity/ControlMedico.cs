using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class ControlMedico
    {
        public Object _id { get; set; }
        public string DNI { get; set; }
        public string Observacion { get; set; }
        public DateTime fechaControl { get; set; }
    }

    public class ControlMedicoData
    {
        public List<ControlMedico> data { get; set; }
    }



}

using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class Medico
    {
        public Medico()
        {
            _id = new Object();
            nombre = "";
            apellido = "";
            correo = "";
            telefono = "";
            DNI = "";
            distrito = "";
            EntidadLaboral = "";
            grado = "";
            usuario = "";
            password = "";
        }
        public Object _id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }
        public string DNI { get; set; }
        public string distrito { get; set; }
        public string EntidadLaboral { get; set; }
        public string grado { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }


    }
}

using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class Limite
    {

        public Limite()
        {
            _id = new Object();
            DNI = "";
            limiteConcentracion = 0;
            limiteMemoria = 0;
            limiteCalculo = 0;
            limiteOxigeno = 0;
            limiteMinSistole = 0;
            limiteMaxSistole = 0;
            limiteMinDiastole = 0;
            limiteMaxDiastole = 0;

        }


        public Object _id { get; set; }
        public string DNI { get; set; }
        public Double limiteConcentracion { get; set; }
        public Double limiteMemoria { get; set; }
        public Double limiteCalculo { get; set; }
        public Double limiteOxigeno { get; set; }
        public Double limiteMinSistole { get; set; }
        public Double limiteMaxSistole { get; set; }
        public Double limiteMinDiastole { get; set; }
        public Double limiteMaxDiastole { get; set; }
    }
}

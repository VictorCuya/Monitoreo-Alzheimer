using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entity
{
    [BsonIgnoreExtraElements]
    public class Paciente
    {
        public Paciente()
        {
            _id = new Object();

            nombre = "";
            apellido = "";
            edad = null;
            distrito = "";
            faseEnfermedad = "";
            puntajeUltimoTestMMSE = 0;
            fechaNacimiento = new DateTime();
            DNI = "";
            UsuarioMedico = "";
            fechaInicio = new DateTime();
            fechaFin = new DateTime();
            correo = "";

            //-------
            nombreCuidador = "";
            apellidoCuidador = "";
            edadCuidador = null;
            DNICuidador = "";
            distritoCuidador = "";

            correoCuidador = "";
            Comentario = "";

            //--------------

            sistole = 0;
            diastole = 0;
            oxigeno = 0;


        }

        public Object _id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int? edad { get; set; }
        public string correo { get; set; }

        public string distrito { get; set; }
        public string faseEnfermedad { get; set; }
        public int puntajeUltimoTestMMSE { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string DNI { get; set; }

        public string UsuarioMedico { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFin { get; set; }

        //-------------------------------------------------------Datos del cuidador

        public string nombreCuidador { get; set; }
        public string apellidoCuidador { get; set; }
        public int? edadCuidador { get; set; }
        public string DNICuidador { get; set; }
        public string distritoCuidador { get; set; }

        public string correoCuidador { get; set; }
        public DateTime? FechaCumpleCuidador { get; set; }
        public string Comentario { get; set; }


        /// ------------------------------------ Datos de Seguimiento

        //...presion arterial
        public int sistole { get; set; }
        public int diastole { get; set; }
        //...oxigenacion
        public Double oxigeno { get; set; }

        public DateTime? fecTomaDatos { get; set; }
        //----------------------------------- Limites 
        public Double? limiteConcentracion{ get; set; }
        public Double? limiteMemoria { get; set; }
        public Double? limiteCalculo { get; set; }
        public Double? limiteOxigeno { get; set; }
        public Double? limiteMinSistole { get; set; }
        public Double? limiteMinDiastole { get; set; }
        public Double? limiteMaxSistole { get; set; }
        public Double? limiteMaxDiastole { get; set; }

        //----------------------------------- Recomendaciones de Alimentacion
        public string dieta { get; set; }
        public DateTime fecRegRecomendacionAlimentacion { get; set; }

        //------------------------------------- Recomendacion de Medicacion

        public string medicamento { get; set; }
        public int frecuencia { get; set; }
        public int duracionMedicacion { get; set; }
        public string notaAdicional { get; set; }
        public DateTime fecRegRecomendacionMedicacion { get; set; }

        //------------------------------------- Recomendacion de actividad fisica

        public string actividadFisica { get; set; }
        public int duracionActividadFisica { get; set; }
        public DateTime fecRegRecomendacionAfisica { get; set; }

        //------------------------------------- Recomendacion de actividad fisica
        public string Observacion { get; set; }
        public DateTime fechaControl { get; set; }

    }
}

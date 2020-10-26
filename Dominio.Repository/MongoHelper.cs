using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entity;

namespace Dominio.Repository
{
    public class MongoHelper
    {
        public static IMongoClient client { get; set; }
        public static IMongoDatabase database { get; set; }
        public static string MongoConnection = "mongodb+srv://userAlzheimer:Alzheimer@cluster0.a5yha.azure.mongodb.net/dbMonitoreoAlzheimer?retryWrites=true&w=majority";
        public static string MongoDatabase = "dbMonitoreoAlzheimer";

        public static IMongoCollection<Medico> medico_collection { get; set; }
        public static IMongoCollection<Paciente> paciente_collection { get; set; }
        public static IMongoCollection<Cuidador> cuidador_collection { get; set; }
        public static IMongoCollection<PresionArterial> PresionArterial_collection { get; set; }
        public static IMongoCollection<Oxigenación> oxigenacion_collection { get; set; }
        public static IMongoCollection<SleepStatus> suenio_collection { get; set; }
        public static IMongoCollection<Limite> limite_collection { get; set; }
        public static IMongoCollection<RecomendacionActividadFisica> actividadFisica_collection { get; set; }
        public static IMongoCollection<RecomendacionMedicacion> medicacion_collection { get; set; }
        public static IMongoCollection<RecomendacionAlimentacion> alimentacion_collection { get; set; }
        public static IMongoCollection<ControlMedico> controlMedico_collection { get; set; }
        public static IMongoCollection<Alerts> alerts_collection { get; set; }

        public static IMongoCollection<Games> games_collection { get; set; }
        public static void ConnectToMongoService()
        {
            try
            {
                client = new MongoClient(MongoConnection);
                database = client.GetDatabase(MongoDatabase);
            }
            catch (Exception)
            {
                throw;
            }

        }


   



    }
}

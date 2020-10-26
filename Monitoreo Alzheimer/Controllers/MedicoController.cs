using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dominio.Repository;
using Dominio.Entity;
using System.Web.Security;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Net.Mime;
using System.Web.Configuration;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using System.Web.Services;
using System.Globalization;

namespace Monitoreo_Alzheimer.Controllers
{
    [Authorize]
    public class MedicoController : Controller
    {
        [HttpGet]
        public ActionResult Perfil()
        {
            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.medico_collection = MongoHelper.database.GetCollection<Medico>("Medicos");

                string username = User.Identity.Name.ToString();

                var builder = Builders<Medico>.Filter;
                var filter1 = builder.Eq("usuario", username);
                var resultUsuario = new Medico();

                try { resultUsuario = MongoHelper.medico_collection.Find(filter1).First();
                    resultUsuario.password = Seguridad.DesEncriptar(resultUsuario.password).ToString();
                }
                catch { resultUsuario = null; }

                //ViewBag.Indicador = TempData["Indicador"];
                return View(resultUsuario);
            }
            catch (Exception)
            {
                if (User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                }

                return RedirectToAction("Login", "Home");
            }
        }


        [HttpPost]
        public ActionResult Perfil(FormCollection collection)
        {
            Medico medico = new Medico();

            medico.nombre = collection["nombre"];
            medico.apellido = collection["apellido"];
            medico.correo = collection["correo"];
            medico.telefono = collection["telefono"];
            medico.DNI = collection["telefono"];
            medico.distrito = collection["telefono"];
            medico.EntidadLaboral = collection["EntidadLaboral"];
            medico.grado = collection["grado"];
            medico.usuario = collection["usuario"];
            medico.password = collection["password"];

            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.medico_collection = MongoHelper.database.GetCollection<Medico>("Medicos");

                var builder = Builders<Medico>.Filter;
                var filter1 = builder.Eq("usuario", collection["usuario"]);
                var filter2 = builder.Eq("DNI", collection["DNI"]);
                var andFilter = filter1 & filter2;

                var update = Builders<Medico>.Update
                    .Set("nombre", collection["nombre"])
                    .Set("apellido", collection["apellido"])
                    .Set("correo", collection["correo"])
                    .Set("telefono", collection["telefono"])
                    .Set("DNI", collection["DNI"])
                    .Set("distrito", collection["distrito"])
                    .Set("EntidadLaboral", collection["EntidadLaboral"])
                    .Set("grado", collection["grado"])
                    .Set("usuario", collection["usuario"])
                    .Set("password", Seguridad.Encriptar(collection["password"])
                    
                    );

                var result = MongoHelper.medico_collection.UpdateOneAsync(andFilter, update);

                ViewBag.Indicador = 1;
            
                return View(medico);
            }
            catch (Exception)
            {
                
                ViewBag.Indicador = 2;
                return View(medico);
            }
        }
 
        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }

       public ActionResult ListaPacientes()
       {
            var resultPacientes = new List<Paciente>();

            try
            {
                string username = User.Identity.Name.ToString();
                var builder = Builders<Paciente>.Filter;
                var filter = builder.Eq("UsuarioMedico", username);


                MongoHelper.ConnectToMongoService();
                MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
                resultPacientes = MongoHelper.paciente_collection.Find(filter).ToList();


                foreach (var item in resultPacientes)
                {
                    var builder_01 = Builders<PresionArterial>.Filter;
                    var filterPaciente_01 = builder_01.Eq("DNIPaciente", item.DNI);
                    var resultados = new PresionArterial();

                

                    MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
                    resultados = MongoHelper.PresionArterial_collection.Find(filterPaciente_01).ToList().OrderByDescending(p => p.fechaRegistro).FirstOrDefault();

                    if (resultados == null)
                    {
                        item.sistole = 0;
                        item.diastole = 0;
                        item.oxigeno = 0;
                        item.fecTomaDatos = null;
                    }
                    else
                    {
                        item.sistole = resultados.sistole;
                        item.diastole = resultados.diastole;
                        item.oxigeno = resultados.oxigeno;
                        item.fecTomaDatos = resultados.fechaRegistro.AddHours(-5);
                    }

                }

                return View(resultPacientes);

            }
            catch (Exception)
            {
                return View(resultPacientes);
            }       
        }

        public ActionResult ListaAlertas()
        {
          
            try
            {
                string username = User.Identity.Name.ToString();
                var builder = Builders<Paciente>.Filter;
                var filter = builder.Eq("UsuarioMedico", username);

                MongoHelper.ConnectToMongoService();
                MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
                MongoHelper.alerts_collection = MongoHelper.database.GetCollection<Alerts>("Alerts");

                var resultPacientes = new List<Paciente>();
                resultPacientes = MongoHelper.paciente_collection.Find(filter).ToList();
                var resultAlerts = new List<Alerts>();
                resultAlerts = MongoHelper.alerts_collection.Find(p => p.created_at >= DateTime.Now.AddDays(-30) && p.created_at <= DateTime.Now).ToList();

                var resultado = from a in resultAlerts
                                join b in resultPacientes on a.patient_id.ToString() equals b._id.ToString() into lista
                                from c in lista
                                select new { c.nombre, c.apellido, c.edad, a.message, a.created_at,c.DNI};

                List<Alerts> alertas = new List<Alerts>();
                
                foreach(var item in resultado)
                {
                    Alerts alert = new Alerts();
                    alert.DNI = item.DNI;
                    alert.nombre = item.nombre;
                    alert.apellido = item.apellido;
                    alert.edad = (int)item.edad;
                    alert.message = item.message;
                    alert.created_at = item.created_at;
                    alertas.Add(alert);
                }                
                return View(alertas);       
                
            }
            catch
            {
                List<Alerts> resultado = new List<Alerts>();
                return View(resultado);
            }    
        }
        public ActionResult BuscarCuidador(string DNI)
        {
          
            try
            {
                var builder = Builders<Cuidador>.Filter;
                var filter = builder.Eq("DNICuidador", DNI);
                var resultCuidador = new Cuidador();

                try {

                    MongoHelper.ConnectToMongoService();
                    MongoHelper.cuidador_collection = MongoHelper.database.GetCollection<Cuidador>("Cuidador");
                    resultCuidador = MongoHelper.cuidador_collection.Find(filter).First();
             

                } catch(Exception) { 
                    resultCuidador = null; 
                }                           
                return Json(resultCuidador, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                var cuidadorTemp = new Cuidador();
                cuidadorTemp = null;
                return Json(cuidadorTemp, JsonRequestBehavior.AllowGet);
            }      
        }



        public Cuidador ValidarIngresoCuidador(string DNI)
        {
            try
            {
                var builder = Builders<Cuidador>.Filter;
                var filter = builder.Eq("DNICuidador", DNI);
                var resultCuidador = new Cuidador();

                try
                {
                    MongoHelper.ConnectToMongoService();
                    MongoHelper.cuidador_collection = MongoHelper.database.GetCollection<Cuidador>("Cuidador");
                    resultCuidador = MongoHelper.cuidador_collection.Find(filter).First();


                }
                catch (Exception)
                {
                    resultCuidador = null;
                }

                return resultCuidador;
            }
            catch
            {
                return null;
            }
        }

        public ActionResult RegistrarPaciente()
        {
            Paciente paciente = new Paciente();
            return View(paciente);
        }


        [HttpPost]
        public ActionResult RegistrarPaciente(FormCollection collection)
        {
            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
                MongoHelper.cuidador_collection = MongoHelper.database.GetCollection<Cuidador>("Cuidador");
                string mensaje = collection["Comentario"];
                var builder = Builders<Paciente>.Filter;
                var filter = builder.Eq("DNI", collection["DNI"]);     
                var resultUsuario = new Paciente();

                try { resultUsuario = MongoHelper.paciente_collection.Find(filter).First(); } catch { resultUsuario = null; }

                if(resultUsuario == null)
                {                                     
                    string username = User.Identity.Name.ToString();
                    Object id = GenerateRandomId(24);
                                  
                    MongoHelper.paciente_collection.InsertOneAsync(new Paciente
                    {
                        _id = id,
                        nombre = collection["nombre"],
                        apellido = collection["apellido"],
                        edad = Convert.ToInt32(collection["edad"]),
                        distrito = collection["distrito"],
                        faseEnfermedad = collection["faseEnfermedad"],
                        puntajeUltimoTestMMSE = Convert.ToInt32(collection["puntajeUltimoTestMMSE"]),
                        fechaNacimiento = Convert.ToDateTime(collection["fechaNacimiento"]),
                        DNI = collection["DNI"],
                        UsuarioMedico = username,
                        fechaInicio = Convert.ToDateTime(collection["fechaInicio"]),
                        fechaFin = Convert.ToDateTime(collection["fechaFin"]),
                        correo = collection["correo"],
                        //------------------------ Datos del cuidador
                        nombreCuidador = collection["nombreCuidador"],
                        apellidoCuidador = collection["apellidoCuidador"],
                        edadCuidador = Convert.ToInt32(collection["edadCuidador"]),
                        DNICuidador = collection["DNICuidador"],
                        distritoCuidador = collection["distritoCuidador"],     
                        correoCuidador = collection["correoCuidador"]

                    });


                    if (mensaje == "El cuidador no se encuentra registrado" || mensaje == "") 
                    {

                        if (ValidarIngresoCuidador(collection["DNICuidador"].ToString()) == null) {

                            Object id_c = GenerateRandomId(24);
                            MongoHelper.cuidador_collection.InsertOneAsync(new Cuidador
                            {
                                _id = id_c,
                                //------------------------ Datos del cuidador
                                nombreCuidador = collection["nombreCuidador"],
                                apellidoCuidador = collection["apellidoCuidador"],
                                edadCuidador = Convert.ToInt32(collection["edadCuidador"]),
                                DNICuidador = collection["DNICuidador"],
                                distritoCuidador = collection["distritoCuidador"],
                                correoCuidador = collection["correoCuidador"],
                                FechaCumpleCuidador = Convert.ToDateTime(collection["FechaCumpleCuidador"])

                            });
                        }
                    }

                    string msjCuidador = "Estimado(a): " + collection["nombreCuidador"].ToUpper() + " " + collection["apellidoCuidador"].ToUpper() +
                                         "<br/>Es gusto poder saludarlo. El paciente:" + collection["nombre"].ToUpper() + " " + collection["apellido"].ToUpper() +
                                         " ha sido registrado en su lista, la cual puede visualizar en la aplicación móvil ALZCare. " +
                                         "<br/><br/><strong>Confiamos en usted</strong>" +
                                         "<br/>------------------------------------------------" +
                                         "<br/><br/><strong> Sus credenciales de acceso son:</strong>" +
                                         "<br/> <strong>DNI: </strong>" + collection["DNICuidador"] +
                                         "<br/> <strong>Fecha de nacimiento: </strong>" + collection["FechaCumpleCuidador"];
                                         


                    string msjPaciente = "Estimado(a): " + collection["nombre"].ToUpper() + " " + collection["apellido"].ToUpper() +
                                         "<br/>Se han registrado sus datos en la plataforma ALZCare de manera exitosa. Podras visualizar tus datos en la aplicacion móvil. " +
                                         "Su cuidador(a) asignado es: " + collection["nombreCuidador"].ToUpper() + " " + collection["apellidoCuidador"].ToUpper()+
                                         "<br/><br/><strong>Gracias por confiar en nosotros</strong>" +
                                         "<br/>------------------------------------------------" +
                                         "<br/><br/> <strong>Sus credenciales de acceso son:</strong>" +
                                         "<br/> <strong>DNI: </strong>" + collection["DNI"] +
                                         "<br/> <strong>Fecha de nacimiento: </strong>" + collection["fechaNacimiento"];


                    EnviarCorreoConfirmacion(collection["correoCuidador"].ToString(), "Confirmación de Registro - ALZCare", msjCuidador);
                    EnviarCorreoConfirmacion(collection["correo"], "Confirmación de Registro - ALZCare", msjPaciente);

                    ViewBag.Indicador = 1;
                    return View();
                }
                else
                {
                    ViewBag.Indicador = 2;
                    return View();
                }
              
            }
            catch (Exception)
            {
                ViewBag.Indicador = 3;
                return View();
            }
           
        }

        public void EnviarCorreoConfirmacion(string destinatario, string asunto, string correo)
        {

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("alzCaremonitoreoalzheimer@gmail.com");

            mail.To.Add(destinatario);
            mail.Subject = asunto;
            mail.IsBodyHtml = true;
            mail.Body = correo;

            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 25; //465; //587
            smtp.Credentials = new NetworkCredential("alzCaremonitoreoalzheimer@gmail.com", "Alzheimer123");

            smtp.EnableSsl = true;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("No se ha podido enviar el email", ex.InnerException);
            }
           
        }

        private static Random random = new Random();
        private object GenerateRandomId(int v)
        {
            string strarray = "abcdefghijklmnopqrstuvwxyz123456789";
            return new string(Enumerable.Repeat(strarray, v).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public ActionResult DashboardPaciente(string id)
        {

            MongoHelper.ConnectToMongoService();
            MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");

            var filterPaciente = Builders<Paciente>.Filter.Eq("DNI", id);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", id);

            var resultPaciente = new Paciente();
            var resultLimites = new Limite();

            try
            {
                resultPaciente = MongoHelper.paciente_collection.Find(filterPaciente).First();
            }
            catch { 
                resultPaciente.nombre = "SIN";
                resultPaciente.apellido = "INFORMACION";
            }

            try { 
            
                resultLimites = MongoHelper.limite_collection.Find(filterLimite).First(); 
            
            } catch (Exception){ 
                
                resultLimites = null; 
            }
           


            if (resultLimites != null)
            {
                resultPaciente.limiteConcentracion = resultLimites.limiteConcentracion;
                resultPaciente.limiteMemoria = resultLimites.limiteMemoria;
                resultPaciente.limiteCalculo = resultLimites.limiteCalculo;
                resultPaciente.limiteMinDiastole = resultLimites.limiteMinDiastole;
                resultPaciente.limiteMinSistole = resultLimites.limiteMinSistole;
                resultPaciente.limiteMaxDiastole = resultLimites.limiteMaxDiastole;
                resultPaciente.limiteMaxSistole = resultLimites.limiteMaxSistole;
                resultPaciente.limiteOxigeno = resultLimites.limiteOxigeno;
            }
            else
            {
                resultPaciente.limiteConcentracion = 0;
                resultPaciente.limiteMemoria = 0;
                resultPaciente.limiteCalculo = 0;
                resultPaciente.limiteMinDiastole = 0;
                resultPaciente.limiteMinSistole = 0;
                resultPaciente.limiteMaxDiastole = 0;
                resultPaciente.limiteMaxSistole = 0;
                resultPaciente.limiteOxigeno = 0;
            }
            ///////////----------------------------------
            
          
            return View(resultPaciente);


        }



        public ActionResult GuardarLimites(List<Limite> Limites)
        {         
            try
            {
                

                MongoHelper.ConnectToMongoService();
                MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");

                var filter = Builders<Limite>.Filter.Eq("DNI", Limites[0].DNI);
                var consulta = new Limite();

                try { consulta = MongoHelper.limite_collection.Find(filter).First(); } catch { consulta = null; }

                if(consulta == null)
                {
                    Object id = GenerateRandomId(24);
                    MongoHelper.limite_collection.InsertOneAsync(new Limite
                    {
                        _id = id,
                        DNI = Limites[0].DNI,
                        limiteConcentracion = Limites[0].limiteConcentracion,
                        limiteMemoria = Limites[0].limiteMemoria,
                        limiteCalculo = Limites[0].limiteCalculo,

                        limiteMinSistole = Limites[0].limiteMinSistole,
                        limiteMinDiastole = Limites[0].limiteMinDiastole,
                        limiteMaxSistole = Limites[0].limiteMaxSistole,
                        limiteMaxDiastole = Limites[0].limiteMaxDiastole,
                        limiteOxigeno = Limites[0].limiteOxigeno

                    }) ;
                }
                else
                {

                    var update = Builders<Limite>.Update
                       .Set("limiteConcentracion", Limites[0].limiteConcentracion)
                       .Set("limiteMemoria", Limites[0].limiteMemoria)
                       .Set("limiteCalculo", Limites[0].limiteCalculo)
                       .Set("limiteOxigeno", Limites[0].limiteOxigeno)
                       .Set("limiteMinSistole", Limites[0].limiteMinSistole)
                       .Set("limiteMinDiastole", Limites[0].limiteMinDiastole)
                       .Set("limiteMaxSistole", Limites[0].limiteMaxSistole)
                       .Set("limiteMaxDiastole", Limites[0].limiteMaxDiastole);


                    var result = MongoHelper.limite_collection.UpdateOneAsync(filter, update);

                }              

                var data = new { Success = "True", Message = "Se han modificado los datos correctamente" };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                var data = new { Success = "False", Message = "Se han producido un error al actualizar" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }  

        }


        public ActionResult GuardarActividadFisica(List<RecomendacionActividadFisica> ActividadFisica)
        {
            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.actividadFisica_collection = MongoHelper.database.GetCollection<RecomendacionActividadFisica>("RActividadFisica");

                Object id = GenerateRandomId(24);
                MongoHelper.actividadFisica_collection.InsertOneAsync(new RecomendacionActividadFisica
                {
                    _id = id,
                    DNI = ActividadFisica[0].DNI,
                    actividadFisica = ActividadFisica[0].actividadFisica,
                    duracionActividadFisica = ActividadFisica[0].duracionActividadFisica,                
                    fecRegRecomendacionAfisica = DateTime.Now

                });

                var data = new { Success = "True", Message = "Los datos se han registrado correctamente" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch {

                var data = new { Success = "False", Message = "Se han producido un error, volver a intentar" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }
       
        }

        public ActionResult GuardarMedicacion(List<RecomendacionMedicacion> Medicacion)
        {
            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.medicacion_collection = MongoHelper.database.GetCollection<RecomendacionMedicacion>("RMedicacion");
                Object id = GenerateRandomId(24);

                MongoHelper.medicacion_collection.InsertOneAsync(new RecomendacionMedicacion
                {
                    _id = id,
                    DNI = Medicacion[0].DNI,
                    medicamento = Medicacion[0].medicamento,
                    frecuencia = Medicacion[0].frecuencia,
                    duracionMedicacion = Medicacion[0].duracionMedicacion,
                    notaAdicional = Medicacion[0].notaAdicional,
                    fecRegRecomendacionMedicacion = DateTime.Now

                });

                var data = new { Success = "True", Message = "Los datos se han registrado correctamente" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var data = new { Success = "False", Message = "Se han producido un error, volver a intentar" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }
           
          
        }

        public ActionResult GuardarRecAlimentacion(List<RecomendacionAlimentacion> Alimentacion)
        {
           try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.alimentacion_collection = MongoHelper.database.GetCollection<RecomendacionAlimentacion>("RAlimentacion");
                Object id = GenerateRandomId(24);

                MongoHelper.alimentacion_collection.InsertOneAsync(new RecomendacionAlimentacion
                {
                    _id = id,
                    DNI = Alimentacion[0].DNI,
                    dieta = Alimentacion[0].dieta,           
                    fecRegRecomendacionAlimentacion = DateTime.Now

                });

                var data = new { Success = "True", Message = "Los datos se han registrado correctamente" };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {

                var data = new { Success = "False", Message = "Se han producido un error, volver a intentar" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }

        }


        public ActionResult GuardarDatosControlMedico(List<ControlMedico> control)
        {

            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.controlMedico_collection = MongoHelper.database.GetCollection<ControlMedico>("ControlMedico");
                Object id = GenerateRandomId(24);

                MongoHelper.controlMedico_collection.InsertOneAsync(new ControlMedico
                {
                    _id = id,
                    DNI = control[0].DNI,
                    Observacion = control[0].Observacion,
                    fechaControl = DateTime.Now

                });

                var data = new { Success = "True", Message = "Los datos se han registrado correctamente" };
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                var data = new { Success = "False", Message = "Se han producido un error, volver a intentar" };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
   
        }
        public JsonResult ObtenerRecomendacionesMedicacion(string DNI)
        {
            
            MongoHelper.ConnectToMongoService();
            MongoHelper.medicacion_collection = MongoHelper.database.GetCollection<RecomendacionMedicacion>("RMedicacion");
            var filter = Builders<RecomendacionMedicacion>.Filter.Eq("DNI", DNI);
            var result = new List<RecomendacionMedicacion>();
            result = MongoHelper.medicacion_collection.Find(filter).ToList().OrderByDescending(P => P.fecRegRecomendacionMedicacion).ToList();
            RecomendacionMedicacionData lista = new RecomendacionMedicacionData();
            lista.data = result.ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ObtenerRecomendacionesAlimentacion(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.alimentacion_collection = MongoHelper.database.GetCollection<RecomendacionAlimentacion>("RAlimentacion");
            var filter = Builders<RecomendacionAlimentacion>.Filter.Eq("DNI", DNI);
            var result = new List<RecomendacionAlimentacion>();
            result = MongoHelper.alimentacion_collection.Find(filter).ToList().OrderByDescending(P => P.fecRegRecomendacionAlimentacion).ToList();
            RecomendacionAlimentacionData lista = new RecomendacionAlimentacionData();
            lista.data = result.ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerRecomendacionesFisicas(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.actividadFisica_collection = MongoHelper.database.GetCollection<RecomendacionActividadFisica>("RActividadFisica");
            var filter = Builders<RecomendacionActividadFisica>.Filter.Eq("DNI", DNI);
            var result = new List<RecomendacionActividadFisica>();
            result = MongoHelper.actividadFisica_collection.Find(filter).ToList().OrderByDescending(P => P.fecRegRecomendacionAfisica).ToList();
            RecomendacionActividadFisicaData lista = new RecomendacionActividadFisicaData();
            lista.data = result.ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerControlesMedicos(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.controlMedico_collection = MongoHelper.database.GetCollection<ControlMedico>("ControlMedico");
            var filter = Builders<ControlMedico>.Filter.Eq("DNI", DNI);
            var result = new List<ControlMedico>();
            result = MongoHelper.controlMedico_collection.Find(filter).ToList().OrderByDescending(P => P.fechaControl).ToList();
            ControlMedicoData lista = new ControlMedicoData();
            lista.data = result.ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }   
        public JsonResult UltimaMedicionPresionArterial(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataPresionArterial data = new DataPresionArterial();

            var filter = Builders<PresionArterial>.Filter.Eq("DNIPaciente", DNI);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var result = new PresionArterial();   
            var limites = new Limite();

            try { result = MongoHelper.PresionArterial_collection.Find(filter).ToList().OrderByDescending(P => P.fechaRegistro).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaPresionArterial = (from d in MongoHelper.PresionArterial_collection.Find(filter).ToList().Where(P => P.fechaRegistro.Date == result.fechaRegistro.Date).OrderByDescending(P => P.fechaRegistro).ToList()
                                 orderby d.fechaRegistro ascending
                                 select new { Datetime=d.fechaRegistro.AddHours(-5), d.sistole, d.diastole }).Reverse().Take(12).Reverse().ToList();

                foreach (var item in listaPresionArterial)
                {
                    data.periodos.Add(item.Datetime.ToString("HH:mm"));

                    data.valoresSistole.Add(item.sistole);
                    data.valoresDiastole.Add(item.diastole);
                    data.valoresMetaMinSistole.Add((double)limites.limiteMinSistole);
                    data.valoresMetaMinDiastole.Add((double)limites.limiteMinDiastole);
                    data.valoresMetaMaxSistole.Add((double)limites.limiteMaxSistole);
                    data.valoresMetaMaxDiastole.Add((double)limites.limiteMaxDiastole);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult PresionArterialDiario(string DNI)
        {
            MongoHelper.ConnectToMongoService();

            MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataPresionArterial data = new DataPresionArterial();

            var filter = Builders<PresionArterial>.Filter.Eq("DNIPaciente", DNI);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);


            var listaSignosVitales = new List<PresionArterial>();
            var limites = new Limite();

            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            try { listaSignosVitales = MongoHelper.PresionArterial_collection.Find(filter).ToList().OrderByDescending(P => P.fechaRegistro).ToList(); } catch { listaSignosVitales = null; }

            if(listaSignosVitales != null)
            {
                var listfinal = (from presion in listaSignosVitales
                               orderby presion.fechaRegistro ascending
                               group presion by presion.fechaRegistro.AddHours(-5).Date.ToString("dd-MM-yyyy") into g                       
                               select new { periodo = g.Key, sistole = g.Average(s => s.sistole),diastole = g.Average(s => s.diastole) }).Reverse().Take(7).Reverse().ToList();
                
                foreach (var item in listfinal)
                {
                    data.periodos.Add(item.periodo);
                    data.valoresDiastole.Add((int)item.diastole);
                    data.valoresSistole.Add((int)item.sistole);
                    data.valoresMetaMaxSistole.Add((int)limites.limiteMaxSistole);
                    data.valoresMetaMaxDiastole.Add((int)limites.limiteMaxDiastole);
                    data.valoresMetaMinSistole.Add((int)limites.limiteMinSistole);
                    data.valoresMetaMinDiastole.Add((int)limites.limiteMinDiastole);
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PresionArterialMensual(string DNI)
        {
            MongoHelper.ConnectToMongoService();

            MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataPresionArterial data = new DataPresionArterial();

            var filter = Builders<PresionArterial>.Filter.Eq("DNIPaciente", DNI);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var listaSignosVitales = new List<PresionArterial>();
            var limites = new Limite();

            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }
            try { listaSignosVitales = MongoHelper.PresionArterial_collection.Find(filter).ToList().OrderByDescending(P => P.fechaRegistro).ToList(); } catch { listaSignosVitales = null; }

            if (listaSignosVitales != null)
            {
                var listfinal = (from presion in listaSignosVitales
                                 orderby presion.fechaRegistro ascending
                                 group presion by presion.fechaRegistro.AddHours(-5).ToString("yyyy-MM") into g
                                 select new { periodo = g.Key, sistole = g.Average(s => s.sistole), diastole = g.Average(s => s.diastole) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listfinal)
                {
                    data.periodos.Add(item.periodo);
                    data.valoresDiastole.Add((int)item.diastole);
                    data.valoresSistole.Add((int)item.sistole);
                    data.valoresMetaMaxSistole.Add((int)limites.limiteMaxSistole);
                    data.valoresMetaMaxDiastole.Add((int)limites.limiteMaxDiastole);
                    data.valoresMetaMinSistole.Add((int)limites.limiteMinSistole);
                    data.valoresMetaMinDiastole.Add((int)limites.limiteMinDiastole);
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UltimaMedicionOxigenacion(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataOxigenacion data = new DataOxigenacion();

            var filter = Builders<PresionArterial>.Filter.Eq("DNIPaciente", DNI);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var result = new PresionArterial();
            var limites = new Limite();

            try { result = MongoHelper.PresionArterial_collection.Find(filter).ToList().OrderByDescending(P => P.fechaRegistro).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaOxigenacion = (from d in MongoHelper.PresionArterial_collection.Find(filter).ToList().Where(P => P.fechaRegistro.Date == result.fechaRegistro.Date).OrderByDescending(P => P.fechaRegistro).ToList()
                                       orderby d.fechaRegistro ascending
                                       select new { Datetime = d.fechaRegistro.AddHours(-5), d.oxigeno }).Reverse().Take(12).Reverse().ToList();

                foreach (var item in listaOxigenacion)
                {
                    data.periodos.Add(item.Datetime.ToString("HH:mm"));
                    data.valoresOxigenacion.Add(item.oxigeno);
                    data.valoresLimiteOxigenacion.Add((double)limites.limiteOxigeno);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult OxigenacionDiario(string DNI)
        {
            MongoHelper.ConnectToMongoService();

            MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataOxigenacion data = new DataOxigenacion();

            var filter = Builders<PresionArterial>.Filter.Eq("DNIPaciente", DNI);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var listaOxigenacion = new List<PresionArterial>();
            var limites = new Limite();

            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            try { listaOxigenacion = MongoHelper.PresionArterial_collection.Find(filter).ToList().OrderByDescending(P => P.fechaRegistro).ToList(); } catch { listaOxigenacion = null; }

            if (listaOxigenacion != null)
            {
                var listfinal = (from oxigenacion in listaOxigenacion
                                 orderby oxigenacion.fechaRegistro ascending
                                 group oxigenacion by oxigenacion.fechaRegistro.AddHours(-5).Date.ToString("dd-MM-yyyy") into g
                                 select new { periodo = g.Key, oxigeno = g.Average(s => s.oxigeno) }).Reverse().Take(7).Reverse().ToList();
                foreach (var item in listfinal)
                {
                    data.periodos.Add(item.periodo);
                    data.valoresOxigenacion.Add((int)item.oxigeno);
                    data.valoresLimiteOxigenacion.Add((int)limites.limiteOxigeno);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public JsonResult OxigenacionMensual(string DNI)
        {
            MongoHelper.ConnectToMongoService();

            MongoHelper.PresionArterial_collection = MongoHelper.database.GetCollection<PresionArterial>("PresionArterial");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataOxigenacion data = new DataOxigenacion();

            var filter = Builders<PresionArterial>.Filter.Eq("DNIPaciente", DNI);
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var listaOxigenacion = new List<PresionArterial>();
            var limites = new Limite();

            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }
            try { listaOxigenacion = MongoHelper.PresionArterial_collection.Find(filter).ToList().OrderByDescending(P => P.fechaRegistro).ToList(); } catch { listaOxigenacion = null; }

            if (listaOxigenacion != null)
            {
                var listfinal = (from oxigenacion in listaOxigenacion
                                 orderby oxigenacion.fechaRegistro ascending
                                 group oxigenacion by oxigenacion.fechaRegistro.AddHours(-5).Date.ToString("yyyy-MM") into g
                                 select new { periodo = g.Key, oxigeno = g.Average(s => s.oxigeno) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listfinal)
                {
                    data.periodos.Add(item.periodo);
                    data.valoresOxigenacion.Add((int)item.oxigeno);
                    data.valoresLimiteOxigenacion.Add((int)limites.limiteOxigeno);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //--------------------------------------------------GRAFICO DE ESTADO DE SUEÑO


        public JsonResult UltimaMedicionEstadoSuenio(string DNI)
        {

            MongoHelper.ConnectToMongoService();
            MongoHelper.suenio_collection = MongoHelper.database.GetCollection<SleepStatus>("SleepStatus");
            MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
            DataSleepStatus data = new DataSleepStatus();
            var patient = new Paciente();
            var filter = Builders<Paciente>.Filter.Eq("DNI", DNI);
            
            patient = MongoHelper.paciente_collection.Find(filter).First();

            string prueba = patient._id.ToString();
            var filtersuenio = Builders<SleepStatus>.Filter.Eq("patient_id",patient._id.ToString());

            var result = new SleepStatus();

            try { result = MongoHelper.suenio_collection.Find(filtersuenio).ToList().OrderByDescending(p => p.date).FirstOrDefault(); } 
            catch(Exception ) { result = null; }

            if (result != null) 
            {
                var listaSleepStatus = from d in MongoHelper.suenio_collection.Find(filtersuenio).ToList().Where(p => p.day == result.day).ToList()
                                       select new { d.date, d.step };

                double despierto = listaSleepStatus.Count(p => p.step == 0);
                double suenioLigero = listaSleepStatus.Count(p => p.step > 0 && p.step < 80);
                double suenioProfundo = listaSleepStatus.Count(p => p.step >= 80);

                data.periodos.Add(result.date.ToString("dd-MM-yyyy"));
                data.valoresDespierto.Add(Math.Round(despierto / 60, 2));
                data.valoresSuenioLigero.Add(Math.Round(suenioLigero / 60, 2));
                data.valoresSuenioProfundo.Add(Math.Round(suenioProfundo / 60, 2));
                data.colorDespierto.Add("#01579B");
                data.colorSuenioLigero.Add("#8C9EFF");
                data.colorSuenioProfundo.Add("#4FC3F7");
            }

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EstadoSuenioDiario(string DNI)
        {

            MongoHelper.ConnectToMongoService();
            MongoHelper.suenio_collection = MongoHelper.database.GetCollection<SleepStatus>("SleepStatus");
            MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
            DataSleepStatus data = new DataSleepStatus();
            var patient = new Paciente();
            var filter = Builders<Paciente>.Filter.Eq("DNI", DNI);

            patient = MongoHelper.paciente_collection.Find(filter).First();

            string prueba = patient._id.ToString();
            var filtersuenio = Builders<SleepStatus>.Filter.Eq("patient_id", patient._id.ToString());

            var result = new SleepStatus();

            try { result = MongoHelper.suenio_collection.Find(filtersuenio).ToList().OrderByDescending(p => p.date).FirstOrDefault(); }
            catch (Exception) { result = null; }

            if (result != null)
            {
        
                var listaSleepStatus = from suenio in MongoHelper.suenio_collection.Find(filtersuenio).ToList().Where(p => p.date >= result.date.AddDays(-5)).ToList()
                                       orderby suenio.date ascending
                                       group suenio by suenio.date.Date.ToString("dd-MM-yyyy") into g
                                       select new { periodo = g.Key, despierto = g.Count(p => p.step == 0), ligero = g.Count(p => p.step > 0 && p.step < 80),profundo=g.Count(p => p.step >= 80) };

                foreach(var item in listaSleepStatus)
                {
                    data.periodos.Add(item.periodo);
                    data.valoresDespierto.Add(Math.Round((double)item.despierto / 60, 2));
                    data.valoresSuenioLigero.Add(Math.Round((double)item.ligero / 60, 2));
                    data.valoresSuenioProfundo.Add(Math.Round((double)item.profundo / 60, 2));
                    data.colorDespierto.Add("#01579B");
                    data.colorSuenioLigero.Add("#8C9EFF");
                    data.colorSuenioProfundo.Add("#4FC3F7");
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EstadoSuenioMensual(string DNI)
        {

            MongoHelper.ConnectToMongoService();
            MongoHelper.suenio_collection = MongoHelper.database.GetCollection<SleepStatus>("SleepStatus");
            MongoHelper.paciente_collection = MongoHelper.database.GetCollection<Paciente>("Paciente");
            DataSleepStatus data = new DataSleepStatus();
            var patient = new Paciente();
            var filter = Builders<Paciente>.Filter.Eq("DNI", DNI);

            patient = MongoHelper.paciente_collection.Find(filter).First();

            string prueba = patient._id.ToString();
            var filtersuenio = Builders<SleepStatus>.Filter.Eq("patient_id", patient._id.ToString());

            var result = new SleepStatus();

            try { result = MongoHelper.suenio_collection.Find(filtersuenio).ToList().OrderByDescending(p => p.date).FirstOrDefault(); }
            catch (Exception) { result = null; }

            if (result != null)
            {

                var SleepStatusPreview = from suenio in MongoHelper.suenio_collection.Find(filtersuenio).ToList().Where(p => p.date >= result.date.AddMonths(-5)).ToList()
                                       orderby suenio.date ascending
                                       group suenio by suenio.date.Date into g
                                       select new { periodo = g.Key, despierto = g.Count(p => p.step == 0), ligero = g.Count(p => p.step > 0 && p.step < 80), profundo = g.Count(p => p.step >= 80) };


                var listaSleepStatus = from suenio in SleepStatusPreview
                                       group suenio by suenio.periodo.Date.ToString("yyyy-MM") into g
                                       select new { periodo = g.Key, despierto = g.Average(s => s.despierto), ligero = g.Average(s => s.ligero), profundo = g.Average(s => s.profundo) };

                foreach (var item in listaSleepStatus)
                {
                    data.periodos.Add(item.periodo);
                    data.valoresDespierto.Add(Math.Round((double)item.despierto / 60, 2));
                    data.valoresSuenioLigero.Add(Math.Round((double)item.ligero / 60, 2));
                    data.valoresSuenioProfundo.Add(Math.Round((double)item.profundo / 60, 2));
                    data.colorDespierto.Add("#01579B");
                    data.colorSuenioLigero.Add("#8C9EFF");
                    data.colorSuenioProfundo.Add("#4FC3F7");
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //--------------------------------------------------GRAFICO DE MEMORIA

        public JsonResult UltimaMedicionGameMemoria(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();

         
            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "MEMORY").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "MEMORY").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  select new { Datetime = d.game_played_at.AddHours(-5), d.game_score }).Reverse().Take(12).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime.ToString("HH:mm"));
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteMemoria);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DiarioGameMemoria(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "MEMORY").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "MEMORY").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  group d by d.game_played_at.AddHours(-5).Date.ToString("dd-MM-yyyy") into g
                                  select new { Datetime = g.Key.ToString(), game_score = g.Average(s => s.game_score) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime);
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteMemoria);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MensualGameMemoria(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "MEMORY").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "MEMORY").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  group d by d.game_played_at.AddHours(-5).Date.ToString("yyyy-MM") into g
                                  select new { Datetime = g.Key.ToString(), game_score = g.Average(s => s.game_score) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime);
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteMemoria);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        //-----------------------------GRAFICO DE CONCENTRACION

        public JsonResult UltimaMedicionGameConcentracion(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "CONCENTRATION").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "CONCENTRATION").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  select new { Datetime = d.game_played_at.AddHours(-5), d.game_score }).Reverse().Take(12).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime.ToString("HH:mm"));
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteConcentracion);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DiarioGameConcentracion(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "CONCENTRATION").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "CONCENTRATION").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  group d by d.game_played_at.AddHours(-5).Date.ToString("dd-MM-yyyy") into g
                                  select new { Datetime = g.Key.ToString(), game_score = g.Average(s => s.game_score) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime);
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteConcentracion);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MensualGameConcentracion(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "CONCENTRATION").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "CONCENTRATION").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  group d by d.game_played_at.AddHours(-5).Date.ToString("yyyy-MM") into g
                                  select new { Datetime = g.Key.ToString(), game_score = g.Average(s => s.game_score) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime);
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteConcentracion);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //-----------------------------GRAFICO DE CALCULO

        public JsonResult UltimaMedicionGameCalculo(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "CALCULATION").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "CALCULATION").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  select new { Datetime = d.game_played_at.AddHours(-5), d.game_score }).Reverse().Take(12).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime.ToString("HH:mm"));
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteCalculo);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DiarioGameCalculo(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "CALCULATION").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "CALCULATION").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  group d by d.game_played_at.AddHours(-5).Date.ToString("dd-MM-yyyy") into g
                                  select new { Datetime = g.Key.ToString(), game_score = g.Average(s => s.game_score) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime);
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteCalculo);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MensualGameCalculo(string DNI)
        {
            MongoHelper.ConnectToMongoService();
            MongoHelper.games_collection = MongoHelper.database.GetCollection<Games>("Games");
            MongoHelper.limite_collection = MongoHelper.database.GetCollection<Limite>("Limite");
            DataGames data = new DataGames();


            var filterLimite = Builders<Limite>.Filter.Eq("DNI", DNI);
            var paciente = new Paciente();
            var result = new Games();
            var limites = new Limite();

            paciente = MongoHelper.paciente_collection.Find(p => p.DNI == DNI).FirstOrDefault();
            var filter = Builders<Games>.Filter.Eq("patient_id", paciente._id.ToString());

            try { result = MongoHelper.games_collection.Find(filter).ToList().Where(p => p.game_name == "CALCULATION").OrderByDescending(P => P.game_played_at).ToList().FirstOrDefault(); } catch { result = null; }
            try { limites = MongoHelper.limite_collection.Find(filterLimite).First(); } catch { limites = new Limite(); }

            if (result != null)
            {
                var listaGames = (from d in MongoHelper.games_collection.Find(filter).ToList().Where(P => P.game_played_at.Date == result.game_played_at.Date && P.game_name == "CALCULATION").OrderByDescending(P => P.game_played_at).ToList()
                                  orderby d.game_played_at ascending
                                  group d by d.game_played_at.AddHours(-5).Date.ToString("yyyy-MM") into g
                                  select new { Datetime = g.Key.ToString(), game_score = g.Average(s => s.game_score) }).Reverse().Take(7).Reverse().ToList();

                foreach (var item in listaGames)
                {
                    data.periodos.Add(item.Datetime);
                    data.valoresPuntaje.Add((int)item.game_score);
                    data.valoresMeta.Add((int)limites.limiteCalculo);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }






    }
}
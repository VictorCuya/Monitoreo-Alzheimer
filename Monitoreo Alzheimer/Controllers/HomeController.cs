using Dominio.Entity;
using Dominio.Repository;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace Monitoreo_Alzheimer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.medico_collection = MongoHelper.database.GetCollection<Medico>("Medicos");

                var builder = Builders<Medico>.Filter;
                var filter1 =  builder.Eq("usuario", collection["usuario"]);
                var filter2 = builder.Eq("password", Seguridad.Encriptar(collection["password"]));
                var andFilter = filter1 & filter2;

                var resultLogin = MongoHelper.medico_collection.Find(andFilter).First();

                if (resultLogin != null) 
                {

                    FormsAuthentication.SetAuthCookie(collection["usuario"], true);                 
                    return RedirectToAction("Perfil", "Medico");
                   
                }
                else {

                    ViewBag.Indicador = 1;
                    return View();
                }    
                
            }
            catch (Exception)
            {
                ViewBag.Indicador = 1;
                return View();
            }
          
        }


        public ActionResult RegistrarMedico()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegistrarMedico(FormCollection collection)
        {
            try
            {
                MongoHelper.ConnectToMongoService();
                MongoHelper.medico_collection = MongoHelper.database.GetCollection<Medico>("Medicos");

                var builder = Builders<Medico>.Filter;
                var filter1 = builder.Eq("usuario", collection["usuario"]);
                var filter2 = builder.Eq("DNI", collection["DNI"]);
                var orFilter = filter1 | filter2;
                var resultUsuario = new Medico();

                try { resultUsuario = MongoHelper.medico_collection.Find(orFilter).First(); } catch { resultUsuario = null; }
                
                
                if (resultUsuario == null)
                {
                    //create some id
                    Object id = GenerateRandomId(24);
                    MongoHelper.medico_collection.InsertOneAsync(new Medico
                    {
                        _id = id,
                        nombre = collection["nombre"],
                        apellido = collection["apellido"],
                        correo = collection["correo"],
                        telefono = collection["telefono"],
                        DNI = collection["DNI"],
                        distrito = collection["distrito"],
                        EntidadLaboral = collection["EntidadLaboral"],
                        grado = collection["grado"],
                        usuario = collection["usuario"],
                        password = Seguridad.Encriptar(collection["password"])
                    });

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
        private static Random random = new Random();
        private object GenerateRandomId(int v)
        {
            string strarray = "abcdefghijklmnopqrstuvwxyz123456789";
            return new string(Enumerable.Repeat(strarray, v).Select(s => s[random.Next(s.Length)]).ToArray());
        }



    }
}
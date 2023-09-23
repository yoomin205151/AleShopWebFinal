using AleShopWeb2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AleShopWeb2.Controllers
{
    public class CuentaController : Controller
    {
        private ALESHOPWEBEntities9 db = new ALESHOPWEBEntities9();
        // GET: Cuenta
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(usuario model)
        {
            if (ModelState.IsValid)
            {
                // Verificar las credenciales del usuario en la base de datos
                var user = db.usuario.FirstOrDefault(u => u.email == model.email && u.contrasenia == model.contrasenia);

                if (user != null)
                {
                    // Autenticación exitosa, realizar el inicio de sesión
                    FormsAuthentication.SetAuthCookie(user.email, false); // Establecer la cookie de autenticación

                    Session["usuario"] = user;

                    return RedirectToAction("Index", "Home");
                }
            }

            // Si se llega a este punto, las credenciales son inválidas
            ModelState.AddModelError("", "El email o la contraseña no coinciden. Vuelva a intentarlo.");
            return View("Index");
        }
        public ActionResult Logout()
        {
            // Cerrar la sesión actual
            FormsAuthentication.SignOut();
            Session["usuario"] = null;

            // Redirigir al inicio o a la página de inicio de sesión
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RegistroVista()
        {
            return View("~/Views/Cuenta/Registro.cshtml");
        }
        [HttpPost]
        public ActionResult Registrar(usuario model)
        {
            if (ModelState.IsValid)
            {

                model.id_rol = 2;

                db.usuario.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View("Index", model);
        }

    }
}
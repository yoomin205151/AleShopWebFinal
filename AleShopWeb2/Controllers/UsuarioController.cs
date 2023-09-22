using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AleShopWeb2.Models;

namespace AleShopWeb2.Controllers
{
    public class UsuarioController : Controller
    {
        private ALESHOPWEBEntities9 db = new ALESHOPWEBEntities9();

        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Usuarios()
        {
            var model = new usuario();
            var roles = db.rol.ToList();
            ViewBag.Roles = roles;
            return View(model);
        }

        public JsonResult ListarUsuarios()
        {
            List<usuario> usuarios = db.usuario.ToList();

            var usuariosDTO = usuarios.Select(u => new
            {
                u.id,
                u.nombre,
                u.apellido,
                u.email,
                u.contrasenia,
                u.id_rol
            });

            return Json(new { data = usuariosDTO }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CrearUsuario(usuario nuevoUsuario)
        {
            if (ModelState.IsValid)
            {
                // Realizar lógica de creación de usuario, por ejemplo, guardar en la base de datos
                db.usuario.Add(nuevoUsuario);
                db.SaveChanges();

                // Opcional: Redirigir a una página de éxito o mostrar un mensaje de éxito en la vista
                TempData["SuccessMessage"] = "Usuario creado exitosamente.";

                // Redirigir a una página de éxito o regresar a la vista original
                return RedirectToAction("Usuarios");
            }

            // Si llegamos a este punto, significa que ha ocurrido un error en la validación
            // Vuelve a cargar la vista con los mensajes de error
            var roles = db.rol.ToList();
            ViewBag.Roles = roles;
            return View("Usuarios");
        }

        [HttpPost]
        public ActionResult ModificarUsuario(usuario usuarioModificado)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = db.usuario.Find(usuarioModificado.id);
                if (usuarioExistente != null)
                {
                    // Actualizar las propiedades del usuario existente con los valores modificados
                    usuarioExistente.nombre = usuarioModificado.nombre;
                    usuarioExistente.apellido = usuarioModificado.apellido;
                    usuarioExistente.email = usuarioModificado.email;
                    usuarioExistente.contrasenia = usuarioModificado.contrasenia;
                    usuarioExistente.id_rol = usuarioModificado.id_rol;

                    // Guardar los cambios en la base de datos
                    db.SaveChanges();

                    // Opcional: Redirigir a una página de éxito o mostrar un mensaje de éxito en la vista
                    TempData["SuccessMessage"] = "Usuario modificado exitosamente.";
                }
                else
                {
                    // Si el usuario no existe, puedes mostrar un mensaje de error o redirigir a una página de error
                    TempData["ErrorMessage"] = "El usuario no existe.";
                }

                // Redirigir a una página de éxito o regresar a la vista original
                return RedirectToAction("Usuarios");
            }

            // Si llegamos a este punto, significa que ha ocurrido un error en la validación
            // Vuelve a cargar la vista con los mensajes de error
            var roles = db.rol.ToList();
            ViewBag.Roles = roles;
            return View("Usuarios", usuarioModificado);
        }

        [HttpPost]
        public ActionResult EliminarUsuario(int id)
        {
            try
            {
                var usuarioExistente = db.usuario.Find(id);
                if (usuarioExistente != null)
                {
                    db.usuario.Remove(usuarioExistente);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Usuario eliminado exitosamente.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["ErrorMessage"] = "El usuario no existe.";
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el usuario. Detalles: " + ex.Message;
                return Json(new { success = false });
            }
        }

    }
}

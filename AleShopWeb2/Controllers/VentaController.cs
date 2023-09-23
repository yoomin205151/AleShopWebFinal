using AleShopWeb2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using System.Threading.Tasks;
using MercadoPago.Resource.Preference;

namespace AleShopWeb2.Controllers
{
    public class VentaController : Controller
    {
        private ALESHOPWEBEntities9 db = new ALESHOPWEBEntities9();
        // GET: Venta
        public ActionResult Venta()
        {
            return View();
        }

        public ActionResult ListarVentasUsuario()
        {
            // Obtener el ID del usuario actual desde la sesión
            if (Session["usuario"] != null && Session["usuario"] is usuario user)
            {
                int idUsuario = user.id;

                // Consultar las ventas donde id_origen sea igual a 1 y id_usuario sea igual al ID del usuario actual,
                // ordenando los resultados por la fecha en orden descendente.
                var ventas = db.venta
                    .Where(v => v.id_origen == 1 && v.id_usuario == idUsuario)
                    .OrderByDescending(v => v.fecha)
                    .ToList();

                return View(ventas);
            }

            // Si el usuario no ha iniciado sesión o la sesión ha expirado, redirigir a la página de inicio de sesión.
            return RedirectToAction("Login", "Cuenta");
        }

        public ActionResult DetalleVenta(int id)
        {
            // Obtener la venta correspondiente al ID proporcionado
            var venta = db.venta.Find(id);

            if (venta == null)
            {
                // Si la venta no existe, redireccionar a una página de error o mostrar un mensaje
                return RedirectToAction("Index", "Home");
            }

            // Obtener los detalles de venta asociados a la venta, incluyendo la categoría del producto
            var detallesVenta = db.detalle_venta
                .Include(d => d.producto.categoria) // Incluimos la categoría del producto
                .Where(d => d.id_venta == id)
                .ToList();

            // Crear una lista de modelos que contendrá el detalle de la venta y los productos asociados
            var detalleVentas = new List<DetalleVentaConProducto>();

            foreach (var detalleVenta in detallesVenta)
            {
                // Crear un modelo que contenga el detalle de venta y el producto asociado
                var detalleVentaConProducto = new DetalleVentaConProducto
                {
                    DetalleVenta = detalleVenta,
                    Producto = detalleVenta.producto
                };

                detalleVentas.Add(detalleVentaConProducto);
            }

            return View(detalleVentas);
        }

        

        

       


    }
}
using AleShopWeb2.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace AleShopWeb2.Controllers
{
    public class ProductoController : Controller
    {
        private ALESHOPWEBEntities9 db = new ALESHOPWEBEntities9();
        // GET: Producto
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Producto(int? categoriaId, string nombreProducto, int? page)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var productos = db.producto.Where(p => p.activo == "activo");

            // Filtrar por categoría si se proporciona una categoríaId válida
            if (categoriaId.HasValue)
            {
                productos = productos.Where(p => p.id_categoria == categoriaId);
            }

            // Filtrar por nombre del producto si se proporciona un nombreProducto válido
            if (!string.IsNullOrEmpty(nombreProducto))
            {
                productos = productos.Where(p => p.nombre.Contains(nombreProducto));
            }

            // Ordenar los productos por algún criterio, por ejemplo, por su ID.
            // Esto es necesario para usar el método Skip en LINQ to Entities.
            productos = productos.OrderBy(p => p.id);

            // Obtener el número total de productos y calcular el número total de páginas
            int totalProductos = productos.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalProductos / pageSize);

            // Obtener la lista final de productos filtrados con paginación
            var productosPaginados = productos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Establecer variables en ViewBag para utilizar en la vista
            ViewBag.CategoriaId = categoriaId;
            ViewBag.NombreProducto = nombreProducto;
            ViewBag.PaginaActual = pageNumber;
            ViewBag.TotalPaginas = totalPaginas;

            return View(productosPaginados);
        }





        public ActionResult ABMProducto()
        {
            var model = new producto();
            var categorias = db.categoria.ToList();
            ViewBag.Categorias = categorias;
            return View(model);
        }

        public JsonResult ListarProducto(int categoriaId=0)
        {
            List<producto> productos;

            if (categoriaId > 0)
            {
                // Filtrar los productos por categoría
                productos = db.producto.Where(p => p.id_categoria == categoriaId).ToList();
            }
            else
            {
                // Obtener todos los productos
                productos = db.producto.ToList();
            }

            // Crear el objeto JSON con los datos de los productos
            var productosDTO = productos.Select(p => new
            {
                p.id,
                p.img, 
                p.nombre,
                p.stock,
                categoria = p.categoria.nombre,
                p.precio,
                p.activo
            });

            return Json(new { data = productosDTO }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarProductoActivo(int categoriaId = 0)
        {
            List<producto> productos;

            if (categoriaId > 0)
            {
                // Filtrar los productos por categoría y por estado activo
                productos = db.producto.Where(p => p.id_categoria == categoriaId && p.activo == "activo").ToList();
            }
            else
            {
                // Obtener todos los productos con estado activo
                productos = db.producto.Where(p => p.activo == "activo").ToList();
            }

            // Crear el objeto JSON con los datos de los productos
            var productosDTO = productos.Select(p => new
            {
                p.id,
                p.img,
                p.nombre,
                p.stock,
                categoria = p.categoria.nombre,
                p.precio,
                p.activo
            });

            return Json(new { data = productosDTO }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarCategoria()
        {
            List<categoria> categorias = db.categoria.ToList();

            var categoriasDTO = categorias.Select(c => new
            {

                c.id, 
                c.nombre
               
            });

            return Json(new { data = categoriasDTO }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CrearProducto(producto nuevoProducto, HttpPostedFileBase ImagenFile)
        {
            if (ModelState.IsValid)
            {
                // Establecer el valor predeterminado del campo "activo"
                nuevoProducto.activo = "activo";

                // Guardar la imagen como base64 en la propiedad img
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile.InputStream))
                    {
                        var imageData = binaryReader.ReadBytes(ImagenFile.ContentLength);
                        nuevoProducto.img = Convert.ToBase64String(imageData);
                    }
                }

                // Realizar lógica de creación de producto, por ejemplo, guardar en la base de datos
                db.producto.Add(nuevoProducto);
                db.SaveChanges();

                // Opcional: Redirigir a una página de éxito o mostrar un mensaje de éxito en la vista
                TempData["SuccessMessage"] = "Producto creado exitosamente.";

                // Redirigir a una página de éxito o regresar a la vista original
                return RedirectToAction("ABMProducto");
            }

            // Si llegamos a este punto, significa que ha ocurrido un error en la validación
            // Vuelve a cargar la vista con los mensajes de error
            var categorias = db.categoria.ToList();
            ViewBag.Categorias = categorias;
            return View("ABMProducto");
        }


        [HttpPost]
        public ActionResult CambiarEstadoProducto(int id, string nuevoEstado)
        {
            var producto = db.producto.Find(id);
            if (producto != null)
            {
                producto.activo = nuevoEstado;
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "El producto no existe." });
            }
        }

        [HttpPost]
        public ActionResult ModificarProducto(producto productoModificado, HttpPostedFileBase ImagenFile)
        {
            if (ModelState.IsValid)
            {
                var productoExistente = db.producto.Find(productoModificado.id);
                if (productoExistente != null)
                {
                    // Actualizar las propiedades del producto existente con los valores modificados
                    productoExistente.nombre = productoModificado.nombre;
                    productoExistente.stock = productoModificado.stock;
                    productoExistente.precio = productoModificado.precio;
                    productoExistente.id_categoria = productoModificado.id_categoria;

                    Console.WriteLine("Producto Modificado:");
                    Console.WriteLine(productoModificado);
                    // Guardar la nueva imagen en base64 si se proporciona
                    if (ImagenFile != null && ImagenFile.ContentLength > 0)
                    {
                        using (var binaryReader = new BinaryReader(ImagenFile.InputStream))
                        {
                            var imageData = binaryReader.ReadBytes(ImagenFile.ContentLength);
                            productoExistente.img = Convert.ToBase64String(imageData);
                        }
                    }
                    Console.WriteLine("Producto Modificado:");
                    Console.WriteLine(productoExistente);
                    // Guardar los cambios en la base de datos
                    db.SaveChanges();

                    // Opcional: Redirigir a una página de éxito o mostrar un mensaje de éxito en la vista
                    TempData["SuccessMessage"] = "Producto modificado exitosamente.";
                }
                else
                {
                    // Si el producto no existe, puedes mostrar un mensaje de error o redirigir a una página de error
                    TempData["ErrorMessage"] = "El producto no existe.";
                }

                // Redirigir a una página de éxito o regresar a la vista original
                return RedirectToAction("ABMProducto");
            }

            // Si llegamos a este punto, significa que ha ocurrido un error en la validación
            // Vuelve a cargar la vista con los mensajes de error
            var categorias = db.categoria.ToList();
            ViewBag.Categorias = categorias;
            return View("ABMProducto", productoModificado);
        }


        [HttpPost]
        public ActionResult EliminarProducto(int id)
        {
            try
            {
                var productoExistente = db.producto.Find(id);
                if (productoExistente != null)
                {
                    db.producto.Remove(productoExistente);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
                    return Json(new { success = true });
                }
                else
                {
                    TempData["ErrorMessage"] = "El producto no existe.";
                    return Json(new { success = false });
                }
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                string errorMessage = innerException?.Message;

                
                Console.WriteLine(errorMessage);

                TempData["ErrorMessage"] = "Ha ocurrido un error al eliminar el producto.";
                return Json(new { success = false });
            }
        }
       







    }
}
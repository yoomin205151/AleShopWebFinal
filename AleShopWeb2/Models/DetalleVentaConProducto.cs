using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AleShopWeb2.Models
{
    public class DetalleVentaConProducto
    {
        public detalle_venta DetalleVenta { get; set; }
        public producto Producto { get; set; }
    }
}
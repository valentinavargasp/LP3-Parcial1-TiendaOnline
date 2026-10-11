using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using parcial1.Models;

namespace parcial1.Pages
{
    /// <summary>
    /// Formulario de Baja de Productos 
    /// </summary>
    public class BajaProductoModel : PageModel
    {
        private readonly IConfiguration _config;

        public BajaProductoModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public int? IdProducto { get; set; }

        [BindProperty]
        public string? Accion { get; set; }

        public ProductoItem? ProductoSeleccionado { get; set; }

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }

        public void OnGet(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                IdProducto = id.Value;
                BuscarProducto(id.Value);
            }
        }

        public IActionResult OnPost()
        {
            if (string.Equals(Accion, "Buscar", StringComparison.OrdinalIgnoreCase))
            {
                if (!IdProducto.HasValue || IdProducto.Value <= 0)
                {
                    MensajeError = "Ingrese un código de producto válido.";
                    return Page();
                }

                BuscarProducto(IdProducto.Value);
                return Page();
            }

            if (string.Equals(Accion, "Eliminar", StringComparison.OrdinalIgnoreCase))
            {
                if (!IdProducto.HasValue || IdProducto.Value <= 0)
                {
                    MensajeError = "Debe indicar el código del producto a eliminar.";
                    return Page();
                }

                BuscarProducto(IdProducto.Value);
                string nombreBorrado = ProductoSeleccionado?.Nombre ?? $"ID #{IdProducto.Value}";

                string conexionString = _config.GetConnectionString("tienda_db")!;

                try
                {
                    using (SqlConnection conexion = new SqlConnection(conexionString))
                    {
                        conexion.Open();

                        // CONSIGNAS DEL PARCIAL:
                        // "Eliminar solo registros seleccionados de la tabla productos"
                        // "Mantener referencias de categorías intactas (no afectar tabla categorías)"
                        string sql = "DELETE FROM productos WHERE idProducto = @idProducto";

                        using (SqlCommand comando = new SqlCommand(sql, conexion))
                        {
                            comando.Parameters.AddWithValue("@idProducto", IdProducto.Value);

                            int filasBorradas = comando.ExecuteNonQuery();

                            if (filasBorradas > 0)
                            {
                                MensajeExito = $"Se eliminó exitosamente el producto '{nombreBorrado}'. Las categorías del sistema permanecen intactas.";
                                ProductoSeleccionado = null;
                                IdProducto = null;
                            }
                            else
                            {
                                MensajeError = $"No se encontró ningún producto con el código #{IdProducto}.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MensajeError = $"Error al eliminar el producto: {ex.Message}";
                }
            }

            return Page();
        }

        private void BuscarProducto(int id)
        {
            string conexionString = _config.GetConnectionString("tienda_db")!;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = @"
                    SELECT 
                        p.idProducto,
                        p.nombre,
                        p.precio,
                        p.idCategoria,
                        c.descripcion AS categoria
                    FROM productos p
                    INNER JOIN categorias c ON p.idCategoria = c.idCategoria
                    WHERE p.idProducto = @idProducto";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@idProducto", id);

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            ProductoSeleccionado = new ProductoItem
                            {
                                IdProducto = Convert.ToInt32(lector["idProducto"]),
                                Nombre = lector["nombre"].ToString() ?? string.Empty,
                                Precio = Convert.ToDecimal(lector["precio"]),
                                IdCategoria = Convert.ToInt32(lector["idCategoria"]),
                                Categoria = lector["categoria"].ToString() ?? string.Empty
                            };
                        }
                        else
                        {
                            MensajeError = $"No se encontró ningún producto con el código #{id}.";
                            ProductoSeleccionado = null;
                        }
                    }
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace parcial1.Pages
{
    /// <summary>
    /// Formulario de Modificación de Productos 
    /// Permite editar nombre, precio y categoría asignada.
    /// Valida obligatoriamente que al menos un campo se haya modificado antes de impactar el UPDATE.
    /// </summary>
    public class ModificacionProductoModel : PageModel
    {
        private readonly IConfiguration _config;

        public ModificacionProductoModel(IConfiguration config)
        {
            _config = config;
        }

        // Código identificador del producto
        [BindProperty]
        public int? IdProducto { get; set; }

        // Campos editables
        [BindProperty]
        public string? Nombre { get; set; }

        [BindProperty]
        public decimal? Precio { get; set; }

        [BindProperty]
        public int IdCategoria { get; set; }

        // Campos de resguardo con los valores previos
        // Utilizados para comprobar si el usuario realmente alteró algún campo
        [BindProperty]
        public string? OriginalNombre { get; set; }

        [BindProperty]
        public decimal? OriginalPrecio { get; set; }

        [BindProperty]
        public int OriginalIdCategoria { get; set; }

        [BindProperty]
        public string? Accion { get; set; }

        public bool ProductoCargado { get; set; }

        public List<SelectListItem> Categorias { get; set; } = new();

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }
        public string? MensajeAdvertencia { get; set; }

        public void OnGet(int? id)
        {
            CargarCategorias();

            if (id.HasValue && id.Value > 0)
            {
                IdProducto = id.Value;
                BuscarProductoPorId(id.Value);
            }
        }

        public IActionResult OnPost()
        {
            CargarCategorias();

            // Flujo 1: El usuario busca el producto por su ID
            if (string.Equals(Accion, "Buscar", StringComparison.OrdinalIgnoreCase))
            {
                if (!IdProducto.HasValue || IdProducto.Value <= 0)
                {
                    MensajeError = "Debe ingresar un código de producto válido para buscar.";
                    return Page();
                }

                BuscarProductoPorId(IdProducto.Value);
                return Page();
            }

            // Flujo 2: El usuario confirma los cambios (Modificar)
            if (string.Equals(Accion, "Modificar", StringComparison.OrdinalIgnoreCase))
            {
                if (!IdProducto.HasValue || IdProducto.Value <= 0)
                {
                    MensajeError = "No se ha seleccionado ningún producto para modificar.";
                    return Page();
                }

                // Validaciones de formato
                if (string.IsNullOrWhiteSpace(Nombre))
                {
                    MensajeError = "El nombre del producto no puede estar vacío.";
                    ProductoCargado = true;
                    return Page();
                }

                if (!Precio.HasValue || Precio.Value <= 0)
                {
                    MensajeError = "El precio debe ser un número válido mayor a cero.";
                    ProductoCargado = true;
                    return Page();
                }

                if (IdCategoria <= 0)
                {
                    MensajeError = "Debe asignar una categoría válida.";
                    ProductoCargado = true;
                    return Page();
                }

                // =========================================================================
                //Validar que al menos un campo se haya modificado antes de guardar
                // =========================================================================
                bool nombreModificado = !string.Equals(Nombre.Trim(), (OriginalNombre ?? string.Empty).Trim(), StringComparison.Ordinal);
                bool precioModificado = (Precio.Value != OriginalPrecio);
                bool categoriaModificada = (IdCategoria != OriginalIdCategoria);

                if (!nombreModificado && !precioModificado && !categoriaModificada)
                {
                    MensajeAdvertencia = "No se detectaron modificaciones. Debe cambiar al menos un campo (Nombre, Precio o Categoría) antes de guardar.";
                    ProductoCargado = true;
                    return Page();
                }

                // Ejecución del UPDATE en SQL Server
                string conexionString = _config.GetConnectionString("tienda_db")!;

                try
                {
                    using (SqlConnection conexion = new SqlConnection(conexionString))
                    {
                        conexion.Open();

                        string sql = @"
                            UPDATE productos 
                            SET nombre = @nombre,
                                precio = @precio,
                                idCategoria = @idCategoria
                            WHERE idProducto = @idProducto";

                        using (SqlCommand comando = new SqlCommand(sql, conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
                            comando.Parameters.AddWithValue("@precio", Precio.Value);
                            comando.Parameters.AddWithValue("@idCategoria", IdCategoria);
                            comando.Parameters.AddWithValue("@idProducto", IdProducto.Value);

                            int filasAfectadas = comando.ExecuteNonQuery();

                            if (filasAfectadas > 0)
                            {
                                MensajeExito = $"Los datos del producto #{IdProducto} se actualizaron correctamente.";

                                // Sincronizar los valores originales tras la modificación exitosa
                                OriginalNombre = Nombre;
                                OriginalPrecio = Precio;
                                OriginalIdCategoria = IdCategoria;
                                ProductoCargado = true;
                            }
                            else
                            {
                                MensajeError = "No se encontró el producto en la base de datos para actualizar.";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MensajeError = $"Error al modificar el producto: {ex.Message}";
                    ProductoCargado = true;
                }
            }

            return Page();
        }

        private void BuscarProductoPorId(int id)
        {
            string conexionString = _config.GetConnectionString("tienda_db")!;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                string sql = "SELECT idProducto, nombre, precio, idCategoria FROM productos WHERE idProducto = @idProducto";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    comando.Parameters.AddWithValue("@idProducto", id);

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            Nombre = lector["nombre"].ToString();
                            Precio = Convert.ToDecimal(lector["precio"]);
                            IdCategoria = Convert.ToInt32(lector["idCategoria"]);

                            // Guardamos los valores originales para comparar modificaciones
                            OriginalNombre = Nombre;
                            OriginalPrecio = Precio;
                            OriginalIdCategoria = IdCategoria;

                            ProductoCargado = true;
                        }
                        else
                        {
                            MensajeError = $"No se encontró ningún producto con el código #{id}.";
                            ProductoCargado = false;
                        }
                    }
                }
            }
        }

        private void CargarCategorias()
        {
            Categorias.Clear();
            string conexionString = _config.GetConnectionString("tienda_db")!;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                string sql = "SELECT idCategoria, descripcion FROM categorias ORDER BY descripcion ASC";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Categorias.Add(new SelectListItem(
                            lector["descripcion"].ToString(),
                            lector["idCategoria"].ToString()
                        ));
                    }
                }
            }
        }
    }
}

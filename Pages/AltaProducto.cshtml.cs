using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace parcial1.Pages
{
    /// <summary>
    /// Formulario de Alta de Productos 
    /// Maneja la recepción de datos desde controles de formulario (TextBox y DropDownList)
    /// y la inserción parametrizada en la base de datos SQL Server.
    /// </summary>
    public class AltaProductoModel : PageModel
    {
        private readonly IConfiguration _config;

        public AltaProductoModel(IConfiguration config)
        {
            _config = config;
        }

        // Propiedades vinculadas al formulario web mediante data-binding
        [BindProperty]
        public string? Nombre { get; set; }

        [BindProperty]
        public decimal? Precio { get; set; }

        [BindProperty]
        public int IdCategoria { get; set; }

        // Lista para poblar el control de selección DropDownList desde la tabla categorias
        public List<SelectListItem> Categorias { get; set; } = new();

        public string? MensajeExito { get; set; }
        public string? MensajeError { get; set; }

        public void OnGet()
        {
            // Se ejecuta al cargar la página por primera vez (equivalente a Page_Load)
            CargarCategorias();
        }

        public IActionResult OnPost()
        {
            // En Razor Pages el control DropDownList se debe repoblar en cada render
            CargarCategorias();

            // Validaciones de integridad y reglas de negocio
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MensajeError = "Debe ingresar el nombre del producto.";
                return Page();
            }

            if (!Precio.HasValue || Precio.Value <= 0)
            {
                MensajeError = "El precio debe ser un número mayor a cero.";
                return Page();
            }

            if (IdCategoria <= 0)
            {
                MensajeError = "Debe seleccionar una categoría para el producto.";
                return Page();
            }

            string conexionString = _config.GetConnectionString("tienda_db")!;

            try
            {
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();

                    // Consulta SQL parametrizada para evitar inyección SQL
                    string sql = "INSERT INTO productos (nombre, precio, idCategoria) VALUES (@nombre, @precio, @idCategoria)";

                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre", Nombre.Trim());
                        comando.Parameters.AddWithValue("@precio", Precio.Value);
                        comando.Parameters.AddWithValue("@idCategoria", IdCategoria);

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MensajeExito = $"El producto '{Nombre}' fue registrado con éxito.";

                            // Limpiar los controles tras guardar
                            Nombre = string.Empty;
                            Precio = null;
                            IdCategoria = 0;
                        }
                        else
                        {
                            MensajeError = "No se pudo registrar el producto en la base de datos.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al conectar con la base de datos: {ex.Message}";
            }

            return Page();
        }

        /// <summary>
        /// Consulta la tabla 'categorias' y genera los elementos del DropDownList
        /// </summary>
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

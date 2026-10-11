using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace parcial1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public int CantidadProductos { get; set; }
        public int CantidadCategorias { get; set; }
        public bool BaseConectada { get; set; }
        public string? ErrorConexion { get; set; }

        public void OnGet()
        {
            ConsultarTotales();
        }

        private void ConsultarTotales()
        {
            string conexionString = _config.GetConnectionString("tienda_db")!;

            try
            {
                using (SqlConnection conexion = new SqlConnection(conexionString))
                {
                    conexion.Open();
                    BaseConectada = true;

                    // Conteo de productos
                    using (SqlCommand cmdProd = new SqlCommand("SELECT COUNT(*) FROM productos", conexion))
                    {
                        CantidadProductos = Convert.ToInt32(cmdProd.ExecuteScalar());
                    }

                    // Conteo de categorías
                    using (SqlCommand cmdCat = new SqlCommand("SELECT COUNT(*) FROM categorias", conexion))
                    {
                        CantidadCategorias = Convert.ToInt32(cmdCat.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                BaseConectada = false;
                ErrorConexion = ex.Message;
            }
        }
    }
}

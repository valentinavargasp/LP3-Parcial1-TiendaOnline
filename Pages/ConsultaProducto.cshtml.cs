using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using parcial1.Models;

namespace parcial1.Pages
{
    public class ConsultaProductoModel : PageModel
    {
        private readonly IConfiguration _config;

        public ConsultaProductoModel(IConfiguration config)
        {
            _config = config;
        }

        // Criterio de búsqueda ingresado por el usuario (TextBox)
        [BindProperty(SupportsGet = true)]
        public string? CriterioBusqueda { get; set; }

        // Filtro por categoría opcional
        [BindProperty(SupportsGet = true)]
        public int? FiltroCategoria { get; set; }

        // Lista de productos con su categoría resultante del JOIN
        public List<ProductoItem> ListaProductos { get; set; } = new();

        public int TotalRegistros => ListaProductos.Count;
        public decimal TotalValorInventario => ListaProductos.Sum(p => p.Precio);

        public void OnGet()
        {
            EjecutarConsultaJoin();
        }

        public void OnPost()
        {
            EjecutarConsultaJoin();
        }

        private void EjecutarConsultaJoin()
        {
            ListaProductos.Clear();
            string conexionString = _config.GetConnectionString("tienda_db")!;

            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();

                // Consulta relacional con JOIN explícito entre productos y categorias
                // (Equivalente al SelectQuery con JOIN)
                string sql = @"
                    SELECT 
                        p.idProducto,
                        p.nombre,
                        p.precio,
                        p.idCategoria,
                        c.descripcion AS categoria
                    FROM productos p
                    INNER JOIN categorias c ON p.idCategoria = c.idCategoria
                    WHERE 1 = 1";

                // Si el usuario ingresó un criterio de búsqueda (ID numérico o fragmento de nombre)
                bool esNumero = int.TryParse(CriterioBusqueda, out int idBuscado);

                if (!string.IsNullOrWhiteSpace(CriterioBusqueda))
                {
                    if (esNumero)
                    {
                        sql += " AND (p.idProducto = @idBuscado OR p.nombre LIKE @patron)";
                    }
                    else
                    {
                        sql += " AND p.nombre LIKE @patron";
                    }
                }

                sql += " ORDER BY p.idProducto ASC";

                using (SqlCommand comando = new SqlCommand(sql, conexion))
                {
                    if (!string.IsNullOrWhiteSpace(CriterioBusqueda))
                    {
                        if (esNumero)
                        {
                            comando.Parameters.AddWithValue("@idBuscado", idBuscado);
                        }
                        comando.Parameters.AddWithValue("@patron", "%" + CriterioBusqueda.Trim() + "%");
                    }

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            ListaProductos.Add(new ProductoItem
                            {
                                IdProducto = Convert.ToInt32(lector["idProducto"]),
                                Nombre = lector["nombre"].ToString() ?? string.Empty,
                                Precio = Convert.ToDecimal(lector["precio"]),
                                IdCategoria = Convert.ToInt32(lector["idCategoria"]),
                                Categoria = lector["categoria"].ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
        }
    }
}

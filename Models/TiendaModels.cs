namespace parcial1.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class ProductoItem
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; } = string.Empty;
    }
}

using System.Text.Json.Serialization;

namespace PAGO.API.Models
{
    public class PostDataInput
    {
        public int Remitente { get; set; }
        public int Destinatario { get; set; }
        public ICollection<Productos> Productos { get; set; }
        public int TotalProductos { get; set; }
        public decimal MontoTotal { get; set; }
        public string Concepto { get; set; }
    }
    public class Productos
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        [JsonIgnore]
        public decimal Precio { get; set; }
    }
}

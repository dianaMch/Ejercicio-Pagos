namespace PAGO.API.Entities
{
    public class PAGOS
    {
        public int IDPAGO { get; set; }
        public int IDREMITENTE { get; set; }
        public int IDDESTINATARIO { get; set; }
        public int PRODUCTOSTOTAL { get; set; }
        public decimal MONTOTOTAL { get; set; }
        public string CONCEPTO { get; set; }
        public string ESTADO { get; set; } = "Pendiente";
        public DateTime AUFECHA { get; set; } = DateTime.UtcNow;
        public string AUUSUARIO { get; set; }
    }
}

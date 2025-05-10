namespace PAGO.API.Entities
{
    public class PAGOPRODUCTOS
    {
        public int IDPAGOPRODUCTO { get; set; }
        public int IDPAGO { get; set; }
        public int IDPRODUCTO { get; set; }
        public int CANTIDAD { get; set; }
        public decimal PRECIOUNIDAD { get; set; }
    }
}

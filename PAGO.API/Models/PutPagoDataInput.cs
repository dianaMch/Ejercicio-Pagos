using System.ComponentModel.DataAnnotations;

namespace PAGO.API.Models
{
    public class PutPagoDataInput
    {
        [Required]
        public int IdPago { get; set; }
        [Required]
        public string Estado { get; set; }
    }
}

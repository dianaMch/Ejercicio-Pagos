using PAGO.WEBSERVICE.WCF.Data;
using PAGO.WEBSERVICE.WCF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace PAGO.WEBSERVICE.WCF
{
    public class PagoService : IPagoService
    {

        public PagosDataOutput ConsultaEstatus(int IdPago)
        {
            string token = HttpContext.Current.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
                throw new UnauthorizedAccessException("Token faltante o inválido.");

            token = token.Substring("Bearer ".Length);

            try
            {
                var principal = JwtValidator.ValidateToken(token);
                string usuario = principal.Identity.Name;

                try
                {
                    using (var db = new PagosBDContext())
                    {
                        var pago = db.PAGOS.Find(IdPago);
                        if (pago == null)
                            return new PagosDataOutput { IdPago = 0, Estado = "", Error = $"ERR_001: No se encontró ningun pago con el id {IdPago}." };

                        return new PagosDataOutput { IdPago = pago.IDPAGO, Estado = pago.ESTADO, Error = "" };
                    }
                }
                catch (Exception ex)
                {
                    return new PagosDataOutput { IdPago = 0, Estado = "", Error = $"ERR_005: " + ex.Message };
                }
            }
            catch
            {
                return new PagosDataOutput { IdPago = 0, Estado = "", Error = "ERR_002: Token inválido." };
            }
        }
    }
}

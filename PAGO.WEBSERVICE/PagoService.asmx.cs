using PAGO.WEBSERVICE.Data;
using PAGO.WEBSERVICE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PAGO.WEBSERVICE
{
    /// <summary>
    /// Descripción breve de PagoService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class PagoService : System.Web.Services.WebService
    {

        [WebMethod]
        public PagosDataOutput ActualizaEstadoPago(int IdPago, string Estado)
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
                            return new PagosDataOutput { IdPago = 0, Descripcion = "", Error = $"ERR_001: No se encontró ningun pago con el id {IdPago}." };

                        pago.ESTADO = Estado;
                        db.SaveChanges();

                        return new PagosDataOutput { IdPago = pago.IDPAGO, Descripcion = "Actualizado correctamente.", Error = "" };
                    }
                }
                catch (Exception ex)
                {
                    return new PagosDataOutput { IdPago = 0, Descripcion = "", Error = $"ERR_005: {ex.Message}" };
                }
            }
            catch
            {
                return new PagosDataOutput { IdPago = 0, Descripcion = "", Error = "ERR_002: Token inválido." };
            }
        }
    }
}

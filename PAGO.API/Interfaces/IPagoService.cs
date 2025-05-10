using Microsoft.AspNetCore.Mvc;
using PAGO.API.Models;
using PAGO.API.Models.ConsultaEstado;

namespace PAGO.API.Interfaces
{
    public interface IPagoService
    {
        //bool GeneraPagoAsync(PutPagoDataInput putPagoDataInput);
        //bool ConsultaPagosAsync(int id);

        Task GeneraPagoAsync(PostDataInput postDataInput, string token, string identityName);
        Task<ConsultaEstadoDataOutput> ConsultaEstadoAsync(int IdPago, string token);
        Task ActualizaEstadoAsync(PutPagoDataInput putPagoDataInput, string token);
    }
}

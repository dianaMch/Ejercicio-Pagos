using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PAGO.API.Data;
using PAGO.API.Entities;
using PAGO.API.Interfaces;
using PAGO.API.Models;
using PAGO.API.Models.ConsultaEstado;
using PAGO.API.Services;
using System.Text;

namespace PAGO.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/pagos")]
    public class PagoController : ControllerBase
    {
        private readonly ILogger<PagoController> _logger;
        private IPagoService _pagoService;

        public PagoController(ILogger<PagoController> logger, IPagoService pagoService)
        {
            _logger = logger;
            _pagoService = pagoService;
        }

        [HttpPost]
        public async Task<IActionResult> GeneraPago([FromBody] PostDataInput postDataInput)
        {
            try
            {
                _logger.LogInformation("Inicia el proceso de Generacion de Pago");
                await _pagoService.GeneraPagoAsync(postDataInput, Request.Headers["Authorization"].ToString(), User.Identity?.Name);
                _logger.LogInformation("Finaliza el proceso de Generacion de Pago");
                return Ok("Pago generado exitosamente!");
                
            }
            catch(Exception e )
            {
                _logger.LogError(e.Message);
                return Problem(detail: e.Message, statusCode: 500, title: "ErrorContext interno del sistema");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultaEstado(int id)
        {
            try
            {
                _logger.LogInformation("Inicia el proceso de Consulta de estado de Pago");
                var consultaEstadoDataOutput = await _pagoService.ConsultaEstadoAsync(id, Request.Headers["Authorization"].ToString());
                _logger.LogInformation("Finaliza el proceso de Consulta de estado de Pago");
                if (consultaEstadoDataOutput.Estado != "")
                    return Ok(consultaEstadoDataOutput);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem(detail: e.Message, statusCode: 500, title: "ErrorContext interno del sistema");
            }

        }

        [HttpPut]
        public async Task<IActionResult> ActualizaEstado([FromBody] PutPagoDataInput putPagoDataInput)
        {
            try
            {
                _logger.LogInformation("Inicia el proceso de  Actualizacion de estado  de Pago");
                await _pagoService.ActualizaEstadoAsync(putPagoDataInput, Request.Headers["Authorization"].ToString());
                _logger.LogInformation("Finaliza el proceso de Actualizacion de estado de Pago");
                return Ok("Estado Actualizado!");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem(detail: e.Message, statusCode: 500, title: "ErrorContext interno del sistema");
            }

        }
    }
}

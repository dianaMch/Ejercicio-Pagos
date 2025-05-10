using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAGO.API.Data;
using PAGO.API.Interfaces;
using PAGO.API.Models;
using PAGO.API.Models.ConsultaEstado;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using PAGO.API.Entities;
using System.Text.Json;
using PAGO.API.Models.GeneraPago;
using PAGO.API.Controllers;
using Microsoft.Extensions.Logging;

namespace PAGO.API.Services
{
    public class PagoService : IPagoService
    {
        private readonly IConfiguration _config;
        private readonly PagosBDContext _context;
        private readonly ILogger<PagoController> _logger;

        public PagoService(ILogger<PagoController> logger, IConfiguration config, PagosBDContext context)
        {
            _config = config;
            _context = context;
            _logger = logger;
        }

        public async Task GeneraPagoAsync(PostDataInput postDataInput, string token, string identityName)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (postDataInput.Productos.Sum(p => p.Cantidad) != postDataInput.TotalProductos)
                    throw new Exception("El total de Productos no coincide, favor de revisar!");

                decimal nMontoTotal = 0;
                foreach (var producto in postDataInput.Productos)
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
                    var response = await httpClient.GetAsync($"{string.Format(_config["URLs:GeneraPago"], producto.IdProducto)}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var resultado = JsonSerializer.Deserialize<ConsultaProductoResponse>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        producto.Precio = resultado.Precio;
                        nMontoTotal += (resultado.Precio * producto.Cantidad);
                    }
                    else
                    {
                        //throw new Exception("Id de producto no valido, favor de revisarlo!");
                        throw new Exception("Error al consulat producto REST");
                    }
                }

                if (nMontoTotal != postDataInput.MontoTotal)
                    throw new Exception("El monto total no coincide con el valor total de articulos, favor de revisar!");

                _logger.LogInformation("Entra a construccionpagos");
                var PAGOS = new PAGOS
                {
                    IDREMITENTE = postDataInput.Remitente,
                    IDDESTINATARIO = postDataInput.Destinatario,
                    PRODUCTOSTOTAL = postDataInput.TotalProductos,
                    CONCEPTO = postDataInput.Concepto,
                    MONTOTOTAL = postDataInput.MontoTotal,
                    AUUSUARIO = identityName
                };
                _context.PAGOS.Add(PAGOS);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Entra a productos");
                foreach (var producto in postDataInput.Productos)
                {
                    var PAGOPRODUCTOS = new PAGOPRODUCTOS
                    {
                        IDPAGO = PAGOS.IDPAGO,
                        IDPRODUCTO = producto.IdProducto,
                        CANTIDAD = producto.Cantidad,
                        PRECIOUNIDAD = producto.Precio * producto.Cantidad
                    };
                    _logger.LogInformation("PRECIOUNIDAD::::");
                    _logger.LogInformation(PAGOPRODUCTOS.PRECIOUNIDAD.ToString());
                    _context.PAGOPRODUCTOS.Add(PAGOPRODUCTOS);
                }
                await _context.SaveChangesAsync();

                _logger.LogInformation("antes de commits");
                await transaction.CommitAsync();
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<ConsultaEstadoDataOutput> ConsultaEstadoAsync(int IdPago, string token)
        {
            ConsultaEstadoDataOutput consultaEstadoDataOutput = new ConsultaEstadoDataOutput();
            try
            {
                var SOAP = string.Format(_config["SOAPs:ConsultaEstado:ActualizaEstadoPago"] ?? "", IdPago);

                var url = _config["URLs:ConsultaEstado"];

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
                    var content = new StringContent(SOAP, new UTF8Encoding(false), "text/xml");
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                    content.Headers.Add("SOAPAction", $"\"{_config["SOAPs:ConsultaEstado:SOAPAction"]}\"");

                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error en peticion SOAP: {response.StatusCode}");

                    var responseXml = await response.Content.ReadAsStringAsync();

                    var xmlDoc = XDocument.Parse(responseXml);

                    XNamespace soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
                    XNamespace tempuriNs = "http://tempuri.org/";
                    XNamespace modelNs = "http://schemas.datacontract.org/2004/07/PAGO.WEBSERVICE.WCF.Models";

                    var resultNode = xmlDoc
                        .Descendants(soapNs + "Body")
                        .Descendants(tempuriNs + $"{_config["SOAPs:ConsultaEstado:Response"]}")
                        .Descendants(tempuriNs + $"{_config["SOAPs:ConsultaEstado:Result"]}")
                        .FirstOrDefault();

                    if (resultNode == null)
                        throw new Exception();

                    if (!string.IsNullOrWhiteSpace(resultNode.Element(tempuriNs + "Error")?.Value))
                        throw new Exception(resultNode.Element(tempuriNs + "Error")?.Value);

                    consultaEstadoDataOutput.IdPago = resultNode.Element(modelNs + "IdPago")?.Value;
                    consultaEstadoDataOutput.Estado = resultNode.Element(modelNs + "Estado")?.Value;

                    return consultaEstadoDataOutput;
                }
            }
            catch (Exception ex)
            {
                return consultaEstadoDataOutput;
            }
        }


        public async Task ActualizaEstadoAsync(PutPagoDataInput putPagoDataInput, string token)
        {
            var SOAP = string.Format(_config["SOAPs:ActualizaEstado:ActualizaEstadoPago"] ?? "", putPagoDataInput.IdPago, putPagoDataInput.Estado);

            var url = _config["URLs:ActualizaEstado"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
                var content = new StringContent(SOAP, new UTF8Encoding(false), "text/xml");
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                content.Headers.Add("SOAPAction", $"\"{_config["SOAPs:ActualizaEstado:SOAPAction"]}\"");

                var response = await client.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error en peticion SOAP: {response.StatusCode}");

                var responseXml = await response.Content.ReadAsStringAsync();

                var xmlDoc = XDocument.Parse(responseXml);

                XNamespace soapNs = "http://schemas.xmlsoap.org/soap/envelope/";
                XNamespace tempuriNs = "http://tempuri.org/";

                var resultNode = xmlDoc
                    .Descendants(soapNs + "Body")
                    .Descendants(tempuriNs + $"{_config["SOAPs:ActualizaEstado:Response"]}")
                    .Descendants(tempuriNs + $"{_config["SOAPs:ActualizaEstado:Result"]}")
                    .FirstOrDefault();

                if (resultNode == null)
                    throw new Exception("Error");

                if (!string.IsNullOrWhiteSpace(resultNode.Element(tempuriNs + "Error")?.Value))
                    throw new Exception(resultNode.Element(tempuriNs + "Error")?.Value);
            }
        }
    }
}

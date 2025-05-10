using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PAGO.MICROSERVICE.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidationParameters"],
            ValidAudience = builder.Configuration["Jwt:ValidationParameters"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

//Configuración de BD
builder.Services.AddDbContext<PagosBDContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:80");
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapGet("/api/pagos/productos/{id}", async (int id, PagosBDContext db) =>
{
    var producto = await db.PRODUCTOS.FindAsync(id);

    if (producto == null)
        return Results.NotFound($"No existe el producto con ID {id}");

    return Results.Ok(new { IdProducto = producto.IDPRODUCTO, Descripcion = producto.DESCRIPCION, Precio = producto.PRECIO });
})
.RequireAuthorization()
.WithName("ConsultaProducto")
.WithOpenApi();

app.Run();


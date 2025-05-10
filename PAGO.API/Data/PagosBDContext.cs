using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PAGO.API.Entities;

namespace PAGO.API.Data
{
    public class PagosBDContext : DbContext
    {
        public PagosBDContext(DbContextOptions<PagosBDContext> options)
        : base(options) { }

        public DbSet<PAGOS> PAGOS { get; set; }
        public DbSet<PRODUCTOS> PRODUCTOS { get; set; }
        public DbSet<CLIENTES> CLIENTES { get; set; }
        public DbSet<PAGOPRODUCTOS> PAGOPRODUCTOS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PAGOS>(entity => { entity.HasKey(e => e.IDPAGO); entity.Property(e => e.IDPAGO).ValueGeneratedOnAdd();  entity.HasIndex(e => e.IDPAGO); entity.Property(e => e.MONTOTOTAL).HasPrecision(18, 2); });
            modelBuilder.Entity<PRODUCTOS>(entity => { entity.HasKey(e => e.IDPRODUCTO); entity.Property(e => e.IDPRODUCTO).ValueGeneratedOnAdd();  entity.HasIndex(e => e.IDPRODUCTO); entity.Property(e => e.PRECIO).HasPrecision(18, 2); });
            modelBuilder.Entity<CLIENTES>(entity => { entity.HasKey(e => e.IDCLIENTE); entity.Property(e => e.IDCLIENTE).ValueGeneratedOnAdd();  entity.HasIndex(e => e.IDCLIENTE); });
            modelBuilder.Entity<PAGOPRODUCTOS>(entity => { entity.HasKey(e => e.IDPAGOPRODUCTO); entity.Property(e => e.IDPAGOPRODUCTO).ValueGeneratedOnAdd();  entity.HasIndex(e => e.IDPAGOPRODUCTO); entity.Property(e => e.PRECIOUNIDAD).HasPrecision(18, 2); });
        }
    }
}

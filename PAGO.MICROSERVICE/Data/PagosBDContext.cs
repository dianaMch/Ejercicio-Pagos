
using Microsoft.EntityFrameworkCore;
using PAGO.MICROSERVICE.Entities;
using System.Collections.Generic;

namespace PAGO.MICROSERVICE.Data
{
    public class PagosBDContext : DbContext
    {
        public PagosBDContext(DbContextOptions<PagosBDContext> options)
        : base(options) { }

        public DbSet<PRODUCTOS> PRODUCTOS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PRODUCTOS>(entity => { entity.HasKey(e => e.IDPRODUCTO); entity.Property(e => e.IDPRODUCTO).ValueGeneratedOnAdd(); entity.HasIndex(e => e.IDPRODUCTO); entity.Property(e => e.PRECIO).HasPrecision(18, 2); });
        }
    }
}

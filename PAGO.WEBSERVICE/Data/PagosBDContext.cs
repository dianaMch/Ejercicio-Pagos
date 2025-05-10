using PAGO.WEBSERVICE.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PAGO.WEBSERVICE.Data
{
    public class PagosBDContext : DbContext
    {
        public PagosBDContext() : base($"name={ConfigurationManager.AppSettings["DB__Name"]}") { }

        public DbSet<PAGOS> PAGOS { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PAGOS>().HasKey(p => p.IDPAGO);
        }
    }
}
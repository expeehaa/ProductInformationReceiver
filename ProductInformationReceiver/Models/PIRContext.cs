using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProductInformationReceiver.Models
{
    public class PIRContext : DbContext
    {
        public DbSet<ProductInformationEntry> pir { get; set; }
        public DbSet<ProductInformation> pi { get; set; }
    }
}
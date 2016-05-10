using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductInformationReceiver.Models
{
    public class ProductInformation
    {
        public int ProductInformationId { get; set; }

        public string price { get; set; }

        public DateTime timestamp { get; set; }
    }
}
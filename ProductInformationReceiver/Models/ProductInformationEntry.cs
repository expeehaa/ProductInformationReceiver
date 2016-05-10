using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductInformationReceiver.Models
{
    public class ProductInformationEntry
    {
        public int ProductInformationEntryId { get; set; }
        public string productname { get; set; }
        public List<ProductInformation> productinformationlist { get; set; }
        public string provider { get; set; }
        public string providerUrl { get; set; }
        public string htmlSearchPattern { get; set; }
        public string currency { get; set; }
    }
}
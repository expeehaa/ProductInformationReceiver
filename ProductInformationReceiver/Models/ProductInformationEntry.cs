using System.Collections.Generic;

namespace ProductInformationReceiver.Models
{
    public class ProductInformationEntry
    {
        public ProductInformationEntry()
        {
            this.productinformationlist = new HashSet<ProductInformation>();
        }

        public int ProductInformationEntryId { get; set; }
        public string productname { get; set; }
        public virtual ICollection<ProductInformation> productinformationlist { get; set; }
        public string provider { get; set; }
        public string providerUrl { get; set; }
        public string htmlSearchPattern { get; set; }
        public string currency { get; set; }
    }
}
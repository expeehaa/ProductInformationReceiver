using System;

namespace ProductInformationReceiver.Models
{
    public class ProductInformation
    {
        public int ProductInformationId { get; set; }

        public double price { get; set; }

        public DateTime timestamp { get; set; }

        public int ProductInformationEntryId { get; set; }
        
        public virtual ProductInformationEntry entry { get; set; }


        public ProductInformation(double price, DateTime timestamp)
        {
            this.price = price;
            this.timestamp = timestamp;
        }

        public ProductInformation() {}
    }
}
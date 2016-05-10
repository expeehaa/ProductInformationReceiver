using Newtonsoft.Json;
using ProductInformationReceiver.Models;
using ProductInformationReceiver.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ProductInformationReceiver.Controllers
{
    public class PIRController : Controller
    {
        private static string all = "all";

        private static Regex regexpDates = new Regex("((\\d{4}\\#\\d{2}\\#\\d{2})|\\*)");
        //YYYY#MM#DD:YYYY#MM#DD


        // GET: PIR
        /// <summary>
        /// Returns a JSON string containing the elements of the ProductInformation database with given filters.
        /// </summary>
        /// <param name="productnames">Name(s) of the product. Seperated with ',', or all</param>
        /// <param name="currencies">Currencies. Seperated with ',', or all</param>
        /// <param name="providers">Who wants to sell/buy the product. Seperated with ',', or all</param>
        /// <param name="betweenDates">Time span for the product information values. Have to match the format YYYY#MM#DD:YYYY#MM#DD. One or both sides of ':' can be replaced with '*'.</param>
        /// <returns></returns>
        public string Index(string productnames = "all", string currencies = "all", string providers = "all", string betweenDates = "*:*")
        {
            using (var db = new PIRContext())
            {
                if(db.pir.Count() == 0) return "No values in the <b>database</b>!";

                var entriesWithoutDatecheck = db.pir.Where(entry => (productnames.ToLower().Split(',').Contains(entry.productname) || productnames.ToLower().Split(',').Contains(all)) && 
                        (currencies.ToLower().Split(',').Contains(entry.currency) || currencies.ToLower().Split(',').Contains(all))
                );

                if(entriesWithoutDatecheck.Count() == 0)
                {
                    return JsonConvert.SerializeObject(entriesWithoutDatecheck);
                }

                //date validation check
                if(betweenDates.Split(':').Count() == 2)
                {
                    var datestrings = betweenDates.Split(':');
                    if(!regexpDates.IsMatch(datestrings[0]) && !regexpDates.IsMatch(datestrings[1]))
                    {
                        return JsonConvert.SerializeObject(entriesWithoutDatecheck);
                    }
                }

                List<ProductInformationEntry> entries = new List<ProductInformationEntry>();
                
                if(betweenDates == "*:*")
                {
                    entries = entriesWithoutDatecheck.ToList();
                }
                else
                {
                    var datestrings = betweenDates.Split(':');
                    //put the datetime strings into DateTime objects
                    var firstDate = (betweenDates.Split(':')[0] == "*" ? DateTime.MinValue : new DateTime(int.Parse(datestrings[0].Split('#')[0]), int.Parse(datestrings[0].Split('#')[1]), int.Parse(datestrings[0].Split('#')[2])));
                    var secondDate = (betweenDates.Split(':')[1] == "*" ? DateTime.MaxValue : new DateTime(int.Parse(datestrings[1].Split('#')[0]), int.Parse(datestrings[1].Split('#')[1]), int.Parse(datestrings[1].Split('#')[2])));

                    foreach (var entry in entriesWithoutDatecheck)
                    {
                        ProductInformationEntry newEntry = new ProductInformationEntry();

                        List<ProductInformation> pilist = new List<ProductInformation>();
                        foreach (var pi in entry.productinformationlist)
                        {
                            //datecheck
                            if(pi.timestamp.CompareTo(firstDate) >= 0 && pi.timestamp.CompareTo(secondDate) <= 0)
                            {
                                pilist.Add(pi);
                            }
                        }

                        //add the productinformation to the newEntry var
                        newEntry.productname = entry.productname;
                        newEntry.provider = entry.provider;
                        newEntry.currency = entry.currency;
                        newEntry.productinformationlist = pilist;

                        entries.Add(newEntry);
                    }
                }

                return JsonConvert.SerializeObject(entries);
            }
        }

        //POST: PIR/addProduct
        /// <summary>
        /// Saves a new product in the database for which information will be gathered.
        /// </summary>
        /// <param name="body">String in JSON format that fits the ProductInformationEntry class</param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public bool addProduct([FromBody]string body)
        {
            var product = JsonConvert.DeserializeObject<ProductInformationEntry>(body);

            if (Uri.IsWellFormedUriString(product.providerUrl, UriKind.RelativeOrAbsolute)) return false;

            using (var db = new PIRContext())
            {
                db.pir.Add(product);
                db.SaveChanges();
            }

            return true;
        }

        //GET: PIR/update
        /// <summary>
        /// Updates the product information manually.
        /// </summary>
        public void update()
        {
            new ProductInformationRequestJob().updateProductInformation();
        }
    }
}

using HtmlAgilityPack;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace ProductInformationReceiver.Models.Requests
{
    public class ProductInformationRequestJob : IJob
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<ProductInformationRequestJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(a => a.WithIntervalInHours(24).OnEveryDay().StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))).Build();
            scheduler.ScheduleJob(job, trigger);
        }

        public void Execute(IJobExecutionContext context)
        {
            updateProductInformation();
        }

        public void updateProductInformation()
        {
            using (var db = new PIRContext())
            {
                if (db.pir.Count() == 0) return;

                foreach (var entry in db.pir)
                {
                    var wr = WebRequest.Create(entry.providerUrl);
                    var responseStream = wr.GetResponse().GetResponseStream();

                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(responseStream);

                    HtmlNode node = doc.DocumentNode.SelectSingleNode(entry.htmlSearchPattern);
                    var priceString = node.InnerHtml;
                    double price = 0;

                    try
                    {
                        var match = new Regex("/^[0-9]+((\\.|\\,)[0-9]+)?$").Match(priceString);
                        price = double.Parse(match.Value);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    entry.productinformationlist.Add(new ProductInformation(price, DateTime.Now));
                }

                db.SaveChanges();
            }
        }
    }
}
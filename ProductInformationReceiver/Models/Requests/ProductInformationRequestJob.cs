using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            using (var db = new PIRContext())
            {
                if (db.pir.Count() == 0) return;

                foreach (var entry in db.pir)
                {
                    var wr = WebRequest.Create(entry.providerUrl);
                    var responseStream = wr.GetResponse().GetResponseStream();

                    XmlTextReader reader = new XmlTextReader(responseStream);
                    reader.
                }
            }
        }
    }
}
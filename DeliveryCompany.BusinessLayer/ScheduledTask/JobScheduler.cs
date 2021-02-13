//using Quartz;
//using Quartz.Impl;

//namespace DeliveryCompany.BusinessLayer.ScheduledTask
//{
//    //public class JobScheduler
//    //{
//    //    public async void Start()
//    //    {
//    //        ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
//    //        IScheduler scheduler = await schedulerFactory.GetScheduler();
//    //        await scheduler.Start();

//    //        IJobDetail job = JobBuilder.Create<ScheduledTaskJob>().Build();

//    //        ITrigger trigger = TriggerBuilder.Create()
//    //            .WithDailyTimeIntervalSchedule
//    //              (s =>
//    //                s.WithIntervalInHours(24)
//    //                .OnEveryDay()
//    //                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
//    //              )
//    //            .Build();

//    //        await scheduler.ScheduleJob(job, trigger);
//    //    }
//    //}
//}

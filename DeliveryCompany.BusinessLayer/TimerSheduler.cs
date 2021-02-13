using DeliveryCompany.BusinessLayer.SpaceTimeProviders;
using System;
using System.Timers;

namespace DeliveryCompany.BusinessLayer
{
    public interface ITimerSheduler
    {
        void Dispose();
        void Start();
        void Stop();
    }

    public class TimerSheduler : ITimerSheduler
    {
        private IPackageStatusOnTheGoService _packageStatus;
        private ITimeProvider _fastForwardTimeProvider;
        private IWaybillsService _waybillsService;

        public TimerSheduler(
            IPackageStatusOnTheGoService packageService,
            ITimeProvider fastForwardTimeProvider,
            IWaybillsService waybillsService)
        {
            _packageStatus = packageService;
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _waybillsService = waybillsService;
        }

        private readonly Timer aTimer;

        public TimerSheduler()
        {
            aTimer = new Timer();
            aTimer.Interval = 1000;
        }

        public void Start()
        {
            aTimer.Elapsed += OnTimeEvent;
            this.aTimer.Start();
        }

        public void Stop()
        {
            aTimer.Elapsed -= OnTimeEvent;
            this.aTimer.Stop();
        }
        //public void SetTimer()
        //{

        //    aTimer = new Timer(1000);
        //    aTimer.Start();

        //    aTimer.Elapsed += OnTimeEvent;
        //    aTimer.AutoReset = true;
        //    aTimer.Enabled = true;
        //}

        private void OnTimeEvent(object sender, ElapsedEventArgs e)
        {
            var now = _fastForwardTimeProvider.Now;

            if (now.TimeOfDay >= new TimeSpan(0, 0, 0, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 0, 0, 59, 999))
            {
                _waybillsService.CreateWaybills();
            }

            if (now.TimeOfDay >= new TimeSpan(0, 8, 0, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 8, 0, 59, 999))
            {
                _packageStatus.ChangingPackageStatusAtTheBeginningOfJourney();
            }

            if (now.TimeOfDay >= new TimeSpan(0, 18, 0, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 18, 0, 59, 999))
            {
                _packageStatus.ChangingPackageStatusAtTheEndOfJourney();
            }
        }

        public void Dispose()
        {
            if (this.aTimer != null)
            {
                this.aTimer.Dispose();
            }
        }
    }
}

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
        private readonly Timer _aTimer;

        public TimerSheduler(
            IPackageStatusOnTheGoService packageService,
            ITimeProvider fastForwardTimeProvider,
            IWaybillsService waybillsService)
        {
            _packageStatus = packageService;
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _waybillsService = waybillsService;
            _aTimer = new Timer
            {
                Interval = 1000
            };
        }

        

        public void Start()
        {
            _aTimer.Elapsed += OnTimeEvent;
            this._aTimer.Start();
        }

        public void Stop()
        {
            _aTimer.Elapsed -= OnTimeEvent;
            this._aTimer.Stop();
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

            if (now.TimeOfDay >= new TimeSpan(0, 0, 0, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 0, 7, 59, 999))
            {
                _waybillsService.CreateWaybills();
            }

            if (now.TimeOfDay >= new TimeSpan(0, 8, 0, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 8, 7, 59, 999))
            {
                _packageStatus.ChangingPackageStatusAtTheBeginningOfJourney();
            }

            if (now.TimeOfDay >= new TimeSpan(0, 18, 0, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 18, 7, 59, 999))
            {
                _packageStatus.ChangingPackageStatusAtTheEndOfJourney();
            }
        }

        public void Dispose()
        {
            if (this._aTimer != null)
            {
                this._aTimer.Dispose();
            }
        }
    }
}

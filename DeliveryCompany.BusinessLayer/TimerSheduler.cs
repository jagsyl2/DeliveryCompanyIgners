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
        private ICourierRatingsService _courierRatingsService;
        private readonly Timer _aTimer;

        public TimerSheduler(
            IPackageStatusOnTheGoService packageStatusService,
            ITimeProvider fastForwardTimeProvider,
            IWaybillsService waybillsService,
            ICourierRatingsService courierRatingsService)
        {
            _packageStatus = packageStatusService;
            _fastForwardTimeProvider = fastForwardTimeProvider;
            _waybillsService = waybillsService;
            _courierRatingsService = courierRatingsService;

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
                _packageStatus.ChangingPackageStatusFromManualWaybillAtTheEndOfWork();
            }

            if (now.TimeOfDay >= new TimeSpan(0, 18, 05, 0, 0) && now.TimeOfDay <= new TimeSpan(0, 18, 05, 59, 999))
            {
                _courierRatingsService.CountAverageRatingForWaybill();
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

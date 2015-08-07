namespace PotteryWheel
{
    public class SpeedFeedback
    {
        private const bool FEEDBACK_MODE = true;
        private const uint SLOWEST_DURATION = 4000;
        private const uint FASTEST_DURATION = 20000;
        private const uint RANGE_DURATION = FASTEST_DURATION - SLOWEST_DURATION;

        private double _pedalPosition;

        private double _previousRpm;
        private double _currentRpm;

        private double _targetRpm;

        public const double MinRpm = 40.0;
        public const double MaxRpm = 300.0;

        private const uint MIN_RPM_UNLOADED_DURATION = 4000;
        private const uint MIN_RPM_LOADED_DURATION = 6000;
        private const uint MAX_RPM_UNLOADED_DURATION = 14250;
        private const uint MAX_RPM_LOADED_DURATION = 18000;

        public void SetCurrentState(double rpm)
        {
            _previousRpm = _currentRpm;
            _currentRpm = rpm;
        }

        public void SetPedalPosition(double pos, double rpm)
        {
            _pedalPosition = pos;
            _targetRpm = rpm;
        }

        public uint Duration
        {
            get
            {
                var duration = (uint) (_pedalPosition*RANGE_DURATION) + SLOWEST_DURATION;

                if (FEEDBACK_MODE)
                {
                    // Compare previous rpm to current
                    // Increase or decrease 
                }

                return duration;
            }
        }
    }
}
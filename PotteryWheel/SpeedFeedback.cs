namespace PotteryWheel
{
    public class SpeedFeedback
    {
        private const bool FEEDBACK_MODE = true;
        private const uint SLOWEST_DURATION = 4000;
        private const uint FASTEST_DURATION = 20000;
        private const uint RANGE_DURATION = FASTEST_DURATION - SLOWEST_DURATION;

        private double _pedalPosition;
        private double _targetRpm;

        private double _previousRpm;
        private double _currentRpm;

        private uint _previousDuration;
        private uint _currentDuration;

        public const double MinRpm = 40.0;
        public const double MaxRpm = 300.0;

        private const double MIN_RPM_UNLOADED_DURATION = 4000;
        private const double MIN_RPM_LOADED_DURATION = 6000;
        private const double MAX_RPM_UNLOADED_DURATION = 14250;
        private const double MAX_RPM_LOADED_DURATION = 18000;

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

        public void Recalculate()
        {
            _previousDuration = _currentDuration;
            _currentDuration = (uint) (_pedalPosition*RANGE_DURATION) + SLOWEST_DURATION;

            if (FEEDBACK_MODE)
            {
                // Compare previous rpm to current
                // Increase or decrease 
                var range = GetRangeForRpm(_targetRpm);
            }
        }

        public uint Duration
        {
            get { return _currentDuration; }
        }

        private LegalRange GetRangeForRpm(double rpm)
        {
            var unloadedRpm = CalculateY(rpm, MinRpm, MaxRpm, MIN_RPM_UNLOADED_DURATION, MAX_RPM_UNLOADED_DURATION);
            var loadedRpm = CalculateY(rpm, MinRpm, MaxRpm, MIN_RPM_LOADED_DURATION, MAX_RPM_LOADED_DURATION);

            return new LegalRange(unloadedRpm, loadedRpm);
        }

        private static double CalculateY(double x, double xmin, double xmax, double ymin, double ymax)
        {
            var run = xmax - xmin;
            var rise = ymax - ymin;
            var slope = rise/run;

            var xdatum = x - xmin;
            var ydatum = slope*xdatum;

            var y = ydatum + ymin;
            return y;
        }
    }

    public class LegalRange
    {
        public LegalRange(double unloaded, double loaded)
        {
            UnloadedDuration = (uint)unloaded;
            LoadedDuration = (uint)loaded;
        }
        public uint UnloadedDuration { get; set; }
        public uint LoadedDuration { get; set; }
    }
}
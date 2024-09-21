using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TimescaleDisplay
{
    public class Calculator
    {
        public event Action<float>? TimescaleChanged;

        /// <summary>
        /// The delta in the game time interval between ticks.
        /// </summary>
        private const float BaseIntervalOffsetPerTick = 16f;

        /// <summary>
        /// The number of samples to measure for smoothing the calculated time scale.
        /// </summary>
        private const int SpeedMeasurementSampleCount = 3;

        /// <summary>
        /// This mod's method of measuring timescale isn't precise, so we need to account for
        /// noise in the sampling. Only update the time scale for if the change is greater than or equal to this value.
        /// </summary>
        private const float MinTimeScaleChange = 0.03f;

        private readonly float[] _timeScaleMeasurements = Enumerable.Repeat(1f, SpeedMeasurementSampleCount).ToArray();

        private int _gameTimeIntervalLastTick;

        private int _nextTimeScaleIndex;

        public float AvgTimescale { get; set; }

        public Calculator(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsMainPlayer)
            {
                return;
            }

            int gameTimeInterval = Game1.gameTimeInterval;
            int intervalDiff = gameTimeInterval - _gameTimeIntervalLastTick;
            if (intervalDiff >= 0)
            {
                float timeScale = intervalDiff / BaseIntervalOffsetPerTick;
                _timeScaleMeasurements[_nextTimeScaleIndex] = timeScale;
                _nextTimeScaleIndex = (_nextTimeScaleIndex + 1) % SpeedMeasurementSampleCount;
                float newAvgTimeScale = CalculateAverageTimeScale();
                float timeScaleChange = Math.Abs(newAvgTimeScale - AvgTimescale);
                if (timeScaleChange > MinTimeScaleChange)
                {
                    AvgTimescale = newAvgTimeScale;
                    TimescaleChanged?.Invoke(AvgTimescale);
                }
            }

            _gameTimeIntervalLastTick = gameTimeInterval;
        }

        private float CalculateAverageTimeScale()
        {
            var value = 0f;
            foreach (float measurement in _timeScaleMeasurements)
            {
                value += measurement;
            }

            value /= SpeedMeasurementSampleCount;
            return value;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;

namespace TimescaleDisplay
{
    public class TimescaleDisplayMod : Mod
    {
        /// <summary>
        /// The delta in the game time interval between ticks.
        /// </summary>
        private const float BaseIntervalOffsetPerTick = 16f;

        /// <summary>
        /// The number of samples to measure for smoothing the calculated time scale.
        /// </summary>
        private const int SpeedMeasurementSampleCount = 3;

        private readonly float[] _timeScaleMeasurements = Enumerable.Repeat(1f, SpeedMeasurementSampleCount).ToArray();

        private int _gameTimeIntervalLastTick;

        private int _nextTimeScaleIndex;

        private float _avgTimeScale = 1.0f;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void RenderLabel(SpriteBatch spriteBatch)
        {
            var text = $"{_avgTimeScale:0.0}x";

            SpriteFont font = Game1.smallFont;
            Vector2 stringSize = font.MeasureString(text);

            Rectangle safeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

            var position = new Vector2(safeArea.Right - 235, safeArea.Top + 86);
            // Origin is middle-right of label
            position.X -= stringSize.X;
            position.Y -= stringSize.Y / 2f;

            const byte Alpha = 200;
            Color color = Color.White;
            color.A = Alpha;
            Color colorShadow = Color.Black;
            colorShadow.A = Alpha;

            Utility.drawTextWithColoredShadow(
                spriteBatch,
                text,
                font,
                position,
                color,
                colorShadow,
                1.125f);
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            int gameTimeInterval = Game1.gameTimeInterval;
            int intervalDiff = gameTimeInterval - _gameTimeIntervalLastTick;
            if (intervalDiff >= 0)
            {
                float timeScale = intervalDiff / BaseIntervalOffsetPerTick;
                _timeScaleMeasurements[_nextTimeScaleIndex] = timeScale;
                _nextTimeScaleIndex = (_nextTimeScaleIndex + 1) % SpeedMeasurementSampleCount;
                _avgTimeScale = CalulateAverageTimeScale();
            }

            _gameTimeIntervalLastTick = gameTimeInterval;
        }

        private float CalulateAverageTimeScale()
        {
            var value = 0f;
            foreach (float measurement in _timeScaleMeasurements)
            {
                value += measurement;
            }

            value /= SpeedMeasurementSampleCount;
            return value;
        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            RenderLabel(e.SpriteBatch);
        }
    }
}
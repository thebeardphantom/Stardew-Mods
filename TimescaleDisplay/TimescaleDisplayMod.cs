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

        /// <summary>
        /// This mod's method of measuring timescale isn't precise, so we need to account for
        /// noise in the sampling. Only update the time scale for networked players
        /// if the change is greater than or equal to this value.
        /// </summary>
        private const float MinNetworkedTimeScaleChange = 0.03f;

        private readonly float[] _timeScaleMeasurements = Enumerable.Repeat(1f, SpeedMeasurementSampleCount).ToArray();

        private int _gameTimeIntervalLastTick;

        private int _nextTimeScaleIndex;

        private float _avgTimeScale = 1.0f;

        private string[]? _messageModIDs;

        public override void Entry(IModHelper helper)
        {
            _messageModIDs = new[] { ModManifest.UniqueID, };
            helper.Events.Display.RenderedHud += OnRenderedHud;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                return;
            }

            if (e.FromModID != ModManifest.UniqueID)
            {
                return;
            }

            var timescaleChangeMessage = e.ReadAs<TimescaleChangeMessage>();
            _avgTimeScale = timescaleChangeMessage.AvgTimeScale;
            Monitor.Log($"Received {nameof(TimescaleChangeMessage)} with value {_avgTimeScale}.");
        }

        private void RenderLabel(SpriteBatch spriteBatch)
        {
            if (Game1.eventUp)
            {
                return;
            }

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
                float timeScaleChange = Math.Abs(newAvgTimeScale - _avgTimeScale);
                if (timeScaleChange >= 0.00001f)
                {
                    _avgTimeScale = newAvgTimeScale;
                    if (timeScaleChange > MinNetworkedTimeScaleChange)
                    {
                        SendToAllPlayers();
                    }
                }
            }

            _gameTimeIntervalLastTick = gameTimeInterval;
        }

        private void SendToAllPlayers()
        {
            Monitor.Log($"Sending {nameof(TimescaleChangeMessage)} with value {_avgTimeScale} to all players.");
            var message = new TimescaleChangeMessage(_avgTimeScale);
            Helper.Multiplayer.SendMessage(message, nameof(TimescaleChangeMessage), _messageModIDs);
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

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            RenderLabel(e.SpriteBatch);
        }
    }
}
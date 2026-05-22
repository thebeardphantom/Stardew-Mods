using BeardPhantom.Stardewtilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TimescaleDisplay
{
    public class Rendering
    {
        private readonly Configuration _configuration;

        private readonly Calculator _calculator;

        private Vector2 _stringSizeMax;

        public Rendering(IModHelper helper, Configuration configuration, Calculator calculator)
        {
            _configuration = configuration;
            _calculator = calculator;
            helper.Events.Display.RenderingHud += OnRenderingHud;
            helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private void OnRenderingHud(object? sender, RenderingHudEventArgs e)
        {
            RenderLabel(e.SpriteBatch, HudRenderPhase.Rendering);
        }

        private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
        {
            RenderLabel(e.SpriteBatch, HudRenderPhase.Rendered);
        }

        private void RenderLabel(SpriteBatch spriteBatch, HudRenderPhase phase)
        {
            if (!CheckShouldRender(phase))
            {
                return;
            }

            IReadOnlyConfigurationData config = _configuration.Data;

            float avgTimeScale = _calculator.AvgTimescale;
            var text = $"{avgTimeScale:0.0}x";

            SpriteFont font = Game1.smallFont;
            Vector2 stringSize = font.MeasureString(text);
            _stringSizeMax = Vector2.Max(stringSize, _stringSizeMax);

            Vector2 stringRenderSize = _stringSizeMax * config.Scale;
            Rectangle safeArea = Utility.getSafeArea();

            var labelRect = new Rectangle(safeArea.GetAnchorLocation(RectAnchor.TopRight), stringRenderSize.ToPoint());
            labelRect.Location += new Point(-config.RenderOffsetX, config.RenderOffsetY);

            if (config.ShowBackground)
            {
                Utility.DrawSquare(
                    spriteBatch,
                    labelRect.GetAnchoredRectangle(RectAnchor.Right),
                    0,
                    backgroundColor: config.ColorBackground.GetPremultiplied());
            }

            var labelPosition = labelRect.GetOffsetLocationForAnchor(RectAnchor.Right).ToVector2();
            if (config.RenderShadow)
            {
                var shadowColor = new Color(Color.Black, config.Color.A);
                Utility.drawTextWithColoredShadow(
                    spriteBatch,
                    text,
                    font,
                    labelPosition,
                    config.Color.GetPremultiplied(),
                    shadowColor.GetPremultiplied(),
                    config.Scale);
            }
            else
            {
                spriteBatch.DrawString(
                    font,
                    text,
                    labelPosition,
                    config.Color.GetPremultiplied(),
                    0f,
                    Vector2.Zero,
                    Vector2.One * config.Scale,
                    SpriteEffects.None,
                    0f);
            }
        }

        private bool CheckShouldRender(HudRenderPhase phase)
        {
            if (!_configuration.Data.DisplayEnabled)
            {
                return false;
            }

            bool wouldRenderAboveHud = phase == HudRenderPhase.Rendered;
            if (_configuration.Data.RenderAboveHud != wouldRenderAboveHud)
            {
                return false;
            }

            if (Game1.isFestival())
            {
                return false;
            }

            if (Game1.eventUp)
            {
                return false;
            }

            if (!Game1.displayHUD)
            {
                return false;
            }

            return true;
        }
    }
}
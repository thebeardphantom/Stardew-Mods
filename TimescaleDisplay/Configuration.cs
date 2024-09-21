using GenericModConfigMenu;
using GMCMOptions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Globalization;

namespace TimescaleDisplay
{
    public class Configuration
    {
        private const string ScreenPositionHelpText = "Timescale display is anchored at the top-right of the screen.";

        private const string GenericModMenuId = "spacechase0.GenericModConfigMenu";

        private const string? FloatFormat = "0.00";
        private const string GenericModMenuOptionsId = "jltaylor-us.GMCMOptions";

        private static readonly CultureInfo s_currentCulture = CultureInfo.CurrentCulture;

        private readonly IModHelper _helper;

        private readonly IManifest _manifest;

        /// <summary>
        /// A "live" version of the config that allows for the game to instantly react to configuration changes made by interacting
        /// with
        /// the editor UI without
        /// </summary>
        private readonly ConfigurationData _dataRealtime = new ConfigurationData().Reset();

        private ConfigurationData _dataSaved = new ConfigurationData().Reset();

        private IGenericModConfigMenuApi? _configMenuApi;

        private IGMCMOptionsAPI? _configMenuOptionsApi;

        private bool _isDisplayingConfigMenu;

        public IReadOnlyConfigurationData Data => _dataRealtime;

        public Configuration(IModHelper helper, IManifest manifest)
        {
            _helper = helper;
            _manifest = manifest;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            bool isDisplayingConfigMenuNow = _configMenuApi!.TryGetCurrentMenu(out IManifest? currentMod, out _)
                                             && currentMod.UniqueID == _manifest.UniqueID;
            if (_isDisplayingConfigMenu && !isDisplayingConfigMenuNow)
            {
                // Menu was just closed, make sure realtime valus match saved data.
                _dataRealtime.OverwriteFrom(_dataSaved);
            }

            _isDisplayingConfigMenu = isDisplayingConfigMenuNow;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            _configMenuApi = _helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(GenericModMenuId);
            _configMenuOptionsApi = _helper.ModRegistry.GetApi<IGMCMOptionsAPI>(GenericModMenuOptionsId);
            if (_configMenuApi == null || _configMenuOptionsApi == null)
            {
                return;
            }

            _helper.Events.Display.MenuChanged += OnMenuChanged;

            _dataSaved = _helper.ReadConfig<ConfigurationData>();
            if (_dataSaved.ResetIfInvalid())
            {
                _helper.WriteConfig(_dataSaved);
            }

            _dataRealtime.OverwriteFrom(_dataSaved);

            _configMenuApi.Register(_manifest, ResetModConfig, SaveModConfig);
            _configMenuApi.OnFieldChanged(_manifest, OnValueChanged);

            CreateGlobalSection();
            CreateLabelSection();
            CreateBackgroundSection();
        }

        private void OnValueChanged(string fieldId, object value)
        {
            if (value is bool boolValue)
            {
                switch (fieldId)
                {
                    case nameof(ConfigurationData.DisplayEnabled):
                    {
                        _dataRealtime.DisplayEnabled = boolValue;
                        break;
                    }
                    case nameof(ConfigurationData.ShowBackground):
                    {
                        _dataRealtime.ShowBackground = boolValue;
                        break;
                    }
                    case nameof(ConfigurationData.DrawShadow):
                    {
                        _dataRealtime.DrawShadow = boolValue;
                        break;
                    }
                }
            }
            else if (value is Color colorValue)
            {
                switch (fieldId)
                {
                    case nameof(ConfigurationData.Color):
                    {
                        _dataRealtime.Color = colorValue;
                        break;
                    }
                    case nameof(ConfigurationData.ColorBackground):
                    {
                        _dataRealtime.ColorBackground = colorValue;
                        break;
                    }
                }
            }
        }

        private void CreateGlobalSection()
        {
            _configMenuApi!.AddSectionTitle(_manifest, () => "Global");
            _configMenuApi.AddBoolOption(
                _manifest,
                name: () => "Display Enabled",
                tooltip: () => "Toggles whether the timescale is displayed.",
                getValue: () => _dataSaved.DisplayEnabled,
                setValue: value => _dataSaved.DisplayEnabled = value,
                fieldId: nameof(ConfigurationData.DisplayEnabled)
            );

            _configMenuApi.AddNumberOption(
                _manifest,
                () => _dataSaved.Scale,
                value =>
                {
                    _dataSaved.Scale = value;
                },
                () => "Scale",
                formatValue: value =>
                {
                    _dataRealtime.Scale = value;
                    return value.ToString(FloatFormat, s_currentCulture);
                },
                min: ConfigurationData.ScaleMin,
                max: ConfigurationData.ScaleMax);

            /*
             * Rendering Offset
             */
            _configMenuApi.AddParagraph(_manifest, () => ScreenPositionHelpText);
            _configMenuApi.AddNumberOption(
                _manifest,
                () => _dataSaved.RenderOffsetX,
                v => _dataSaved.RenderOffsetX = v,
                () => "Offset X",
                formatValue: value =>
                {
                    _dataRealtime.RenderOffsetX = value;
                    return value.ToString(s_currentCulture);
                },
                min: 0,
                max: Game1.viewport.Width);
            _configMenuApi.AddNumberOption(
                _manifest,
                () => _dataSaved.RenderOffsetY,
                value => _dataSaved.RenderOffsetY = value,
                () => "Offset Y",
                formatValue: value =>
                {
                    _dataRealtime.RenderOffsetY = value;
                    return value.ToString(s_currentCulture);
                },
                min: 0,
                max: Game1.viewport.Height);
        }

        private void CreateLabelSection()
        {
            _configMenuApi!.AddSectionTitle(_manifest, () => "Label");
            _configMenuApi.AddBoolOption(
                _manifest,
                name: () => "Draw Shadow",
                tooltip: () => "Toggles the label has a shadow drawn.",
                getValue: () => _dataSaved.DrawShadow,
                setValue: value => _dataSaved.DrawShadow = value,
                fieldId: nameof(ConfigurationData.DrawShadow)
            );
            _configMenuOptionsApi!.AddColorOption(
                _manifest,
                () => _dataSaved.Color,
                value => _dataSaved.Color = value,
                () => "Color",
                fieldId: nameof(ConfigurationData.Color));
        }

        private void CreateBackgroundSection()
        {
            _configMenuApi!.AddSectionTitle(_manifest, () => "Background");
            _configMenuApi.AddBoolOption(
                _manifest,
                name: () => "Enabled",
                tooltip: () => "Toggles whether the background box is displayed.",
                getValue: () => _dataSaved.ShowBackground,
                setValue: value => _dataSaved.ShowBackground = value,
                fieldId: nameof(ConfigurationData.ShowBackground)
            );
            _configMenuOptionsApi!.AddColorOption(
                _manifest,
                () => _dataSaved.ColorBackground,
                value => _dataSaved.ColorBackground = value,
                () => "Color",
                fieldId: nameof(ConfigurationData.ColorBackground));
        }

        private void ResetModConfig()
        {
            _dataSaved.Reset();
            _dataRealtime.Reset();
        }

        private void SaveModConfig()
        {
            _dataRealtime.OverwriteFrom(_dataSaved);
            _helper.WriteConfig(_dataSaved);
        }
    }
}
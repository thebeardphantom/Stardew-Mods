using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TimescaleDisplay
{
    public class NetworkSync
    {
        private readonly IModHelper _helper;

        private readonly IManifest _manifest;

        private readonly IMonitor _monitor;

        private readonly Calculator _calculator;

        private readonly string[]? _messageModIDs;

        public NetworkSync(IModHelper helper, IManifest manifest, IMonitor monitor, Calculator calculator)
        {
            _helper = helper;
            _manifest = manifest;
            _monitor = monitor;
            _calculator = calculator;
            _messageModIDs = new[] { manifest.UniqueID, };
            _calculator.TimescaleChanged += OnTimescaleChanged;
            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
        }

        public void SendToAllPlayers(float avgTimeScale)
        {
            _monitor.Log($"Sending {nameof(ChangeNetworkMessage)} with value {avgTimeScale} to all players.");
            var message = new ChangeNetworkMessage(avgTimeScale);
            _helper.Multiplayer.SendMessage(message, nameof(ChangeNetworkMessage), _messageModIDs);
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                return;
            }

            if (e.FromModID != _manifest.UniqueID)
            {
                return;
            }

            var timescaleChangeMessage = e.ReadAs<ChangeNetworkMessage>();
            float avgTimescale = timescaleChangeMessage.AvgTimeScale;
            _calculator.AvgTimescale = avgTimescale;
            _monitor.Log($"Received {nameof(ChangeNetworkMessage)} with value {avgTimescale}.");
        }

        private void OnTimescaleChanged(float avgTimescale)
        {
            SendToAllPlayers(avgTimescale);
        }
    }
}
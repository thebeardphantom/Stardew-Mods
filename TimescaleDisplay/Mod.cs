using StardewModdingAPI;
using System.Diagnostics.CodeAnalysis;

namespace TimescaleDisplay
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class Mod : StardewModdingAPI.Mod
    {
        public override void Entry(IModHelper helper)
        {
            var configuration = new Configuration(helper, ModManifest);
            var calculator = new Calculator(helper);
            new NetworkSync(helper, ModManifest, Monitor, calculator);
            new Rendering(helper, configuration, calculator);
        }
    }
}
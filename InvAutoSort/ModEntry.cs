using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace InvAutoSort
{
    public class ModEntry : Mod
    {
        private const bool ModEnabled = false;

        private int _lastOrganizationTick;

        public override void Entry(IModHelper helper)
        {
            if (ModEnabled)
            {
                helper.Events.Player.InventoryChanged += OnInventoryChanged;
            }
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
            {
                return;
            }

            int currentTickCount = Game1.ticks;
            int tickDiff = currentTickCount - _lastOrganizationTick;
            if (tickDiff < 1)
            {
                return;
            }

            _lastOrganizationTick = currentTickCount;
            ItemGrabMenu.organizeItemsInList(e.Player.Items);
            Monitor.Log("InvAutoSort::Sort");
        }
    }
}
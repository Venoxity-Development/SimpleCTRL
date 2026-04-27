using Rage;
using SimpleCTRL.Handlers;

namespace SimpleCTRL.Engine.Helpers
{

    internal static class VehicleHelper
    {
        public static RepairShop GetNearestRepairShop()
        {
            foreach (RepairShop repairShop in ConfigHandler.RepairShops)
            {
                if (Game.LocalPlayer.Character.DistanceTo(repairShop.Position) < repairShop.UseRange)
                {
                    return repairShop;
                }
            }
            return null;
        }

        public static int GetBrakePressure(int pressure)
        {
            if (pressure > 230)
            {
                return 10;
            }
            else if (pressure > 205)
            {
                return 8;
            }
            else if (pressure > 180)
            {
                return 6;
            }
            else if (pressure > 155)
            {
                return 4;
            }
            return 2;
        }
    }
}

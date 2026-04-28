using Rage.Attributes;

namespace SimpleCTRL.Engine.FrontendSystems
{
    internal class ConsoleCommands : CommonPlugin
    {
        [ConsoleCommand]
        internal static void GetDevFuelInfo()
        {
            if (EntityExtensions.Exists(ClientCurrentVehicle))
            {
                double current = Math.Round(ClientCurrentVehicle.GetFuelLevel(), 2);
                double max = Math.Round(ClientCurrentVehicle.MaxFuelLevel(), 2);
                string model = ClientCurrentVehicle.DisplayName().ToUpper();
                Game.DisplayNotification("Fuel data for ~b~" + model + "~w~...");
                string msg = $"~g~FUEL: ~w~{current} L // ~b~MAX: ~w~{max} L";
                Game.DisplayNotification(msg);
            }
            else
            {
                Game.DisplayNotification("~r~ERROR: ~w~Cannot find player vehicle.");
            }
        }

        [ConsoleCommand]
        internal static void SetDevFuel(int percent)
        {
            if (EntityExtensions.Exists(ClientCurrentVehicle))
            {
                float max = (float)Math.Round(ClientCurrentVehicle.MaxFuelLevel(), 2);
                float toSet = max * ((float)percent / 100f);
                ClientCurrentVehicle.SetFuelLevel(toSet);
                string model = ClientCurrentVehicle.DisplayName().ToUpper();
                Game.DisplayNotification("Fuel data for ~b~" + model + "~w~...");
                string msg = $"~g~FUEL: ~w~Set fuel level to ~b~{toSet} ~w~L / ~b~{max} ~w~L.";
                Game.DisplayNotification(msg);
            }
            else
            {
                Game.DisplayNotification("~r~ERROR: ~w~Cannot find player vehicle.");
            }
        }
    }
}
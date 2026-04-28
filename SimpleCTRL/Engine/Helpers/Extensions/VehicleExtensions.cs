namespace SimpleCTRL.Engine.Helpers.Extensions
{
    internal static class VehicleExtensions
    {
        public static void ControlAircraftEngine(this Vehicle vehicle)
        {
            if (Game.IsControlJustPressed(0, GameControl.VehicleFlyUnderCarriage))
            {
                ToggleEngine(vehicle);
            }
        }

        /// <summary>
        /// Toggles the engine of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        public static void ToggleEngine(this Vehicle vehicle)
        {
            SetEngine(vehicle, !vehicle.IsEngineOn);
        }

        /// <summary>
        /// Sets the engine state of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="engineOn">Whether to turn the engine on or off.</param>
        public static void SetEngine(this Vehicle vehicle, bool engineOn)
        {
            vehicle.IsDriveable = engineOn;
            N.SetVehicleEngineOn(vehicle, engineOn, false, true);
            AircraftEngineOn = engineOn;
        }
    }
}

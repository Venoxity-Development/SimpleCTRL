namespace SimpleCTRL.Engine.Helpers
{
    internal static class FuelConstants
    {
        internal static bool AircraftEngineOn = false;
        internal static uint LastAircraftEngineHintDisplayed;
        internal static uint LastAircraftLowFuelWarning;
        internal static float LastAircraftAltitude = 0f;
        internal static DateTime LastWorldTime;
        internal static bool NozzleAttached = false;
        internal static bool NozzleInVehicle = false;
        internal static Rage.Object FuelNozzle = null;
        internal static Model FuelNozzleModel = new Model("prop_cs_fuel_nozle");
        internal static bool RefuelingAllowed = true;
        internal static bool VehicleFuelLevelInitialized = false;
        internal static float VehicleFuelCapacity = 65f;
        internal static bool WasDriver = false;
        internal static Vehicle MyVehicle;
        internal static List<Blip> GasStationMarkers = new List<Blip>();
        internal static GasStation AvailableGasStation = null;
        internal static List<GasStation> AvailableGasStations = new List<GasStation>();
        internal static List<GasPump> AvailableExtendedStations = new List<GasPump>();
        internal static List<Rage.Object> ExtendedStationPumpProps = new List<Rage.Object>();
        internal static Dictionary<int, TripInfo> TripInfos = new Dictionary<int, TripInfo>();
        internal static int Rope;
        internal static readonly string[] VehicleFuelTankBones = new string[5]
        {
            "petrolcap",
            "petroltank",
            "petroltank_r",
            "petroltank_l",
            "wheel_lr"
       };
        internal readonly static List<string> GasPumpProps = new List<string>
       {
            "prop_gas_pump_1d",
            "prop_gas_pump_1a",
            "prop_gas_pump_1b",
            "prop_gas_pump_1c",
            "prop_vintage_pump",
            "prop_gas_pump_old2",
            "prop_gas_pump_old3",
            "wire_pump_prop",
       };
    }
}
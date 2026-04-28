namespace SimpleCTRL.Engine.Data
{
    /// <summary>
    /// Defines the specifications for vehicle fuel consumption.
    /// </summary>
    internal struct VehicleFuelSpecs
    {
        /// <summary>
        /// Gets or sets the engine displacement of the vehicle (in liters).
        /// </summary>
        internal float Displacement { get; set; }

        /// <summary>
        /// Gets or sets the fuel consumption rate of the vehicle (liters per 100 kilometers).
        /// </summary>
        internal float LPer100KM { get; set; }
    }

    internal static class VehicleProperties
    {
        #region Vehicle Fuel Data by Class

        /// <summary>
        /// A dictionary that stores the fuel specifications for each vehicle class.
        /// </summary>
        public static Dictionary<VehicleClass, VehicleFuelSpecs> VehicleSpecs { get; } = new()
        {
            [VehicleClass.Compact] = new VehicleFuelSpecs
            {
                Displacement = 1.5f,
                LPer100KM = 7f
            },
            [VehicleClass.Sedan] = new VehicleFuelSpecs
            {
                Displacement = 3f,
                LPer100KM = 9f
            },
            [VehicleClass.SUV] = new VehicleFuelSpecs
            {
                Displacement = 3.5f,
                LPer100KM = 11f
            },
            [VehicleClass.Coupe] = new VehicleFuelSpecs
            {
                Displacement = 3.5f,
                LPer100KM = 10f
            },
            [VehicleClass.Muscle] = new VehicleFuelSpecs
            {
                Displacement = 5f,
                LPer100KM = 13f
            },
            [VehicleClass.SportClassic] = new VehicleFuelSpecs
            {
                Displacement = 3.5f,
                LPer100KM = 17f
            },
            [VehicleClass.Sport] = new VehicleFuelSpecs
            {
                Displacement = 3.9f,
                LPer100KM = 13f
            },
            [VehicleClass.Super] = new VehicleFuelSpecs
            {
                Displacement = 6.5f,
                LPer100KM = 19.6f
            },
            [VehicleClass.Motorcycle] = new VehicleFuelSpecs
            {
                Displacement = 0.6f,
                LPer100KM = 3.9f
            },
            [VehicleClass.OffRoad] = new VehicleFuelSpecs
            {
                Displacement = 1.5f,
                LPer100KM = 13f
            },
            [VehicleClass.Industrial] = new VehicleFuelSpecs
            {
                Displacement = 13f,
                LPer100KM = 36.2f
            },
            [VehicleClass.Utility] = new VehicleFuelSpecs
            {
                Displacement = 6f,
                LPer100KM = 19.6f
            },
            [VehicleClass.Van] = new VehicleFuelSpecs
            {
                Displacement = 5.4f,
                LPer100KM = 15.7f
            },
            [VehicleClass.Cycle] = new VehicleFuelSpecs
            {
                Displacement = 0f,
                LPer100KM = 0f
            },
            [VehicleClass.Boat] = new VehicleFuelSpecs
            {
                Displacement = 0f,
                LPer100KM = 0f
            },
            [VehicleClass.Helicopter] = new VehicleFuelSpecs
            {
                Displacement = 0f,
                LPer100KM = 0f
            },
            [VehicleClass.Plane] = new VehicleFuelSpecs
            {
                Displacement = 0f,
                LPer100KM = 0f
            },
            [VehicleClass.Service] = new VehicleFuelSpecs
            {
                Displacement = 5.4f,
                LPer100KM = 15.68f
            },
            [VehicleClass.Emergency] = new VehicleFuelSpecs
            {
                Displacement = 3.5f,
                LPer100KM = 10f
            },
            [VehicleClass.Military] = new VehicleFuelSpecs
            {
                Displacement = 6.5f,
                LPer100KM = 23.5f
            },
            [VehicleClass.Commercial] = new VehicleFuelSpecs
            {
                Displacement = 6f,
                LPer100KM = 19.6f
            },
            [VehicleClass.Rail] = new VehicleFuelSpecs
            {
                Displacement = 0f,
                LPer100KM = 0f
            }
        };

        #endregion
    }
}
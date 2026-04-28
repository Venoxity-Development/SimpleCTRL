namespace SimpleCTRL.API
{
    public static class Functions
    {
        /// <summary>
        /// Randomizes the fuel level of the vehicle.
        /// </summary>
        /// <param name="veh">The vehicle.</param>
        /// <param name="fuelCapacity">The fuel capacity of the vehicle.</param>
        /// <returns>The randomized fuel level.</returns>
        internal static float RandomizeFuelLevel(this Vehicle veh, float fuelCapacity)
        {
            float randomizedFuelLevel;
            if (new Random().Next(0, 4) != 0)
            {
                float min = fuelCapacity / 4f;
                float max = fuelCapacity / 2f;
                randomizedFuelLevel = (float)(new Random().NextDouble() * (double)(max - min) + (double)min);
            }
            else
            {
                randomizedFuelLevel = fuelCapacity;
            }
            return randomizedFuelLevel;
        }

        /// <summary>
        /// Initializes the fuel level of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        internal static void InitFuel(this Vehicle vehicle)
        {
            VehicleFuelLevelInitialized = true;
            VehicleFuelCapacity = MaxFuelLevel(vehicle);
            if (!N.DecorExistOn(vehicle, "_Fuel_Level"))
            {
                if (VehicleUtilities.IsAircraft(vehicle))
                {
                    N.DecorSetFloat(vehicle, "_Fuel_Level", VehicleFuelCapacity);
                }
                else
                {
                    N.DecorSetFloat(vehicle, "_Fuel_Level", RandomizeFuelLevel(vehicle, VehicleFuelCapacity));
                }
            }
            vehicle.FuelLevel = N.DecorGetFloat(vehicle, "_Fuel_Level");
        }

        /// <summary>
        /// Sets the fuel level of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="fuelLevel">The fuel level to set.</param>
        public static void SetFuelLevel(this Vehicle vehicle, float fuelLevel)
        {
            float max = MaxFuelLevel(vehicle);
            if (fuelLevel > max)
            {
                fuelLevel = max;
            }
            vehicle.FuelLevel = fuelLevel;
            N.DecorSetFloat(vehicle, "_Fuel_Level", fuelLevel);
        }

        /// <summary>
        /// Gets the fuel level of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <returns>The fuel level of the vehicle.</returns>
        public static float GetFuelLevel(this Vehicle vehicle)
        {
            if (N.DecorExistOn(vehicle, "_Fuel_Level"))
            {
                return N.DecorGetFloat(vehicle, "_Fuel_Level");
            }
            return 65f;
        }

        /// <summary>
        /// Gets the maximum fuel level of the vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <returns>The maximum fuel level of the vehicle.</returns>
        internal static float MaxFuelLevel(this Vehicle vehicle)
        {
            float maxFuel = vehicle.HandlingData.PetrolTankVolume;
            if (maxFuel == 0f)
            {
                return 65f;
            }
            return maxFuel;
        }
    }
}
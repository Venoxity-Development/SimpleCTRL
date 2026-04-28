namespace SimpleCTRL.Engine.InternalSystems
{
    internal static class VehicleRefuelingManager
    {
        /// <summary>
        /// Consumes fuel for an aircraft.
        /// </summary>
        /// <param name="vehicle">The aircraft.</param>
        internal static void ConsumeAircraftFuel(this Vehicle vehicle)
        {

            float fuel = Functions.GetFuelLevel(vehicle);
            if (fuel > 0f && vehicle.IsEngineOn)
            {
                TimeSpan timeElapsed = DateTime.UtcNow - LastWorldTime;
                float fuelUsed = UtilityHelper.ConsumeFuelAircraft(vehicle, timeElapsed);
                fuel -= fuelUsed;
                fuel = ((fuel < 0f) ? 0f : fuel);
            }
            fuel = ProcessRefuelingAircraft(vehicle, fuel);
            Functions.SetFuelLevel(vehicle, fuel);
        }

        /// <summary>
        /// Processes refueling for an aircraft.
        /// </summary>
        /// <param name="vehicle">The aircraft.</param>
        /// <param name="fuel">The current fuel level.</param>
        /// <returns>The updated fuel level.</returns>
        private static float ProcessRefuelingAircraft(this Vehicle vehicle, float fuel)
        {
            if (vehicle.IsInAir)
            {
                return fuel;
            }
            if (vehicle.Speed < 2f)
            {
                VehicleExtensions.ControlAircraftEngine(vehicle);
            }

            return fuel;
        }

        /// <summary>
        /// Consumes fuel for a road vehicle.
        /// </summary>
        /// <param name="vehicle">The road vehicle.</param>
        internal static void ConsumeRoadVehicleFuel(this Vehicle vehicle)
        {
            float fuel = vehicle.GetFuelLevel();
            if (fuel > 0f && vehicle.IsEngineOn)
            {
                if (!TripInfos.ContainsKey(vehicle.Handle.ToInt32()))
                {
                    TripInfos[vehicle.Handle.ToInt32()] = new TripInfo();
                }
                if (TripInfos[vehicle.Handle.ToInt32()].LastPosition == Vector3.Zero)
                {
                    TripInfos[vehicle.Handle.ToInt32()].LastPosition = vehicle.Position;
                }
                float distance = Vector3.Distance(TripInfos[vehicle.Handle.ToInt32()].LastPosition, vehicle.Position);
                float kmTravelled = Math.Abs(distance / 1000f) * 5.2f;
                float fuelUsed = UtilityHelper.ConsumeCarFuel(vehicle, kmTravelled);
                TripInfos[vehicle.Handle.ToInt32()].DistanceTraveledKM += kmTravelled;
                TripInfos[vehicle.Handle.ToInt32()].LastPosition = vehicle.Position;
                TripInfos[vehicle.Handle.ToInt32()].FuelConsumed += fuelUsed;
                fuel -= fuelUsed;
                if (fuel < 0.2f && vehicle.IsElectric() && vehicle.IsDriveable)
                {
                    Game.DisplayNotification("Your battery has run out of juice!");
                    vehicle.IsDriveable = false;
                }
                fuel = ((fuel < 0f) ? 0f : fuel);
            }
            fuel = ProcessRefuelingCar(vehicle, fuel);
            Functions.SetFuelLevel(vehicle, fuel);
        }

        /// <summary>
        /// Processes the refueling for a vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        /// <param name="fuel">The current fuel level.</param>
        /// <returns>The updated fuel level.</returns>
        private static float ProcessRefuelingCar(this Vehicle vehicle, float fuel)
        {
            if (AvailableGasStation == null || !GasStationManager.IsVehicleNearAnyPump(vehicle))
                return fuel;

            GasPump closestPump = GasStationManager.FindClosestPump(vehicle, AvailableGasStation);

            if (closestPump == null) return fuel;

            return fuel;
        }
    }
}
namespace SimpleCTRL.Engine.Helpers
{
    /// <summary>
    /// Provides various extension and helper methods.
    /// </summary>
    internal static class UtilityHelper
    {
        #region Float Utilities

        /// <summary>
        /// Safely returns a float value, ensuring it is neither infinity nor NaN.
        /// </summary>
        /// <param name="value">The float value to check.</param>
        /// <param name="defaultValue">The default value to return if the input is infinity or NaN.</param>
        /// <returns>The original float value if valid, otherwise the specified default value.</returns>
        internal static float SafeFloat(float value, float defaultValue)
        {
            return (float.IsInfinity(value) || float.IsNaN(value)) ? defaultValue : value;
        }

        #endregion

        #region Ped Utilities

        /// <summary>
        /// A placeholder method for manual refueling of a player character (Ped).
        /// </summary>
        internal static void ManualRefuel(this Ped playerPed)
        {
            // Implementation for manual refueling can go here.
        }

        #endregion

        #region PoolHandle Utilities

        /// <summary>
        /// Converts a <see cref="PoolHandle"/> to an integer.
        /// </summary>
        /// <param name="poolHandle">The <see cref="PoolHandle"/> to convert.</param>
        /// <returns>The integer value of the <see cref="PoolHandle"/>.</returns>
        internal static int ToInt32(this PoolHandle poolHandle)
        {
            return (int)poolHandle.Value;
        }

        #endregion

        #region Fuel Calculation Utilities

        /// <summary>
        /// Calculates the amount of fuel consumed by a vehicle based on the distance traveled.
        /// </summary>
        /// <param name="vehicle">The vehicle consuming fuel.</param>
        /// <param name="kilometresTravelled">The distance the vehicle has traveled in kilometers.</param>
        /// <returns>The amount of fuel consumed, in liters.</returns>
        internal static float ConsumeCarFuel(Vehicle v, float kilometresTravelled)
        {
            if (EntityExtensions.Exists(v))
            {
                float fuelUsed = 0f;

                if (v.Model.IsBicycle || v.Model.IsHelicopter || v.Model.IsPlane)
                {
                    return 0f;
                }
                if (v.Model.IsCar || v.Model.IsBike || v.Model.IsQuadBike)
                {
                    return ConsumeFuelRoadVehicle(v, kilometresTravelled);
                }
                return 0f;
            }
            return 0f;
        }

        internal static float ConsumeFuelAircraft(Vehicle v, TimeSpan timeElapsed)
        {
            return 0f;
        }

        private static float ConsumeFuelRoadVehicle(Vehicle v, float kilometresTravelled)
        {
            VehicleFuelSpecs vfs = VehicleProperties.VehicleSpecs[(VehicleClass)18];
            if (VehicleProperties.VehicleSpecs.ContainsKey(v.Class))
            {
                vfs = VehicleProperties.VehicleSpecs[v.Class];
            }
            if (v.Model.IsQuadBike)
            {
                VehicleFuelSpecs vehicleFuelSpecs = default(VehicleFuelSpecs);
                vehicleFuelSpecs.Displacement = 1f;
                vehicleFuelSpecs.LPer100KM = 4f;
                vfs = vehicleFuelSpecs;
            }
            float fuelUsed = 0f;
            float vehSpeed = Math.Abs(v.Speed);
            if (vehSpeed > 3f)
            {
                float baseLitresPer100Km = vfs.LPer100KM;
                int gear = v.CurrentGear;
                if (v.Acceleration() < 0f)
                {
                    gear = 1;
                }
                float econLossFactor = v.CurrentRPM() / (float)v.CurrentGear;
                econLossFactor = SafeFloat(econLossFactor, 1f);
                float accelerationLossFactor = Math.Abs(v.Acceleration() * 1.5f);
                accelerationLossFactor = SafeFloat(accelerationLossFactor, 0f);
                float normalizedFuelEconomy = baseLitresPer100Km + baseLitresPer100Km * econLossFactor + accelerationLossFactor;
                normalizedFuelEconomy = SafeFloat(normalizedFuelEconomy, baseLitresPer100Km);
                fuelUsed = normalizedFuelEconomy * kilometresTravelled / 100f;
            }
            else
            {
                float idleFuelFlowFactor = 2.7777778E-06f;
                float engineDisplacement = vfs.Displacement;
                fuelUsed = idleFuelFlowFactor * engineDisplacement;
                if (v.WheelSpeed() > 3f || v.CurrentRPM() > 0.3f)
                {
                    float rpmfactor = 1f + Math.Abs(v.CurrentRPM() * 5f);
                    fuelUsed *= rpmfactor;
                }
            }

            fuelUsed = SafeFloat(fuelUsed, 0f);
            return Math.Abs(fuelUsed);
        }

        #endregion

        #region Helper Methods for Vehicle & World Interactions

        /// <summary>
        /// Checks if a vehicle is located directly in front of the player within a specified range.
        /// It calculates the player's forward direction and checks for nearby vehicles within a 5.0f radius.
        /// If a vehicle is within 3.0f units of the calculated position in front of the player, it returns the vehicle.
        /// </summary>
        /// <returns>Returns the vehicle in front if found, otherwise returns null.</returns>
        internal static Entity VehicleInFront()
        {
            Vector3 playerPosition = Game.LocalPlayer.Character.Position;
            Rotator playerRotation = Game.LocalPlayer.Character.Rotation;

            Vector3 forwardDirection = GetForwardDirection(playerRotation.Yaw);
            Vector3 offsetPosition = playerPosition + forwardDirection * 2.0f;

            var nearbyEntities = World.GetEntities(playerPosition, 5.0f, GetEntitiesFlags.ConsiderCars);
            var closestEntity = World.GetClosestEntity(nearbyEntities, playerPosition);

            if (closestEntity is Vehicle vehicle && IsWithinRange(offsetPosition, vehicle))
            {
                return vehicle;
            }

            return null;
        }

        /// <summary>
        /// Checks if a vehicle is within 3.0f units of the specified position.
        /// </summary>
        /// <param name="offsetPosition">The position to check against.</param>
        /// <param name="vehicle">The vehicle to check the distance for.</param>
        /// <returns>Returns true if the vehicle is within range, otherwise false.</returns>
        internal static bool IsWithinRange(Vector3 offsetPosition, Vehicle vehicle)
        {
            return Vector3.Distance(offsetPosition, vehicle.Position) < 3.0f;
        }

        /// <summary>
        /// Calculates the forward direction vector based on the player's yaw (rotation angle).
        /// </summary>
        /// <param name="yaw">The yaw angle of the player in degrees.</param>
        /// <returns>Returns a Vector3 representing the forward direction of the player.</returns>
        internal static Vector3 GetForwardDirection(float yaw)
        {
            float radYaw = MathHelper.ConvertDegreesToRadians(yaw);
            return new Vector3((float)Math.Cos(radYaw), (float)Math.Sin(radYaw), 0);
        }

        /// <summary>
        /// Retrieves the position of the vehicle's fuel tank based on predefined bone names.
        /// This method checks for the fuel tank's position by iterating through a list of possible bone names and returning the position of the first valid bone found.
        /// </summary>
        /// <param name="vehicle">The vehicle to get the fuel tank position for.</param>
        /// <returns>The position of the fuel tank in world coordinates, or <see cref="Vector3.Zero"/> if no valid bone is found.</returns>
        internal static Vector3 GetVehicleTankPos(Vehicle vehicle)
        {
            foreach (string boneName in FuelConstants.VehicleFuelTankBones)
            {
                try
                {
                    int boneIndex = vehicle.GetBoneIndex(boneName);
                    if (boneIndex != -1 && vehicle.HasBone(boneIndex))
                    {
                        return vehicle.GetBonePosition(boneIndex);
                    }
                }
                catch (ArgumentException) { }
            }
            return Vector3.Zero;
        }

        #endregion
    }
}
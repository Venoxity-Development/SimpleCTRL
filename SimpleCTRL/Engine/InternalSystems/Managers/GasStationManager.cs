namespace SimpleCTRL.Engine.InternalSystems
{
    internal static class GasStationManager
    {
        #region Initialization

        public static void SetupGasStationBlips()
        {
            GasStations.LoadAllStations();

            foreach (GasStation station in AvailableGasStations)
            {
                bool hasElectricPumps = station.Pumps.PumpList.Exists(pump => pump.Type == "Electric");
                station.CreateBlip(hasElectricPumps);
            }
        }

        #endregion

        #region Prop Spawning

        public static void StartPumpPropManagement()
        {
            GameFiber.StartNew(() => ManagePumpPropSpawning());
        }

        private static void ManagePumpPropSpawning()
        {
            string pumpModel = "prop_gas_pump_old2";
            string electricPumpModel = "wire_pump_prop";

            Model regularPump = new Model(pumpModel);
            Model electricPump = new Model(electricPumpModel);

            N.RequestModel(regularPump);
            N.RequestModel(electricPump);

            while (true)
            {
                var playerPosition = Game.LocalPlayer.Character.Position;

                if (N.HasModelLoaded(regularPump) || N.HasModelLoaded(electricPump))
                {
                    foreach (GasPump pump in AvailableExtendedStations)
                    {
                        if (pump != null)
                        {
                            string currentPumpModel = pump.Type == "Electric" ? electricPumpModel : pumpModel;
                            HandlePumpPropSpawning(pump, currentPumpModel, playerPosition);
                        }

                        GameFiber.Wait(100);
                    }

                    GameFiber.Wait(100);
                }

                GameFiber.Yield();
            }
        }

        private static void HandlePumpPropSpawning(GasPump pump, string pumpModel, Vector3 playerPosition)
        {
            try
            {
                float distSq = playerPosition.DistanceToSquared(pump.Position);
                if (distSq > 50000f) return;

                var modelHash = new Model(pumpModel).Hash;
                if (N.DoesObjectOfTypeExistAtCoords(pump.Position.X, pump.Position.Y, pump.Position.Z, 2f, modelHash)) return;

                if (pump.Type != "Electric")
                {
                    N.RequestCollisionAtCoord(pump.Position.X, pump.Position.Y, 1000f);
                    float groundZ = N.GetGroundZFor3DCoord(pump.Position.X, pump.Position.Y, 1000f, false);
                    pump.Position = new Vector3(pump.Position.X, pump.Position.Y, groundZ);
                }

                Rage.Object obj = new Rage.Object(modelHash, pump.Position);
                ExtendedStationPumpProps.Add(obj);

                NativeFunction.CallByHash<int>(0x8524A8B0171D5E07, obj, 0.0f, 0.0f,
                    MathUtils.DirectionToRotation(MathUtils.HeadingToDirection(pump.Rotation), 0f).Z, 1);

                if (EntityExtensions.Exists(obj))
                {
                    obj.IsInvincible = true;
                    obj.IsPositionFrozen = true;
                    obj.IsExplosionProof = true;
                    obj.IsFireProof = true;
                    obj.IsCollisionProof = true;
                }
                else
                {
                    Logging.Warning($"Failed to spawn pump at {pump.Position} | Type: {pump.Type}", nameof(GasStationManager));
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"Exception while spawning pump at {pump.Position} | Type: {pump.Type}", nameof(GasStationManager), ex);
            }
        }

        #endregion

        #region Cleanup

        public static void DeleteNozzleAndRope()
        {
            try
            {
                DeleteNozzle();
                DeleteRope();
            }
            catch (Exception ex)
            {
                Logging.Error("Failed during nozzle/rope cleanup.", nameof(GasStationManager), ex);
            }
        }

        private static void DeleteNozzle()
        {
            try
            {
                if (EntityExtensions.Exists(FuelNozzle))
                {
                    FuelNozzle.Delete();
                }
            }
            catch (Exception ex)
            {
                Logging.Error("Error deleting fuel nozzle.", nameof(GasStationManager), ex);
            }
        }

        private static void DeleteRope()
        {
            try
            {
                NativeFunction.CallByHash<int>(0x6CE36C35C1AC8163); // ROPE_UNLOAD_TEXTURES
                unsafe
                {
                    fixed (int* pRopeId = &Rope)
                    {
                        if (NativeFunction.Natives.DOES_ROPE_EXIST<bool>((IntPtr)pRopeId))
                        {
                            NativeFunction.Natives.DELETE_ROPE<int>((IntPtr)pRopeId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error("Error deleting rope.", nameof(GasStationManager), ex);
            }
        }

        #endregion

        #region Blip and Prop Cleanup

        internal static void DeleteAllBlips()
        {
            foreach (var blip in GasStationMarkers)
            {
                try
                {
                    if (blip.Exists())
                    {
                        blip.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error($"Failed to delete blip.", nameof(GasStationManager), ex);
                }
            }

            GasStationMarkers.Clear();
        }

        internal static void DeleteAllPumps()
        {
            foreach (var obj in ExtendedStationPumpProps)
            {
                try
                {
                    if (EntityExtensions.Exists(obj))
                    {
                        obj.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error($"Failed to delete pump.", nameof(GasStationManager), ex);
                }
            }

            ExtendedStationPumpProps.Clear();
        }


        #endregion

        #region Gas Station Utilities

        internal static GasPump FindClosestPump(Vehicle vehicle, GasStation gasStation)
        {
            Vector3 fuelTankPos = UtilityHelper.GetVehicleTankPos(vehicle);

            var pumpDistances = gasStation.Pumps.PumpList
                .Select(pump => new { pump, distance = Vector3.DistanceSquared(fuelTankPos, pump.Position) })
                .Where(p => p.distance <= 20f)
                .OrderBy(p => p.distance)
                .FirstOrDefault();

            return pumpDistances?.pump;
        }

        internal static bool IsVehicleNearAnyPump(this Vehicle vehicle)
        {
            Vector3 fuelTankPos = UtilityHelper.GetVehicleTankPos(vehicle);

            return AvailableGasStations
                .Any(gasStation => gasStation.Pumps.PumpList
                    .Any(pump => Vector3.DistanceSquared(pump.Position, fuelTankPos) <= 20f));
        }

        internal static Rage.Object GetClosestPump(Vector3 coords)
        {
            var propHashes = GasPumpProps.Select(prop => Game.GetHashKey(prop)).ToArray();

            return propHashes
                .Select(propHash => NativeFunction.Natives.GET_CLOSEST_OBJECT_OF_TYPE<Rage.Object>(coords.X, coords.Y, coords.Z, 3.0f, propHash, true, true, true))
                .FirstOrDefault(p => p != null);
        }

        #endregion
    }
}

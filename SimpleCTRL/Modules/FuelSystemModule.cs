using SimpleCTRL.Engine.FrontendSystems;

namespace SimpleCTRL.Modules
{
    internal class FuelSystemModule : CommonPlugin
    {
        private static Vehicle vehicle;

        internal static void UpdateFuel()
        {
            Ped player = Game.LocalPlayer.Character;

            Vehicle playerVeh = player.CurrentVehicle;

            if (EntityExtensions.Exists(ClientCurrentVehicle))
            {
                WasDriver = ClientCurrentVehicle.Driver == ClientPed;
            }
            if (MyVehicle != ClientCurrentVehicle)
            {
                VehicleFuelLevelInitialized = false;
            }
            MyVehicle = ClientLastVehicle ?? null;
            int refuelingAllowed;
            if (MyVehicle != null)
            {
                vehicle = MyVehicle;
                if (vehicle != null && (int)vehicle.Class != 13 && !vehicle.IsBoat() && !vehicle.DisplayName().Contains("BLIMP"))
                {
                    if (ClientPed.IsOnFoot)
                    {
                        Vector3 position = ClientPed.Position;
                        if (position.DistanceToSquared(vehicle.Position) <= 15f)
                        {
                            refuelingAllowed = (WasDriver ? 1 : 0);
                            goto IL_0162;
                        }
                    }
                    refuelingAllowed = 0;
                    goto IL_0162;
                }
            }
            goto IL_0328;
            IL_0162:
            RefuelingAllowed = (byte)refuelingAllowed != 0;
            if (!VehicleFuelLevelInitialized)
            {
                vehicle.InitFuel();
            }
            if (playerVeh.IsAircraft())
            {
                if (!AircraftEngineOn && vehicle.IsEngineOn)
                {
                    N.SetVehicleEngineOn(vehicle, false, false, true);
                    AircraftEngineOn = false;
                }
                else if (AircraftEngineOn && vehicle.IsEngineOn)
                {
                    float percentFuel = vehicle.FuelLevel / vehicle.MaxFuelLevel() * 100f;
                    if (vehicle.IsInAir && percentFuel < Settings.AircraftLowFuelWarning)
                    {
                        uint currentGameTime = Game.GameTime;
                        uint elapsedTime = currentGameTime >= LastAircraftLowFuelWarning
                            ? currentGameTime - LastAircraftLowFuelWarning
                            : UInt32.MaxValue - LastAircraftLowFuelWarning + currentGameTime;

                        if (elapsedTime > 5000) // 5000 milliseconds = 5 seconds
                        {
                            AudioHelper.PlayAudio(AudioHelper.Audio.LowFuel);
                            LastAircraftLowFuelWarning = Game.GameTime;
                        }
                    }
                }
                vehicle.ConsumeAircraftFuel();
                if (!vehicle.IsInAir && vehicle.Speed < 2f && !vehicle.IsEngineOn)
                {
                    uint currentGameTime = Game.GameTime;
                    uint elapsedTime = currentGameTime >= LastAircraftEngineHintDisplayed
                        ? currentGameTime - LastAircraftEngineHintDisplayed
                        : UInt32.MaxValue - LastAircraftEngineHintDisplayed + currentGameTime;

                    if (elapsedTime > 30000) // 30000 milliseconds = 30 seconds
                    {
                        Game.DisplayHelp("Press ~INPUT_VEH_FLY_UNDERCARRIAGE~ to start the engine.");
                        LastAircraftEngineHintDisplayed = Game.GameTime;
                    }
                }
                LastAircraftAltitude = vehicle.HeightAboveGround;
            }
            else
            {
                if (vehicle.Model.IsCar && vehicle.PetrolTankHealth() < 700f && vehicle.GetFuelLevel() > 0f)
                {
                    if (vehicle.PetrolTankHealth() < 250f && !vehicle.IsElectric())
                    {
                        vehicle.SetFuelLevel(vehicle.GetFuelLevel() - 0.005f);
                    }
                    vehicle.SetFuelLevel(vehicle.GetFuelLevel() - 0.003f);
                }
                if (vehicle.IsPlayerDriving() || RefuelingAllowed)
                {
                    vehicle.ConsumeRoadVehicleFuel();
                }
            }
            if (RefuelingAllowed || vehicle.IsPlayerDriving())
            {
                if (!N.IsHudHidden() || (player.CurrentVehicle != null && player.CurrentVehicle.IsAircraft()))
                {
                    FuelBarUI.RenderBar(vehicle.FuelLevel, VehicleFuelCapacity, vehicle.IsElectric());
                }
                GasStation gas = GasStation.GetClosestInRange(player.Position, 250f);
                if (gas != null)
                {
                    if (gas != AvailableGasStation)
                    {
                        AvailableGasStation = gas;
                    }
                }
                else if (AvailableGasStation != null)
                {
                    AvailableGasStation = null;
                }
            }
            goto IL_0328;
            IL_0328:
            if (Game.LocalPlayer.Character.IsOnFoot)
            {
                AircraftEngineOn = false;
                ClientPed.ManualRefuel();
                VehicleFuelLevelInitialized = false;
                foreach (int x in TripInfos.Keys)
                {
                    if (TripInfos[x] != null)
                    {
                        try
                        {
                            Vehicle v = World.GetEntityByHandle<Vehicle>(new PoolHandle((uint)x));
                            if (v.Exists() && v.IsEngineOn)
                            {
                                v.ConsumeRoadVehicleFuel();
                            }
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
            }
            LastWorldTime = DateTime.UtcNow;
        }

        public static void Start()
        {
            GameFiber.StartNew(delegate { Run(); });
        }

        public static void Run()
        {
            while (true)
            {
                GameFiber.Yield();
                UpdateFuel();
            }
        }
    }
}
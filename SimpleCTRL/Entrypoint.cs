using SimpleCTRL.Modules;

[assembly: Rage.Attributes.Plugin("SimpleCTRL", Author = "Venoxity Development", PrefersSingleInstance = true, ShouldTickInPauseMenu = true, SupportUrl = "https://discord.gg/jCEdAF8AQz")]

namespace SimpleCTRL
{
    public class EntryPoint : CommonPlugin
    {
        #region Fields
        private static readonly Dictionary<string, DecoratorType> decorators = new()
        {
            { "_Fuel_Level", DecoratorType.Float },
            { "brakeHeat", DecoratorType.Int }
        };
        #endregion

        #region Plugin Lifecycle
        public static void Main()
        {
            Logging.Info("SimpleCTRL plugin lifecycle started.", "EntryPoint");

            DependencyManager.AddDependency("NAudio.dll", "2.2.1");
            DependencyManager.AddDependency("Venoxity.Common.dll", "1.0.9");
            DependencyManager.AddDependency("RageNativeUI.dll", "1.9.3.0");
            if (!DependencyManager.CheckDependencies()) return;

            InitializePlugin();
        }
        #endregion

        #region Plugin Initialization
        private static void InitializePlugin()
        {
            Logging.Info("Initializing SimpleCTRL...", "EntryPoint");

            try
            {
                Settings.Initialize();
                Decorators.Initialize();
                Decorators.Register(decorators);

                GasStationManager.SetupGasStationBlips();
                GasStationManager.StartPumpPropManagement();
                FuelSystemModule.Start();

                VehicleDamageModule.Start();
                VehicleSystemModule.Start();
                KeybindManager.Start();

                LastWorldTime = DateTime.UtcNow;

                Logging.Info("SimpleCTRL successfully initialized.", "EntryPoint");
            }
            catch (Exception ex)
            {
                Logging.Error($"Initialization failed: {ex.Message}", "EntryPoint");
            }
        }
        #endregion

        #region Unload
        public static void OnUnload(bool isTerminating)
        {
            CleanUp();

            Logging.Info("Plugin unloading initiated.", "EntryPoint");
        }
        private static void CleanUp()
        {
            Logging.Info("Cleaning up resources...", "EntryPoint");

            GasStationManager.DeleteAllBlips();
            GasStationManager.DeleteAllPumps();
            GasStationManager.DeleteNozzleAndRope();
        }
        #endregion
    }
}
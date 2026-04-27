using Rage;
using SimpleCTRL.Engine.InternalSystems;
using SimpleCTRL.Engine.FrontendSystems.UI;
using SimpleCTRL.Engine.Helpers;

namespace SimpleCTRL.Handlers
{
    internal static class UIHandler
    {
        private static bool IsRunning = false;

        public static void Start()
        {
            Logging.Info("starting...", "UIHandler");
            GameFiber.StartNew(delegate { Run(); });
        }

        private static void Run()
        {
            IsRunning = true;
            while (IsRunning)
            {
                GameFiber.Yield();
                VehicleUI.Start();
            }
        }
    }
}

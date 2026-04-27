using Rage;
using RAGENativeUI;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SimpleCTRL.Engine.Helpers
{
    internal static class ConversionAndFormattingHelper
    {
        #region String to Integer Conversion
        public static int ToInt32(this string text, [CallerMemberName] string callingMethod = null)
        {
            int i = 0;
            try
            {
                i = Convert.ToInt32(text);
            }
            catch (Exception e)
            {
                Game.LogTrivial(e.Message);
            }
            return i;
        }
        #endregion

        #region Key Binding Formatting
        public static string FormatKeyBinding(ControllerButtons key) => $"{key.GetInstructionalId()}";

        public static string FormatKeyBinding(Keys key) => $"{key.GetInstructionalId()}";
        #endregion
    }
}

using NAudio.Wave;

namespace SimpleCTRL.Engine.InternalSystems
{
    /// <summary>
    /// Provides helper methods for handling audio playback within the DynamicFuel system.
    /// </summary>
    internal static class AudioHelper
    {
        #region Fields

        private static WaveOutEvent output;
        private static AudioFileReader reader;

        #endregion

        #region Methods

        /// <summary>
        /// Plays the specified audio file associated with the given <paramref name="audio"/> type.
        /// </summary>
        /// <param name="audio">The type of audio to be played.</param>
        internal static void PlayAudio(Audio audio)
        {
            string audioFilePath = Path.Combine(Settings.AudioPath, GetAudioFileName(audio));

            try
            {
                DisposeAudio();

                reader = new AudioFileReader(audioFilePath)
                {
                    Volume = N.GetProfileSetting(300) / 20f
                };

                output = new WaveOutEvent();
                output.Init(reader);
                output.Play();
            }
            catch (Exception ex)
            {
                Logging.Error($"Failed to play audio file: {audioFilePath}", "AudioHelper", ex);
            }
        }

        /// <summary>
        /// Disposes the current audio resources to ensure that the next audio file can be played cleanly.
        /// </summary>
        private static void DisposeAudio()
        {
            try
            {
                output?.Stop();
                output?.Dispose();
                output = null;

                reader?.Dispose();
                reader = null;
            }
            catch (Exception ex)
            {
                Logging.Error("Failed to dispose audio resources.", "AudioHelper", ex);
            }
        }

        /// <summary>
        /// Retrieves the file name of the audio file corresponding to the given <paramref name="audio"/> type.
        /// </summary>
        /// <param name="audio">The type of audio for which to get the file name.</param>
        /// <returns>The file name of the corresponding audio file.</returns>
        private static string GetAudioFileName(Audio audio)
        {
            switch (audio)
            {
                case Audio.ChargeStop:
                    return "charge_stop.wav";
                case Audio.Charging:
                    return "charging.wav";
                case Audio.FuelStop:
                    return "fuel_stop.wav";
                case Audio.LowFuel:
                    return "low_fuel.wav";
                case Audio.PickupNozzle:
                    return "pickup_nozzle.wav";
                case Audio.PutBackCharger:
                    return "put_back_charger.wav";
                case Audio.PutBackNozzle:
                    return "put_back_nozzle.wav";
                case Audio.Refuel:
                    return "refuel.wav";
                default:
                    Logging.Warning($"Unknown audio type: {Enum.GetName(typeof(Audio), audio)}", "AudioHelper");
                    return string.Empty;
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Enum representing different types of audio clips used in the DynamicFuel system.
        /// </summary>
        internal enum Audio
        {
            ChargeStop,
            Charging,
            FuelStop,
            LowFuel,
            PickupNozzle,
            PutBackCharger,
            PutBackNozzle,
            Refuel
        }

        #endregion
    }
}
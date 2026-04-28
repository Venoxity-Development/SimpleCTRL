namespace SimpleCTRL.Engine.Data
{
    /// <summary>
    /// Contains information about a trip, including distance traveled, fuel consumed, and fuel economy calculations.
    /// </summary>
    internal class TripInfo
    {
        /// <summary>
        /// Gets or sets the distance traveled in kilometers.
        /// </summary>
        public float DistanceTraveledKM { get; set; }

        /// <summary>
        /// Gets or sets the amount of fuel consumed during the trip (in liters).
        /// </summary>
        public float FuelConsumed { get; set; }

        /// <summary>
        /// Gets or sets the last known position as a <see cref="Vector3"/>. Default is <see cref="Vector3.Zero"/>.
        /// </summary>
        public Vector3 LastPosition { get; set; } = Vector3.Zero;

        /// <summary>
        /// Gets the fuel economy in liters per 100 kilometers.
        /// </summary>
        public float FuelEconomyInLPer100Km => MathUtils.GetFuelEconomyInLPer100Km(FuelConsumed, DistanceTraveledKM);

        /// <summary>
        /// Gets the fuel economy in miles per gallon (MPG), converted from liters per 100 kilometers.
        /// </summary>
        public float FuelEconomyInMPG => MathUtils.ConvertLPer100KmToMPG(FuelEconomyInLPer100Km);

        /// <summary>
        /// Gets the distance traveled in miles, converted from kilometers.
        /// </summary>
        public float DistanceTraveledMiles => MathUtils.ConvertKilometersToMiles(DistanceTraveledKM);

        /// <summary>
        /// Resets the trip information with new position data.
        /// </summary>
        /// <param name="v">The new position to reset the last position to.</param>
        public void Reset(Vector3 v)
        {
            DistanceTraveledKM = 0f;
            FuelConsumed = 0f;
            LastPosition = v;
        }
    }
}
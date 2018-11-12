using System.Globalization;

namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents a location on the map.
    /// </summary>
    public struct Location
    {
        /// <summary>
        /// Gets or sets the latitude coordinate.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Gets or sets the longitude coordinate.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Initializes a new instance of Location.
        /// </summary>
        /// <param name="latitude">Latitude coordinate.</param>
        /// <param name="longitude">Longitude coordinate.</param>
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Returns string representation of Location.
        /// </summary>
        /// <returns>String representation of Location.</returns>
        public override string ToString()
        {
            return $"{Latitude.ToString(CultureInfo.InvariantCulture)},{Longitude.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}
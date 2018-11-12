using System.Xml.Serialization;

namespace VrpTestCasesGenerator.Model
{
    /// <summary>
    /// Represents an address.
    /// </summary>
    [XmlRoot("addressparts")]
    public class Address
    {
        /// <summary>
        /// Gets or sets house number.
        /// </summary>
        [XmlElement("house_number")]
        public string HouseNumber { get; set; }
        /// <summary>
        /// Gets or sets street name.
        /// </summary>
        [XmlElement("road")]
        public string Street { get; set; }
        /// <summary>
        /// Gets or sets city name.
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }
        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        [XmlElement("country")]
        public string Country { get; set; }
        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        [XmlElement("postcode")]
        public string PostCode { get; set; }
    }
}

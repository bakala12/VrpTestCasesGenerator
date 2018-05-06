using System.Xml.Serialization;

namespace VrpTestCasesGenerator.Model
{
    [XmlRoot("addressparts")]
    public class Address
    {
        [XmlElement("house_number")]
        public string HouseNumber { get; set; }

        [XmlElement("road")]
        public string Street { get; set; }

        [XmlElement("city")]
        public string City { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("postcode")]
        public string PostCode { get; set; }
    }
}

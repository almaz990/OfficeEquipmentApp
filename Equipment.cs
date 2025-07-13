using System.Xml.Serialization;

namespace OfficeEquipment
{
    [XmlRoot("Equipment")]
    public class Equipment
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Type")]
        public string Type { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }
    }
}
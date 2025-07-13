using OfficeEquipment;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OfficeEquipment
{
    public static class XmlDataService
    {
        private static readonly string FilePath = "equipment.xml";

        public static List<Equipment> LoadData()
        {
            if (!File.Exists(FilePath))
            {
                return GenerateInitialData();
            }

            try
            {
                var serializer = new XmlSerializer(typeof(List<Equipment>),
                                    new XmlRootAttribute("EquipmentList"));

                using (var reader = new StreamReader(FilePath))
                {
                    return (List<Equipment>)serializer.Deserialize(reader);
                }
            }
            catch
            {
                // Если файл поврежден, создаем новые данные
                return GenerateInitialData();
            }
        }

        public static void SaveData(List<Equipment> data)
        {
            var serializer = new XmlSerializer(typeof(List<Equipment>),
                                new XmlRootAttribute("EquipmentList"));

            using (var writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, data);
            }
        }

        private static List<Equipment> GenerateInitialData()
        {
            return new List<Equipment>
            {
                new Equipment { Id = 1, Name = "Принтер HP LaserJet", Type = "Принтер", Status = "В пользовании" },
                new Equipment { Id = 2, Name = "Сканер Epson Perfection", Type = "Сканер", Status = "На складе" },
                new Equipment { Id = 3, Name = "Монитор Dell 24\"", Type = "Монитор", Status = "На ремонте" },
                new Equipment { Id = 4, Name = "Принтер Canon PIXMA", Type = "Принтер", Status = "На складе" },
                new Equipment { Id = 5, Name = "Монитор LG 27\"", Type = "Монитор", Status = "В пользовании" }
            };
        }
    }
}
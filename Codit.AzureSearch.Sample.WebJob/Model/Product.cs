using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Codit.AzureSearch.Sample.WebJob.Model
{
    [XmlRoot]
    public class Product
    {
        [XmlElement]
        public string ProductId { get; set; }
        [XmlElement]
        public string Title { get; set; }
        [XmlElement]
        public string Category { get; set; }
        [XmlElement]
        public string Description { get; set; }
        [XmlElement]
        public string ReleaseDate { get; set; }
        [XmlElement]
        public bool IsPromotion { get; set; }
        [XmlElement]
        public double Price { get; set; }
    }
}
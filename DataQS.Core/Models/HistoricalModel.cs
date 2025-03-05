using LiteDB;


namespace DataQS.Core.Models
{
    public record HistoricalModel
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime AnalisysDateTime { get; set; }
        public string CompactedDataModels { get; set; }
        public string CompactedAnalisys { get; set; }
        public DataModelInformation DataModelInformation { get; set; }
    }
    public class HistoricalSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

using LiteDB;


namespace DataQS.Core.Models
{
    public record StationModel
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Height { get; set; }
        public float MaxTemperature { get; set; }
        public float MinTemperature { get; set; }
        public float MaxRainfall { get; set; } // Precipitação
    }
}

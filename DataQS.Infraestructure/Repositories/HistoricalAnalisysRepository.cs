using DataQS.Core.Models;
using LiteDB;
using System.IO.Compression;
using System.Text;

namespace DataQS.Infraestructure.Repositories
{
    public class HistoricalAnalisysRepository
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<HistoricalModel> _historicalCollection;

        public HistoricalAnalisysRepository(LiteDatabase database)
        {
            // Inicializar o banco de dados LiteDB
            _database = database;

            // Obter a coleção de dados "stations"
            _historicalCollection = _database.GetCollection<HistoricalModel>("historical");
            //_historicalCollection.DeleteAll();
            // Verificar se a coleção está vazia e inserir dados padrão
        }


        // Método para adicionar uma nova estação
        public void AddHistorical(string historicalName, IEnumerable<DataModel> dataModels, Dictionary<string, string[]> dataAnalisys, DataModelInformation dataModelInformation)
        {
            HistoricalModel historicalModel = new()
            {
                Name = historicalName,
                AnalisysDateTime = DateTime.Now,
                CompactedAnalisys =Compress( System.Text.Json.JsonSerializer.Serialize(dataAnalisys)),
                CompactedDataModels =Compress( System.Text.Json.JsonSerializer.Serialize(dataModels)),
                DataModelInformation = dataModelInformation
            };
            _historicalCollection.Insert(historicalModel);
        }

        // Método para obter todas as estações
        public List<HistoricalSummary> GetHistoricals()
        {
            return _historicalCollection
                .FindAll()
                .OrderByDescending(i => i.AnalisysDateTime)
                .Select(i => new HistoricalSummary { Id = i.Id, Name = i.Name })
                .ToList();
        }
        public (string HistoricalName, IEnumerable<DataModel> DataModels, Dictionary<string, string[]> DataAnalisys, DataModelInformation dataModelInformation) GetHistorical(int id)
        {
            var historical = (_historicalCollection.Find(i => i.Id == id)?.FirstOrDefault()) ?? throw new ArgumentException($"Histórico com ID {id} não encontrado.");

            var dataModels = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<DataModel>>(
                Decompress(historical.CompactedDataModels)
            );

            var dataAnalisys = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(
                Decompress(historical.CompactedAnalisys)
            );

            var dataInformation = historical.DataModelInformation;

            return (historical.Name, dataModels, dataAnalisys, dataInformation);
        }


        // Método para atualizar uma estação existente
        public bool UpdateStation(HistoricalModel historical)
        {
            return _historicalCollection.Update(historical);
        }

        
        public void DeleteAllHistoricalData()
        {
            _ = _historicalCollection.DeleteAll();
        }

        private static string Decompress(string compressedData)
        {
            var bytes = Convert.FromBase64String(compressedData);

            using var input = new MemoryStream(bytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            {
                gzip.CopyTo(output);
            }

            return Encoding.UTF8.GetString(output.ToArray());
        }
        private static string Compress(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);

            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }

            return Convert.ToBase64String(output.ToArray()); // Armazena em Base64
        }
    }
}

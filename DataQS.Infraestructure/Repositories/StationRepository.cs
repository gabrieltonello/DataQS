using DataQS.Core.Models;
using LiteDB;

namespace DataQS.Infraestructure.Repositories
{
    public class StationRepository
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<StationModel> _stationCollection;

        public StationRepository(LiteDatabase database)
        {
            // Inicializar o banco de dados LiteDB
            _database = database;

            // Obter a coleção de dados "stations"
            _stationCollection = _database.GetCollection<StationModel>("stations");
            _stationCollection.DeleteAll();
            // Verificar se a coleção está vazia e inserir dados padrão
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (_stationCollection.Count() == 0)
            {
                var defaultStations = new List<StationModel>
                {
                    new () { Name = "Belo Jardim (BJD)", Latitude = -8.3675f, Longitude = -36.4294f, Height = 718, MaxTemperature = 35.9f, MinTemperature = 11.6f, MaxRainfall = 108.6f },
                    new () { Name = "Brasília (BRB)", Latitude = -15.60083f, Longitude = -47.71306f, Height = 1023, MaxTemperature = 35.5f, MinTemperature = 1.6f, MaxRainfall = 132.8f },
                    new () { Name = "Cachoeira Paulista (CPA)", Latitude = -22.6896f, Longitude = -45.0062f, Height = 574, MaxTemperature = 37.5f, MinTemperature = 1.2f, MaxRainfall = 117f },
                    new () { Name = "Caicó (CAI)", Latitude = -6.4669f, Longitude = -37.0847f, Height = 176, MaxTemperature = 39.7f, MinTemperature = 13.2f, MaxRainfall = 142.8f },
                    new () { Name = "Campo Grande (CGR)", Latitude = -20.4383f, Longitude = -54.5383f, Height = 677, MaxTemperature = 39.7f, MinTemperature = -0.9f, MaxRainfall = 115.1f },
                    new () { Name = "Cuiabá (CBA)", Latitude = -15.5553f, Longitude = -56.0700f, Height = 185, MaxTemperature = 41.1f, MinTemperature = 3.3f, MaxRainfall = 124.6f },
                    new () { Name = "Ourinhos (ORN)", Latitude = -22.9486f, Longitude = -49.8942f, Height = 446, MaxTemperature = 40f, MinTemperature = -1.8f, MaxRainfall = 271.2f },
                    new () { Name = "Palmas (PMA)", Latitude = -10.1778f, Longitude = -48.3619f, Height = 216, MaxTemperature = 40.1f, MinTemperature = 11.4f, MaxRainfall = 120f },
                    new () { Name = "Petrolina (PTR)", Latitude = -9.0689f, Longitude = -40.3197f, Height = 387, MaxTemperature = 44.1f, MinTemperature = 12.4f, MaxRainfall = 151.3f },
                    new () { Name = "Rolim de Moura (RLM)", Latitude = -11.58166f, Longitude = -61.77361f, Height = 252, MaxTemperature = 41f, MinTemperature = 4f, MaxRainfall = 194f },
                    new () { Name = "São Luiz (SLZ)", Latitude = -2.5933f, Longitude = -44.2122f, Height = 40, MaxTemperature = 36f, MinTemperature = 13.1f, MaxRainfall = 210f },
                    new () { Name = "São João do Cariri (SCR)", Latitude = -7.3817f, Longitude = -36.5272f, Height = 486, MaxTemperature = 34f, MinTemperature = 13.2f, MaxRainfall = 105f },
                    new () { Name = "São Martinho da Serra (SMS)", Latitude = -29.4428f, Longitude = -53.8231f, Height = 489, MaxTemperature = 39.5f, MinTemperature = -2.4f, MaxRainfall = 103.8f },
                    new () { Name = "Triunfo (TRI)", Latitude = -7.8272f, Longitude = -38.1222f, Height = 1123, MaxTemperature = 33.2f, MinTemperature = 6.8f, MaxRainfall = 126.8f },
                    new () { Name = "Chapecó (CHP)", Latitude = -27.0800f, Longitude = -52.6144f, Height = 700, MaxTemperature = 37.2f, MinTemperature = -4.4f, MaxRainfall = 146.7f },
                    new () { Name = "Curitiba (CTB)", Latitude = -25.495444f, Longitude = -49.331208f, Height = 891, MaxTemperature = 34.8f, MinTemperature = -5.4f, MaxRainfall = 104.6f },
                    new () { Name = "Florianópolis (FLN)", Latitude = -27.6017f, Longitude = -58.5178f, Height = 31, MaxTemperature = 38.8f, MinTemperature = 0.7f, MaxRainfall = 187.1f },
                    new () { Name = "Joinville (JOI)", Latitude = -26.2525f, Longitude = -48.8578f, Height = 48, MaxTemperature = 41.7f, MinTemperature = -3f, MaxRainfall = 160.4f },
                    new () { Name = "Natal (NAT)", Latitude = -5.8367f, Longitude = -35.2064f, Height = 58, MaxTemperature = 33.8f, MinTemperature = 10.6f, MaxRainfall = 168.4f },
                    new () { Name = "Sombrio (SBR)", Latitude = -29.0956f, Longitude = -49.8133f, Height = 15, MaxTemperature = 39.3f, MinTemperature = -0.1f, MaxRainfall = 112.4f }
                };
                _stationCollection.InsertBulk(defaultStations);
            }
        }

        // Método para adicionar uma nova estação
        public void AddStation(StationModel station)
        {
            _stationCollection.Insert(station);
        }

        // Método para obter todas as estações
        public List<StationModel> GetStations()
        {
            return [.. _stationCollection.FindAll().OrderBy(i => i.Name)];
        }

        // Método para atualizar uma estação existente
        public bool UpdateStation(StationModel station)
        {
            return _stationCollection.Update(station);
        }

        // Método para deletar uma estação
        public bool DeleteStation(int id)
        {
            return _stationCollection.Delete(id);
        }
    }
}

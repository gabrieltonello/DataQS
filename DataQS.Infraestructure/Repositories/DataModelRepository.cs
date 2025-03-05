using DataQS.Core.Models;
using LiteDB;

namespace DataQS.Infraestructure.Repositories
{
    public class DataModelRepository
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<DataModelInformation> _DataModelCollection;

        public DataModelRepository(LiteDatabase database)
        {
            _database = database;

            _DataModelCollection = _database.GetCollection<DataModelInformation>("data_model");
            _DataModelCollection.DeleteAll();

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (_DataModelCollection.Count() == 0)
            {
                List<DataModelInformation> defaultStations =
                    [
                    DataModelBuilder.BuildDataModelInformation("Dados SONDA - Dados Ambientais antes da vs3.3", ";",
                        [
                            "id","year","day", "datetm" , "min", "glo_avg" ,"dir_avg" ,"diff_avg"    ,"lw_avg"  ,"par_avg" ,"lux_avg" ,"tp_sfc"  ,"humid"   ,"press"   ,"rain"    ,"ws_10m"  ,"wd_10m"
                        ]),               
                    DataModelBuilder.BuildDataModelInformation("Dados SONDA - Dados Ambientais após a vs3.3", ";",
                        [
                            "id","year","day","min","glo_avg","dir_avg","diff_avg","lw_avg","par_avg","lux_avg","tp_sfc","humid","press","rain","ws_10m","wd_10m"
                        ]),   
                    DataModelBuilder.BuildDataModelInformation("Dados SONDA - Dados Anemométricos", ";",
                        [
                            "id", "year", "day", "datetm", "min", "ws_25", "wd_25", "tp_25", "ws_50", "wd_50", "tp_50"                   
                        ]),                    
                ];
                _DataModelCollection.InsertBulk(defaultStations);
            }
        }

        // Método para adicionar uma nova estação
        public void AddStation(DataModelInformation dataModelInformation)
        {
            _DataModelCollection.Insert(dataModelInformation);
        }

        // Método para obter todas as estações
        public List<DataModelInformation> GetDataModelInformation()
        {
            return _DataModelCollection.FindAll().ToList();
        }

        // Método para atualizar uma estação existente
        public bool UpdateStation(DataModelInformation dataModelInformation)
        {
            return _DataModelCollection.Update(dataModelInformation);
        }

        // Método para deletar uma estação
        public bool DeleteStation(int id)
        {
            return _DataModelCollection.Delete(id);
        }
    }
}

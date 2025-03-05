namespace UnitTests
{
    using DataQS.Core.Models;
    using DataQS.Core.Services;
    using DataQS.Infraestructure.Repositories;
    using LiteDB;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class UnitTest1
    {
        private string[] _AmbientalDataLines;
        private string[] _AmbientalDataValidatorLines;
        private List<DataModel> _datamodels = [];
        private DataModelRepository _modelRepository;
        private StationRepository _stationRepository;

        public UnitTest1()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDirectory, "Files", "CHP0904ED.csv");
            string filePathValidator = Path.Combine(baseDirectory, "Files", "CHP0904ED_DQC.csv"); 
            
            string delimiter = ";";
            _AmbientalDataLines = File.ReadAllLines(filePath);
            _AmbientalDataValidatorLines = File.ReadAllLines(filePathValidator);

            _datamodels = _AmbientalDataLines.GetDataModels(null, delimiter).ToList();

            var databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataQS_LiteDB.db");
            var liteDatabase = new LiteDatabase($"Filename={databasePath};Mode=Shared");

            _modelRepository = new(liteDatabase);
            _stationRepository = new(liteDatabase);
        }

        [Fact]
        public void Pressao()
        {
            List<string> errors = [];

            var data = _datamodels.Select(i => i.Press).ToArray();
            var vpress = ValidatorService.Pressao(data, 1100, 500);
            vpress = ValidatorService.Pressao2(vpress, data);

            string[] vwDirString = vpress.Select(i => string.Join("", i)).ToArray();
            string delimiter = ";";
            Dictionary<string, int> dict = vwDirString
                .GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var item in dict)
            {
                Console.WriteLine($"{item.Key}:\t{item.Value}\t{item.Value/vpress.Count}");
            }

            for (int i = 0; i < vpress.Count; i++)
            {
                string currentValidator = vwDirString[i];
                string sondaValidator = _AmbientalDataValidatorLines[i].Split(delimiter)[13];
                if (currentValidator != sondaValidator)
                {
                    errors.Add($"line {i} is incorrect. Our validator: {currentValidator} vs Sonda Validator: {sondaValidator}");
                }
            }
        }
        [Fact]
        public void Wd10m()
        {
            List<string> errors = new();
            var data = _datamodels.Select(i => i.Wd10m).ToArray();
            var vWdDir = ValidatorService.WindDir(data);
            vWdDir = ValidatorService.WindDir2(vWdDir, data);
            vWdDir = ValidatorService.WindDir3(vWdDir, data);

            string[] vwDirString = vWdDir.Select(i => string.Join("", i)).ToArray();

            Dictionary<string, int> dict = vwDirString
                .GroupBy(x => x)
                .ToDictionary(g => g.Key, g => g.Count());
            string delimiter = ";";

            for (int i = 0; i < vWdDir.Count; i++) 
            {
                string currentValidator = vwDirString[i];
                string sondaValidator = "";
                try
                {
                    sondaValidator = _AmbientalDataValidatorLines[i].Split(delimiter)[16];
                }
                catch
                {

                }
                if (currentValidator != sondaValidator)
                {
                    errors.Add($"line {i} is incorrect. Our validator: {currentValidator} vs Sonda Validator: {sondaValidator}");
                }
            }
        }      
        [Fact]
        public void Todos()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(baseDirectory, "Files", "CHP0904ED.csv");
            var station = _stationRepository.GetStations().Where(i => i.Name == "Chapecó (CHP)").FirstOrDefault();
            var model = _modelRepository.GetDataModelInformation().Where(i => i.Name == "Dados SONDA - Dados Ambientais antes da vs3.3").FirstOrDefault();

            FileService.StartQualityProcess(filePath, model, station);
        }      
        
        [Fact]
        public void BuildDataModelInformation()
        {
            var build = DataModelBuilder.BuildDataModelInformation("Name", ";",
                [
                "id","year","day", "datetm" , "min", "glo_avg" ,"dir_avg" ,"diff_avg"    ,"lw_avg"  ,"par_avg" ,"lux_avg" ,"tp_sfc"  ,"humid"   ,"press"   ,"rain"    ,"ws_10m"  ,"wd_10m"
                ]);

            Assert.True(build.Year == 2);
            Assert.True(build.Day == 3);
            Assert.True(build.DateTm == 4);
            Assert.True(build.DirAvg == 6);
            Assert.True(build.Ws50 == -1);

        }
    }
}
using DataQS.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataQS.Core.Services
{
    public static class FileService
    {
        public static List<DataModel> GetDataModels(this string[] data, DataModelInformation dataModelInformation, string delimiter)
        {
            // Get the right culture to parse decimal delimiter
            CultureInfo culture = data[0].Contains(',') ? new CultureInfo("pt-BR") : CultureInfo.InvariantCulture;

            List<DataModel> models = new();
            foreach (var line in data)
            {
                culture ??= data[0].Contains(',') ? new CultureInfo("pt-BR") : CultureInfo.InvariantCulture;

                var lineSplit = line.Split(delimiter);

                double.TryParse(dataModelInformation.Press == -1 ? "0.0" : lineSplit[dataModelInformation.Press], culture, out double Press);
                double.TryParse(dataModelInformation.Wd10m == -1 ? "0.0" : lineSplit[dataModelInformation.Wd10m], culture, out double Wd10m);
                double.TryParse(dataModelInformation.GloAvg == -1 ? "0.0" : lineSplit[dataModelInformation.GloAvg], culture, out double GloAvg);
                double.TryParse(dataModelInformation.DirAvg == -1 ? "0.0" : lineSplit[dataModelInformation.DirAvg], culture, out double DirAvg);
                double.TryParse(dataModelInformation.DiffAvg == -1 ? "0.0" : lineSplit[dataModelInformation.DiffAvg], culture, out double DiffAvg);
                double.TryParse(dataModelInformation.LwAvg == -1 ? "0.0" : lineSplit[dataModelInformation.LwAvg], culture, out double LwAvg);
                double.TryParse(dataModelInformation.ParAvg == -1 ? "0.0" : lineSplit[dataModelInformation.ParAvg], culture, out double ParAvg);
                double.TryParse(dataModelInformation.LuxAvg == -1 ? "0.0" : lineSplit[dataModelInformation.LuxAvg], culture, out double LuxAvg);
                double.TryParse(dataModelInformation.TpSfc == -1 ? "0.0" : lineSplit[dataModelInformation.TpSfc], culture, out double TpSfc);
                double.TryParse(dataModelInformation.Humid == -1 ? "0.0" : lineSplit[dataModelInformation.Humid], culture, out double Humid);
                double.TryParse(dataModelInformation.Rain == -1 ? "0.0" : lineSplit[dataModelInformation.Rain], culture, out double Rain);
                double.TryParse(dataModelInformation.Ws10m == -1 ? "0.0" : lineSplit[dataModelInformation.Ws10m], culture, out double Ws10m);
                double.TryParse(dataModelInformation.Ws25 == -1 ? "0.0" : lineSplit[dataModelInformation.Ws25], culture, out double Ws25);
                double.TryParse(dataModelInformation.Ws50 == -1 ? "0.0" : lineSplit[dataModelInformation.Ws50], culture, out double Ws50);
                double.TryParse(dataModelInformation.Tp25 == -1 ? "0.0" : lineSplit[dataModelInformation.Tp25], culture, out double Tp25);
                double.TryParse(dataModelInformation.Tp50 == -1 ? "0.0" : lineSplit[dataModelInformation.Tp50], culture, out double Tp50);


                models.Add(new()
                {
                    Year = int.Parse(lineSplit[dataModelInformation.Year]),
                    Day = int.Parse(lineSplit[dataModelInformation.Day]),
                    Min = int.Parse(lineSplit[dataModelInformation.Min]),
                    Press = Press,
                    Wd10m = Wd10m,
                    GloAvg = GloAvg,
                    DirAvg = DirAvg,
                    DiffAvg = DiffAvg,
                    LwAvg = LwAvg,
                    ParAvg = ParAvg,
                    LuxAvg = LuxAvg,
                    TpSfc = TpSfc,
                    Humid = Humid,
                    Rain = Rain,
                    Ws10m = Ws10m,
                    Ws25 = Ws25,
                    Ws50 = Ws50,
                    Tp25 = Tp25,
                    Tp50 = Tp50,

                });
            }

            return models;
        }

        public static double[] GetVariableData(IEnumerable<DataModel> dataModels, string variable)
        {
            return variable switch
            {
                "glo_rad" => dataModels.Select(i => i.GloAvg).ToArray(),
                "dif_rad" => dataModels.Select(i => i.DiffAvg).ToArray(),
                "lw_rad" => dataModels.Select(i => i.LwAvg).ToArray(),
                "dni_rad" => dataModels.Select(i => i.DirAvg).ToArray(),
                "par_rad" => dataModels.Select(i => i.ParAvg).ToArray(),
                "lux_rad" => dataModels.Select(i => i.LuxAvg).ToArray(),
                "press" => dataModels.Select(i => i.Press).ToArray(),
                "rain" => dataModels.Select(i => i.Rain).ToArray(),
                "humid" => dataModels.Select(i => i.Humid).ToArray(),
                "wdir" => dataModels.Select(i => i.Wd10m).ToArray(),
                "wspe" => dataModels.Select(i => i.Ws10m).ToArray(),
                "temp" => dataModels.Select(i => i.TpSfc).ToArray(),
                _ => []
            };
        }
        public static async Task ExportQualityFile(string filePath,
        DataModelInformation dataModelInformation,
        IEnumerable<DataModel> dataModels,
        Dictionary<string, string[]> dataAnalisys)
        {
            // Obter as propriedades de DataModel e DataModelInformation
            var dataProperties = typeof(DataModel).GetProperties();
            var configProperties = typeof(DataModelInformation).GetProperties();

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                bool headerWritten = false;

                foreach (var data in dataModels)
                {
                    var exportData = new Dictionary<string, object>();

                    foreach (var dataProp in dataProperties)
                    {
                        if (dataProp.Name == "Id" || dataProp.Name == "DateTm") continue; // Ignorar os campos Id e DateTm

                        var configProp = configProperties.FirstOrDefault(p => p.Name == dataProp.Name);
                        if (configProp != null)
                        {
                            var configValue = configProp.GetValue(dataModelInformation);

                            if (configValue is int intValue && intValue != -1)
                            {
                                exportData[dataProp.Name] = dataProp.GetValue(data);
                            }
                            else if (configValue is double doubleValue && doubleValue != -1)
                            {
                                exportData[dataProp.Name] = dataProp.GetValue(data);
                            }
                        }
                    }

                    if (!headerWritten)
                    {
                        var headers = exportData.Keys.ToList();

                        // Adicionar os cabeçalhos do dicionário de análise como colunas com prefixo "Quality_"
                        if (dataAnalisys != null && dataAnalisys.Count > 0)
                        {
                            headers.AddRange(dataAnalisys.Keys.Select(key => $"quality_{key}"));
                        }

                        await writer.WriteLineAsync(string.Join(",", headers));
                        headerWritten = true;
                    }

                    var rowValues = exportData.Values.Select(v => v?.ToString()).ToList();

                    // Adicionar os valores do dicionário de análise
                    if (dataAnalisys != null && dataAnalisys.Count > 0)
                    {
                        foreach (var key in dataAnalisys.Keys)
                        {
                            // Adicionar os valores correspondentes, ou vazio se não houver correspondência
                            rowValues.Add(dataAnalisys[key].FirstOrDefault() ?? string.Empty);
                        }
                    }

                    await writer.WriteLineAsync(string.Join(",", rowValues));
                }
            }
        }

        public static (IEnumerable<DataModel> DataModels, Dictionary<string, string[]> Analisys) StartQualityProcess(string filePath, DataModelInformation dataModelInformation, StationModel stationModel)
        {
            // Popular os dados com o model em lista
            var models = GetDataModels(File.ReadAllLines(filePath), dataModelInformation, dataModelInformation.Delimiter);

            return (models, new()
            {
                {"glo_rad", ValidateGloRad(models, dataModelInformation, stationModel) },
                {"dif_rad", ValidateDifRad(models, dataModelInformation, stationModel) },
                {"lw_rad", ValidatLwRad(models, dataModelInformation, stationModel) }  ,
                {"dni_rad", ValidateDniRad(models, dataModelInformation, stationModel) },
                {"par_rad", ValidateParRad(models, dataModelInformation, stationModel) },
                {"lux_rad", ValidateLuxRad(models, dataModelInformation, stationModel) },
                {"press", ValidatePress(models, dataModelInformation, stationModel) } ,
                {"rain", ValidateRain(models, dataModelInformation, stationModel) }  ,
                {"humid", ValidateUmi(models, dataModelInformation, stationModel) }   ,
                {"wdir",  ValidateHdir(models, dataModelInformation, stationModel) } ,
                {"wspe", ValidateWspe(models, dataModelInformation, stationModel) }  ,
                {"temp", ValidateTp(models, dataModelInformation, stationModel) }
            });
        }
        private static string[] ValidateGloRad(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (new List<int>
            {
                dataModelInformation.DiffAvg,
                dataModelInformation.Day,
                dataModelInformation.Min,

            }.Any(i => i == -1))
            {
                return []; // cannot validate without this values above.
            }

            List<string> validations = [];
            if (dataModelInformation.DirAvg == -1 || dataModelInformation.GloAvg == -1) // glo rad incomplete
            {
                foreach (var value in data)
                {
                    var validation = ValidatorService.GloRadIncomplete(value.GloAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                    validations.Add(string.Join("", validation));
                }
            }
            else
            {
                foreach (var value in data) // glo rad complete
                {
                    var validation = ValidatorService.GloRadComplete(value.GloAvg, value.DirAvg, value.DiffAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                    validations.Add(string.Join("", validation));
                }
            }

            return validations.ToArray();
        }
        private static string[] ValidateDifRad(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (new List<int>
            {
                dataModelInformation.DiffAvg,
                dataModelInformation.Day,
                dataModelInformation.Min,

            }.Any(i => i == -1))
            {
                return []; // cannot validate without this values above.
            }

            List<string> validations = [];
            if (dataModelInformation.DirAvg == -1 || dataModelInformation.GloAvg == -1) // glo rad incomplete
            {
                foreach (var value in data)
                {
                    var validation = ValidatorService.DifRadIncomplete(value.DiffAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                    validations.Add(string.Join("", validation));
                }
            }
            else
            {
                foreach (var value in data) // glo rad complete
                {
                    var validation = ValidatorService.DifRadComplete(value.GloAvg, value.DirAvg, value.DiffAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                    validations.Add(string.Join("", validation));
                }
            }

            return validations.ToArray();
        }
        private static string[] ValidatLwRad(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (new List<int>
            {
                dataModelInformation.LwAvg

            }.Any(i => i == -1))
            {
                return []; // cannot validate without this values above.
            }

            List<string> validations = [];
            if (dataModelInformation.TpSfc == -1) // incomplete
            {
                foreach (var value in data)
                {
                    var validation = ValidatorService.LwRadIncomplete(value.LwAvg);
                    validations.Add(string.Join("", validation));
                }
            }
            else
            {
                foreach (var value in data)
                {
                    var validation = ValidatorService.LwRadComplete(value.LwAvg, value.TpSfc);
                    validations.Add(string.Join("", validation));
                }
            }

            return validations.ToArray();
        }
        private static string[] ValidateDniRad(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (new List<int>
            {
                dataModelInformation.DiffAvg,
                dataModelInformation.Day,
                dataModelInformation.Min,

            }.Any(i => i == -1))
            {
                return []; // cannot validate without this values above.
            }

            List<string> validations = [];
            if (dataModelInformation.DirAvg == -1 || dataModelInformation.GloAvg == -1) // glo rad incomplete
            {
                foreach (var value in data)
                {
                    var validation = ValidatorService.DniRadIncomplete(value.DiffAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                    validations.Add(string.Join("", validation));
                }
            }
            else
            {
                foreach (var value in data) // glo rad complete
                {
                    var validation = ValidatorService.DniRadComplete(value.GloAvg, value.DirAvg, value.DiffAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                    validations.Add(string.Join("", validation));
                }
            }

            return validations.ToArray();
        }
        private static string[] ValidateParRad(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (new List<int>
            {
                dataModelInformation.ParAvg
            }.Any(i => i == -1))
            {
                return []; // cannot validate without this values above.
            }

            List<string> validations = [];

            foreach (var value in data)
            {
                var validation = ValidatorService.ParRad(value.ParAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                validations.Add(string.Join("", validation));
            }

            return validations.ToArray();
        }
        private static string[] ValidateLuxRad(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (new List<int>
            {
                dataModelInformation.LuxAvg
            }.Any(i => i == -1))
            {
                return []; // cannot validate without this values above.
            }

            List<string> validations = [];

            foreach (var value in data)
            {
                var validation = ValidatorService.LuxRad(value.LuxAvg, value.Day, station.Latitude, station.Longitude, value.Min);
                validations.Add(string.Join("", validation));
            }

            return validations.ToArray();
        }
        private static string[] ValidatePress(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (dataModelInformation.Rain == -1)
            {
                return []; // cannot validate without this value.
            }

            var pressData = data.Select(i => i.Press).ToArray();
            var validations = ValidatorService.Pressao(pressData, 1100, 500);
            validations = ValidatorService.Pressao2(validations, pressData);

            return validations.Select(i => string.Join("", i)).ToArray();
        }
        private static string[] ValidateRain(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (dataModelInformation.Rain == -1)
            {
                return []; // cannot validate without this value.
            }

            var rainData = data.Select(i => i.Rain).ToArray();

            var validations = ValidatorService.Rain1(rainData, station.MaxRainfall);
            validations = ValidatorService.Rain2(validations, rainData);
            validations = ValidatorService.Rain3(validations, rainData);

            return validations.Select(i => string.Join("", i)).ToArray();
        }
        private static string[] ValidateUmi(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (dataModelInformation.Humid == -1)
            {
                return []; // cannot validate without this value.
            }
            var umiData = data.Select(i => i.Humid).ToArray();

            var validations = ValidatorService.Umidade(umiData);
            validations = ValidatorService.Umidade2(validations, umiData);
            validations = ValidatorService.Rain3(validations, umiData);

            return validations.Select(i => string.Join("", i)).ToArray();
        }
        private static string[] ValidateHdir(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (dataModelInformation.Wd10m == -1)
            {
                return []; // cannot validate without this value.
            }

            var wDirData = data.Select(i => i.Wd10m).ToArray();

            var validations = ValidatorService.WindDir(wDirData);
            validations = ValidatorService.WindDir2(validations, wDirData);
            validations = ValidatorService.WindDir3(validations, wDirData);

            return validations.Select(i => string.Join("", i)).ToArray();
        }
        private static string[] ValidateWspe(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (dataModelInformation.Ws10m == -1)
            {
                return []; // cannot validate without this value.
            }

            var wspeData = data.Select(i => i.Ws10m).ToArray();

            var validations = ValidatorService.WindSp(wspeData);
            validations = ValidatorService.WindSp2(validations, wspeData);
            validations = ValidatorService.WindSp3(validations, wspeData);

            return validations.Select(i => string.Join("", i)).ToArray();
        }
        private static string[] ValidateTp(IEnumerable<DataModel> data, DataModelInformation dataModelInformation, StationModel station)
        {
            if (dataModelInformation.TpSfc == -1)
            {
                return []; // cannot validate without this value.
            }

            var tpData = data.Select(i => i.TpSfc).ToArray();

            var validations = ValidatorService.Temp1(tpData, station.MaxTemperature, station.MinTemperature);
            validations = ValidatorService.Temp2(validations, tpData);
            validations = ValidatorService.Temp3(validations, tpData);

            return validations.Select(i => string.Join("", i)).ToArray();
        }
    }
}

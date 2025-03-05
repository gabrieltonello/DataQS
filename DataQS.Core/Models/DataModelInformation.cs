using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataQS.Core.Models
{
    public record DataModelInformation
    {
        /// <summary>
        /// Unique identifier for the data entry.
        /// </summary>
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier for the data entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique identifier for the data entry.
        /// </summary>
        public string Delimiter { get; set; }

        /// <summary>
        /// The year of the recorded data.
        /// </summary>
        public int Year { get; set; } = -1;

        /// <summary>
        /// The day of the year for the recorded data.
        /// </summary>
        public int Day { get; set; } = -1;

        /// <summary>
        /// The day of the year for the recorded data.
        /// </summary>
        public int DateTm { get; set; } = -1;

        /// <summary>
        /// The minute of the day for the recorded data.
        /// </summary>
        public int Min { get; set; } = -1;

        /// <summary>
        /// Global average radiation measurement.
        /// </summary>
        public int GloAvg { get; set; } = -1;

        /// <summary>
        /// Average direct radiation measurement.
        /// </summary>
        public int DirAvg { get; set; } = -1;

        /// <summary>
        /// Average diffuse radiation measurement.
        /// </summary>
        public int DiffAvg { get; set; } = -1;

        /// <summary>
        /// Longwave radiation average measurement.
        /// </summary>
        public int LwAvg { get; set; } = -1;

        /// <summary>
        /// Photosynthetically Active Radiation (PAR) average measurement.
        /// </summary>
        public int ParAvg { get; set; } = -1;

        /// <summary>
        /// Average illuminance (lux) measurement.
        /// </summary>
        public int LuxAvg { get; set; } = -1;

        /// <summary>
        /// Surface temperature measurement.
        /// </summary>
        public int TpSfc { get; set; } = -1;

        /// <summary>
        /// Humidity percentage measurement.
        /// </summary>
        public int Humid { get; set; } = -1;

        /// <summary>
        /// Atmospheric pressure measurement.
        /// </summary>
        public int Press { get; set; } = -1;

        /// <summary>
        /// Precipitation measurement.
        /// </summary>
        public int Rain { get; set; } = -1;

        /// <summary>
        /// Wind speed measured at 10 meters above ground level.
        /// </summary>
        public int Ws10m { get; set; } = -1;

        /// <summary>
        /// Wind direction measured at 10 meters above ground level.
        /// </summary>
        public int Wd10m { get; set; } = -1;

        /// <summary>
        /// Wind speed measured at 25 meters above ground level.
        /// </summary>
        public int Ws25 { get; set; } = -1;

        /// <summary>
        /// Wind direction measured at 25 meters above ground level.
        /// </summary>
        public int Wd25 { get; set; } = -1;

        /// <summary>
        /// Temperature measured at 25 meters above ground level.
        /// </summary>
        public int Tp25 { get; set; } = -1;

        /// <summary>
        /// Wind speed measured at 50 meters above ground level.
        /// </summary>
        public int Ws50 { get; set; } = -1;

        /// <summary>
        /// Wind direction measured at 50 meters above ground level.
        /// </summary>
        public int Wd50 { get; set; } = -1;

        /// <summary>
        /// Temperature measured at 50 meters above ground level.
        /// </summary>
        public int Tp50 { get; set; } = -1;
    }


    public class DataModelBuilder
    {
        // Dicionário de mapeamento para associar sinônimos aos nomes das propriedades
        private static readonly Dictionary<string, string> _mappingDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "year", "Year" },
        { "day", "Day" },
        { "datetm", "DateTm" },
        { "min", "Min" },
        { "gloavg", "GloAvg" },
        { "diravg", "DirAvg" },
        { "diffavg", "DiffAvg" },
        { "lwavg", "LwAvg" },
        { "paravg", "ParAvg" },
        { "luxavg", "LuxAvg" },
        { "tpsfc", "TpSfc" },
        { "humid", "Humid" },
        { "humidade", "Humid" },
        { "press", "Press" },
        { "pressao", "Press" },
        { "rain", "Rain" },
        { "ws10m", "Ws10m" },
        { "wd10m", "Wd10m" },
        { "ws25", "Ws25" },
        { "wd25", "Wd25" },
        { "tp25", "Tp25" },
        { "ws50", "Ws50" },
        { "wd50", "Wd50" },
        { "tp50", "Tp50" }
    };

        public static DataModelInformation BuildDataModelInformation(string name, string delimiter, List<string> headers)
        {
            // Instancia um novo objeto DataModelInformation
            var dataModelInfo = new DataModelInformation();

            // Obtém todas as propriedades de DataModelInformation
            var properties = typeof(DataModelInformation).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Dicionário para mapear o nome da propriedade ao PropertyInfo
            var propertyMap = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in properties)
            {
                propertyMap[prop.Name] = prop;
            }

            // Itera sobre os cabeçalhos e tenta mapear cada um para uma propriedade usando o dicionário de mapeamento
            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                string normalizedHeader = Normalize(header); // Aplica regex para manter apenas letras

                // Verifica se o cabeçalho ou seu sinônimo está no dicionário de mapeamento
                if (_mappingDictionary.TryGetValue(normalizedHeader, out string mappedPropertyName))
                {
                    // Se o nome mapeado existe nas propriedades do DataModelInformation
                    if (propertyMap.TryGetValue(mappedPropertyName, out var property))
                    {
                        // Define o índice da coluna na propriedade correspondente
                        property.SetValue(dataModelInfo, i);
                    }
                }
            }
            dataModelInfo.Name = name;
            dataModelInfo.Delimiter = delimiter;
            return dataModelInfo;
        }

        // Método para normalizar nomes, removendo caracteres não alfabéticos e convertendo para minúsculas
        private static string Normalize(string input)
        {
            // Usa regex para manter apenas letras
            var lettersOnly = Regex.Replace(input, "[^a-zA-Z0-9]", "").ToLower();
            return lettersOnly;
        }
    }
}

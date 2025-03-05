using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQS.Core.Models
{
    public record EntryVariablesModel
    {
        public string FilePath { get; set; } = "";
        public QualityType QualityType { get; set; }
        public string SondaFileVersion { get; set; } = "";
        public AdvancedSettings AdvancedSettings { get; set; }
    }

    public enum QualityType
    {
        Anemometric,
        Meteorological,
        Radiometric
    }
    public record AdvancedSettings
    {
        bool EliminateDuplicates { get; set; } = true;

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQS.Core.Models
{
    public record DataModel
    {
        /// <summary>
        /// Unique identifier for the data entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The year of the recorded data.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// The day of the year for the recorded data.
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// The day of the year for the recorded data.
        /// </summary>
        public DateTime DateTm { get; set; }
        /// <summary>
        /// The minute of the day for the recorded data.
        /// </summary>
        public int Min { get; set; }

        /// <summary>
        /// Global average radiation measurement.
        /// </summary>
        public double GloAvg { get; set; }

        /// <summary>
        /// Average direct radiation measurement (dni).
        /// </summary>
        public double DirAvg { get; set; }

        /// <summary>
        /// Average diffuse radiation measurement (dif).
        /// </summary>
        public double DiffAvg { get; set; }

        /// <summary>
        /// Longwave radiation average measurement.
        /// </summary>
        public double LwAvg { get; set; }

        /// <summary>
        /// Photosynthetically Active Radiation (PAR) average measurement.
        /// </summary>
        public double ParAvg { get; set; }

        /// <summary>
        /// Average illuminance (lux) measurement.
        /// </summary>
        public double LuxAvg { get; set; }

        /// <summary>
        /// Surface temperature measurement.
        /// </summary>
        public double TpSfc { get; set; }

        /// <summary>
        /// Humidity percentage measurement.
        /// </summary>
        public double Humid { get; set; }

        /// <summary>
        /// Atmospheric pressure measurement.
        /// </summary>
        public double Press { get; set; }

        /// <summary>
        /// Precipitation measurement.
        /// </summary>
        public double Rain { get; set; }

        /// <summary>
        /// Wind speed measured at 10 meters above ground level.
        /// </summary>
        public double Ws10m { get; set; }

        /// <summary>
        /// Wind direction measured at 10 meters above ground level.
        /// </summary>
        public double Wd10m { get; set; }

        /// <summary>
        /// Wind speed measured at 25 meters above ground level.
        /// </summary>
        public double Ws25 { get; set; }

        /// <summary>
        /// Wind direction measured at 25 meters above ground level.
        /// </summary>
        public double Wd25 { get; set; }

        /// <summary>
        /// Temperature measured at 25 meters above ground level.
        /// </summary>
        public double Tp25 { get; set; }

        /// <summary>
        /// Wind speed measured at 50 meters above ground level.
        /// </summary>
        public double Ws50 { get; set; }

        /// <summary>
        /// Wind direction measured at 50 meters above ground level.
        /// </summary>
        public double Wd50 { get; set; }

        /// <summary>
        /// Temperature measured at 50 meters above ground level.
        /// </summary>
        public double Tp50 { get; set; }
    }
}

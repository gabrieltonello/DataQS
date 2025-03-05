using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DataQS.Core.Enums;
using LiveChartsCore.Defaults;
using DataQS.Core.Models;
using DataQS.Core.Services;
using LiveChartsCore.Drawing;


namespace DataQS.Core.ViewModels
{
    public class DashBoardViewModel : INotifyPropertyChanged
    {
        private string[]? _analisysData;
        private string _analisysTitle = "";
        private int _totalQuantity;
        private int _totalVariables;
        private int _goodDataCount;
        private int _badDataCount;
        private string _groupDataFormat = "yyyy-MM-dd";
        private ObservableCollection<ISeries>? _pieChartBadGoodData;
        private ObservableCollection<ISeries>? _barChartBadGoodData;
        private ObservableCollection<ISeries>? _barChartQualityData;
        private Axis[]? _xAxis;

        public void SetAnalisysData(string[] analisysData, List<DataModel> dataModel, string currentVariable, string[] validations, string groupDataFormat) 
        {
            if (groupDataFormat is not null) _groupDataFormat = groupDataFormat;

            var data = FileService.GetVariableData(dataModel, currentVariable);
            var dateTimes = dataModel.Select(i => new DateTime(i.Year, 1, 1)
                .AddDays(i.Day - 1)
                .AddMinutes(i.Min)).ToArray();

            bool showMaxAndMinValues = _groupDataFormat != "yyyy-MM-dd:HH-mm" && _groupDataFormat != "yyyy-MM-dd:HH:mm:ss";

            Dictionary<ValidationTypes, SolidColorPaint> validationColors = new()
            {
                { ValidationTypes.GoodData, new(SKColors.MediumSeaGreen) },  // Verde mais suave
                { ValidationTypes.RareData, new(SKColors.Goldenrod) },       // Amarelo/dourado
                { ValidationTypes.PhysicallyInvalid, new(SKColors.Tomato) }, // Vermelho vibrante
                { ValidationTypes.SessionRelatedData, new(SKColors.LightBlue) }, // Azul claro
                { ValidationTypes.Unknown, new(SKColors.DimGray) },          // Cinza escuro para dados desconhecidos
            };

            if (analisysData is null) return;

            TotalQuantity = analisysData.Length;
            TotalVariables = validations.Length;
            AnalisysTitle = $"Variável: {currentVariable}";
            
            List<ValidationTypes> validationTypes = [];
            foreach (string item in analisysData)
            {
                validationTypes.Add(item switch
                {
                    "0999" => ValidationTypes.GoodData,
                    "0599" => ValidationTypes.GoodData,
                    "0552" => ValidationTypes.PhysicallyInvalid,
                    "0529" => ValidationTypes.RareData,
                    "0029" => ValidationTypes.RareData,
                    "0299" => ValidationTypes.SessionRelatedData,
                    _ => ValidationTypes.Unknown
                });
            }

            GoodDataCount = validationTypes.Where(i => i.Equals(ValidationTypes.GoodData) || i.Equals(ValidationTypes.RareData)).Count();
            BadDataCount = validationTypes.Count - GoodDataCount;

            var validationData = validationTypes
                .GroupBy(p => p)
                .ToDictionary(g => g.Key, g => g.Count());

            this.PieChartBadGoodData = [];
            this.BarChartBadGoodData = [];
            this.BarChartQualityData = [];
            this.XAxis = [];

            foreach(var item in validationData)
            {
                this.PieChartBadGoodData.Add(new PieSeries<int> { Values = [item.Value], Name = item.Key.ToString(), Fill = validationColors[item.Key] });
            }    

            List<(DateTime date, ValidationTypes type, double data)> allData = []; 
            for (int i = 0; i < data.Length; i++) 
            {
                allData.Add((dateTimes[i], validationTypes[i], data[i]));
            }

            List<double> groupedByAverage = dateTimes
                .Zip(data, (date, value) => new { Date = date, Value = value })
                .GroupBy(item => item.Date.ToString(_groupDataFormat)) // Agrupa por ano, mês, dia e hora
                .Select(group => group.Average(item => item.Value)) // Calcula a média dos valores no grupo
                .ToList();
            
            List<double> groupedByMin = showMaxAndMinValues ? 
                dateTimes
                .Zip(data, (date, value) => new { Date = date, Value = value })
                .GroupBy(item => item.Date.ToString(_groupDataFormat)) // Agrupa por ano, mês, dia e hora
                .Select(group => group.Min(item => item.Value)) // Calcula a média dos valores no grupo
                .ToList()
                : [];
            
            List<double> groupedByMax = showMaxAndMinValues ? 
                dateTimes
                .Zip(data, (date, value) => new { Date = date, Value = value })
                .GroupBy(item => item.Date.ToString(_groupDataFormat)) // Agrupa por ano, mês, dia e hora
                .Select(group => group.Max(item => item.Value)) // Calcula a média dos valores no grupo
                .ToList()
                : [];

            this.BarChartBadGoodData.Add(new LineSeries<double>
            {
                Values = groupedByAverage,
                Name = "Average",
                Fill = new SolidColorPaint(SKColors.MediumSeaGreen),
                Stroke = null,
                GeometryFill = null, // Remove preenchimento do ponto
                GeometryStroke = null, // Remove borda do ponto
                GeometrySize = 0 // Remove completamente os pontos
            });

            this.BarChartBadGoodData.Add(new LineSeries<double>
            {
                Values = groupedByMax,
                Name = "Max",
                Stroke = null,
                GeometryFill = new SolidColorPaint(SKColors.Orange)
            });
                       
            this.BarChartBadGoodData.Add(new LineSeries<double>
            {
                Values = groupedByMin,
                Name = "Min",
                Stroke = null, 
                GeometryFill = new SolidColorPaint(SKColors.Orange)
            });

            var qualitygroupedData = allData
                     .GroupBy(item => new { Date = item.date.ToString(_groupDataFormat), Type = item.type })
                     .Select(group => new
                     {
                         group.Key.Date,  // Data no formato especificado
                         group.Key.Type,  // Tipo de validação
                         Count = group.Count()   // Contagem de ocorrências
                     })
                     .ToList();

            var series = qualitygroupedData
                .GroupBy(g => g.Type) // Agrupar novamente por tipo
                .Select(typeGroup => new StackedColumnSeries<double>
                {
                    Name = typeGroup.Key.ToString(), // Nome da série (Good, Bad)
                    Values = typeGroup.Select(x => (double)x.Count).ToArray(),
                    Fill = validationColors[typeGroup.Key]
                })
                .ToList();
            BarChartQualityData = new ObservableCollection<ISeries>(series);

            XAxis =
            [
                new Axis
                {
                    Name = "Dias Corridos",
                    NamePaint = new SolidColorPaint(SKColors.Black),

                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    TextSize = 11,
                    NameTextSize = 11,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 },
                    Labels = qualitygroupedData.Select(i => i.Date).ToList()
                }
            ];
        }

        public string AnalisysTitle
        {
            get => _analisysTitle;
            set
            {
                if (_analisysTitle == value) return;

                _analisysTitle = value;
                OnPropertyChanged(nameof(AnalisysTitle));
            }
        }


        public int TotalQuantity
        {
            get => _totalQuantity;
            set
            {
                if (_totalQuantity == value) return;

                _totalQuantity = value;
                OnPropertyChanged(nameof(TotalQuantity));
            }
        }
        public int TotalVariables
        {
            get => _totalVariables;
            set
            {
                if (_totalVariables == value) return;

                _totalVariables = value;
                OnPropertyChanged(nameof(TotalVariables));
            }
        }       
        
        public int GoodDataCount
        {
            get => _goodDataCount;
            set
            {
                if (_goodDataCount == value) return;

                _goodDataCount = value;
                OnPropertyChanged(nameof(GoodDataCount));
            }
        }       
               
        public int BadDataCount
        {
            get => _badDataCount;
            set
            {
                if (_badDataCount == value) return;

                _badDataCount = value;
                OnPropertyChanged(nameof(BadDataCount));
            }
        }       

        public ObservableCollection<ISeries> PieChartBadGoodData
        {
            get => _pieChartBadGoodData ?? [];
            set
            {
                if (_pieChartBadGoodData == value) return;

                _pieChartBadGoodData = value;
                OnPropertyChanged(nameof(PieChartBadGoodData));

            }
        }
        
        public ObservableCollection<ISeries> BarChartBadGoodData
        {
            get => _barChartBadGoodData ?? [];
            set
            {
                if (_barChartBadGoodData == value) return;

                _barChartBadGoodData = value;
                OnPropertyChanged(nameof(BarChartBadGoodData));
            }
        }
             
        public ObservableCollection<ISeries> BarChartQualityData
        {
            get => _barChartQualityData ?? [];
            set
            {
                if (_barChartQualityData == value) return;

                _barChartQualityData= value;
                OnPropertyChanged(nameof(BarChartQualityData));
            }
        }
                         
        public Axis[] XAxis
        {
            get => _xAxis ?? [];
            set
            {
                if (_xAxis == value) return;

                _xAxis = value;
                OnPropertyChanged(nameof(XAxis));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

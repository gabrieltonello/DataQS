
using DataQS.Core.Models;
using DataQS.Infraestructure.Repositories;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using DataQS.Core.ViewModels;
using LiveChartsCore.SkiaSharpView.Maui;
using System.Collections.ObjectModel;
using DataQS.Core.Services;
using System.Collections.Immutable;


namespace DataQSApp
{
    public partial class DashBoardPage1 : ContentPage
    {
        private HistoricalAnalisysRepository _historicalAnalisysRepository;
        private string _historyName = "";
        private string _currentvariableView = "";
        private readonly List<GroupByViewItem> _groupByViewData = [];
        private List<DataModel> _dataModels = new();
        Dictionary<string, string[]> _dataAnalisys = new();

        public DashBoardPage1(HistoricalAnalisysRepository historicalAnalisysRepository)
        {
            InitializeComponent();
            _historicalAnalisysRepository = historicalAnalisysRepository;

            UpdateHistoricalData();

            _groupByViewData =
            [
                new () { Title = "Agrupar por dia", Format = "yyyy-MM-dd" },
                new () { Title = "Agrupar por hora", Format = "yyyy-MM-dd:HH" },
                new () { Title = "Agrupar por minuto", Format = "yyyy-MM-dd:HH-mm" },
                new () { Title = "Agrupar por segundo", Format = "yyyy-MM-dd:HH:mm:ss" }
            ];

            UpdateGroupViewData();
            BindingContext = new DashBoardViewModel();
        }

        public void UpdateData(string historicalName, IEnumerable<DataModel> dataModels, Dictionary<string, string[]> dataAnalisys)
        {
            _historyName = historicalName;
            _dataModels = dataModels.ToList();
            _dataAnalisys = dataAnalisys;

            UpdateHistoricalData();
            UpdateModelsToAnalise();

            if (string.IsNullOrEmpty(_currentvariableView)) _currentvariableView = _dataAnalisys.FirstOrDefault().Key;

            UpdateCharts();
        }
        
        public void UpdateData(string groupDataFormat)
        {
            if (string.IsNullOrEmpty(_currentvariableView)) _currentvariableView = _dataAnalisys.FirstOrDefault().Key;

            UpdateCharts(groupDataFormat);
        }

        private void UpdateModelsToAnalise()
        {
            VariablesPicker.ItemsSource = _dataAnalisys
                .Where(i => i.Value.Count() > 0)
                .OrderBy(i => i.Key).Select(i => i.Key)
                .ToList();
        }

        private void UpdateHistoricalData()
        {
            var historicalData = _historicalAnalisysRepository.GetHistoricals();

            HistoricalPicker.ItemsSource = historicalData;
            HistoricalPicker.ItemDisplayBinding = new Binding("Name");
        }
        
        private void UpdateGroupViewData()
        {
            GroupByViewPicker.ItemsSource = _groupByViewData;
            GroupByViewPicker.ItemDisplayBinding = new Binding("Title");
        }

        private void OnHistoricalPickerSelected(object sender, EventArgs e)
        {
            if (HistoricalPicker.SelectedItem is HistoricalSummary historical)
            {
                var (HistoricalName, DataModels, DataAnalisys, _) = _historicalAnalisysRepository.GetHistorical(historical.Id);
                UpdateData(HistoricalName, DataModels, DataAnalisys);
            }

        }   
        
        private void OnGroupByViewPickerSelected(object sender, EventArgs e)
        {
            if (GroupByViewPicker.SelectedItem is GroupByViewItem groupBy)
            {
                //BindingContext
                UpdateData(groupBy.Format);
            }

        }   

        private void OnVariablesPickerSelected(object sender, EventArgs e)
        {
            if (VariablesPicker.SelectedItem is string pickedItem)
            {
                _currentvariableView = pickedItem;
                UpdateCharts();
            }
        }

        private void UpdateCharts(string groupDataFormat = null)
        {
            DashBoardViewModel? viewModel = BindingContext as DashBoardViewModel;
            _ = _dataAnalisys.TryGetValue(_currentvariableView, out string[] analisys);

            if (viewModel == null) return;

            viewModel.SetAnalisysData(analisys, _dataModels, _currentvariableView, _dataAnalisys.Keys.ToArray(), groupDataFormat);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateCharts();
        }
    }
}

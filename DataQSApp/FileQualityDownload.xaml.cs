
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
using CommunityToolkit.Maui.Storage;


namespace DataQSApp
{
    public partial class FileQualityDownload : ContentPage
    {
        private HistoricalAnalisysRepository _historicalAnalisysRepository;
        private string _historyName = "";
        private List<DataModel> _dataModels = new();
        private DataModelInformation _dataModelInformation = new();
        Dictionary<string, string[]> _dataAnalisys = new();

        public FileQualityDownload(HistoricalAnalisysRepository historicalAnalisysRepository)
        {
            InitializeComponent();
            _historicalAnalisysRepository = historicalAnalisysRepository;

            UpdateHistoricalData();

            BindingContext = new FileQualityDownloadViewModel();
        }

        public void UpdateData(string historicalName, IEnumerable<DataModel> dataModels, Dictionary<string, string[]> dataAnalisys, DataModelInformation dataInformation)
        {
            _historyName = historicalName;
            _dataModels = dataModels.ToList();
            _dataAnalisys = dataAnalisys;
            _dataModelInformation = dataInformation;
            UpdateHistoricalData();

        }

        private void UpdateHistoricalData()
        {
            var historicalData = _historicalAnalisysRepository.GetHistoricals();

            HistoricalPicker.ItemsSource = historicalData;
            HistoricalPicker.ItemDisplayBinding = new Binding("Name");
        }
        
        private void OnHistoricalPickerSelected(object sender, EventArgs e)
        {
            if (HistoricalPicker.SelectedItem is HistoricalSummary historical)
            {
                var (historicalName, dataModels, dataAnalisys, dataInformation) = _historicalAnalisysRepository.GetHistorical(historical.Id);
                UpdateData(historicalName, dataModels, dataAnalisys, dataInformation);
            }

        }           
        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            if (HistoricalPicker.SelectedItem is not HistoricalSummary historical) 
                return;

            var (historicalName, dataModels, dataAnalisys, dataInformation) = _historicalAnalisysRepository.GetHistorical(historical.Id);
            UpdateData(historicalName, dataModels, dataAnalisys, dataInformation);

            string fileName = $"{historicalName}.csv";
            
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            try
            {
                var folderResult = await FolderPicker.PickAsync(default);
                                
                if (!folderResult.IsSuccessful)
                {
                    // Usuário cancelou a seleção
                    await DisplayAlert("Cancelado", "Nenhuma pasta foi selecionada.", "OK");
                    return;
                }

                // Caminho da pasta selecionada
                string selectedFolderPath = folderResult.Folder.Path;
                string filePath = Path.Combine(selectedFolderPath, fileName);

                await FileService.ExportQualityFile(filePath, dataInformation, dataModels, dataAnalisys);

                await DisplayAlert("Sucesso", $"Arquivo baixado no diretório {selectedFolderPath}", "OK");

            }
            catch (Exception ex)
            {
                // Tratar erros, como cancelamento ou problemas no sistema
                await DisplayAlert("Erro", $"Não foi possível salvar o arquivo: {ex.Message}", "OK");
            }


        }   

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}

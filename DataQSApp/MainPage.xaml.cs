using DataQS.App;
using DataQS.Core.Models;
using DataQS.Core.Services;
using DataQS.Infraestructure.Repositories;
using Microsoft.Maui;
using System.Globalization;

namespace DataQSApp
{
    public partial class MainPage : ContentPage
    {
        private readonly StationRepository _stationRepository;
        private readonly DataModelRepository _dataModelRepository;
        private readonly StationRegistry _stationRegistry;
        private readonly DashBoardPage1 _dashBoardPage;
        private readonly DataModelRegistry _dataModelRegistry;
        private readonly HistoricalAnalisysRepository _historicalAnalisysRepository;
        private string _filePath = "";

        public MainPage(
            StationRegistry stationRegistry, 
            StationRepository stationRepository,
            DataModelRegistry dataModelRegistry, 
            DashBoardPage1 dashBoardPage,
            HistoricalAnalisysRepository historicalAnalisysRepository,
            DataModelRepository dataModelRepository)
        {
            InitializeComponent();
            _stationRepository = stationRepository;
            _dataModelRepository = dataModelRepository;
            _historicalAnalisysRepository = historicalAnalisysRepository;
            _stationRegistry = stationRegistry;
            _dataModelRegistry = dataModelRegistry;
            _dashBoardPage = dashBoardPage;
            IsBusy = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStations();
            LoadDataModel();
            _filePath = "";
        }

        private void LoadStations()
        {
            // Carrega a lista de estações do banco de dados
            var stations = _stationRepository.GetStations();

            StationPicker.ItemsSource = stations;
            StationPicker.ItemDisplayBinding = new Binding("Name");

        }
        private void LoadDataModel()
        {
            var dataModelInformation = _dataModelRepository.GetDataModelInformation();

            DataModelPicker.ItemsSource = dataModelInformation;
            DataModelPicker.ItemDisplayBinding = new Binding("Name");

        }
        private void OnStationPickerSelected(object sender, EventArgs e)
        {
            if (StationPicker.SelectedItem is StationModel selectedStation)
            {
                // Atualiza os rótulos com os detalhes da estação selecionada
                LatitudeLabel.Text = selectedStation.Latitude.ToString();
                LongitudeLabel.Text = selectedStation.Longitude.ToString();
                HeightLabel.Text = selectedStation.Height.ToString();
                MaxTempLabel.Text = selectedStation.MaxTemperature.ToString();
                MinTempLabel.Text = selectedStation.MinTemperature.ToString();
                MaxRainfallLabel.Text = selectedStation.MaxRainfall.ToString();

            }
        }

        // Evento quando o usuário seleciona um modelo de dados
        private void OnDataModelPickerSelected(object sender, EventArgs e)
        {
            if (DataModelPicker.SelectedItem is DataModelInformation selectedModel)
            {
                // Atualiza os Labels com os valores do modelo selecionado
                YearLabel.Text = selectedModel.Year.ToString();
                DayLabel.Text = selectedModel.Day.ToString();
                DateTmLabel.Text = selectedModel.DateTm.ToString();
                MinLabel.Text = selectedModel.Min.ToString();
                GloAvgLabel.Text = selectedModel.GloAvg.ToString();
                DirAvgLabel.Text = selectedModel.DirAvg.ToString();
                DiffAvgLabel.Text = selectedModel.DiffAvg.ToString();
                LwAvgLabel.Text = selectedModel.LwAvg.ToString();
                ParAvgLabel.Text = selectedModel.ParAvg.ToString();
                LuxAvgLabel.Text = selectedModel.LuxAvg.ToString();
                TpSfcLabel.Text = selectedModel.TpSfc.ToString();
                HumidLabel.Text = selectedModel.Humid.ToString();
                PressLabel.Text = selectedModel.Press.ToString();
                RainLabel.Text = selectedModel.Rain.ToString();
                Ws10mLabel.Text = selectedModel.Ws10m.ToString();
                Wd10mLabel.Text = selectedModel.Wd10m.ToString();
                Ws25Label.Text = selectedModel.Ws25.ToString();
                Wd25Label.Text = selectedModel.Wd25.ToString();
                Tp25Label.Text = selectedModel.Tp25.ToString();
                Ws50Label.Text = selectedModel.Ws50.ToString();
                Wd50Label.Text = selectedModel.Wd50.ToString();
                Tp50Label.Text = selectedModel.Tp50.ToString();
            }
        }

        // Evento para Cadastrar Nova Estação
        private async void OnAddStationClicked(object sender, EventArgs e)
        {

            // Redireciona para uma nova página de cadastro
            await Navigation.PushAsync(_stationRegistry);
        }

        // Evento para Deletar Estação Selecionada
        private async void OnDeleteStationClicked(object sender, EventArgs e)
        {
            // Verifica se há uma estação selecionada
            if (StationPicker.SelectedItem is StationModel selectedStation)
            {
                // Confirmação antes de deletar
                var confirm = await DisplayAlert("Confirmar", $"Deseja deletar a estação '{selectedStation.Name}'?", "Sim", "Não");
                if (confirm)
                {
                    // Deleta a estação e recarrega a lista no Picker
                    _stationRepository.DeleteStation(selectedStation.Id);
                    LoadStations();
                    await DisplayAlert("Sucesso", "Estação deletada com sucesso.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro", "Selecione uma estação para deletar.", "OK");
            }
        }   
        // Evento para Cadastrar Nova Estação
        private async void OnAddDataModelClicked(object sender, EventArgs e)
        {

            // Redireciona para uma nova página de cadastro
            await Navigation.PushAsync(_dataModelRegistry);
        }

        // Evento para Deletar Estação Selecionada
        private async void OnDeleteDataModelClicked(object sender, EventArgs e)
        {
            // Verifica se há uma estação selecionada
            if (StationPicker.SelectedItem is StationModel selectedStation)
            {
                // Confirmação antes de deletar
                var confirm = await DisplayAlert("Confirmar", $"Deseja deletar a estação '{selectedStation.Name}'?", "Sim", "Não");
                if (confirm)
                {
                    // Deleta a estação e recarrega a lista no Picker
                    _stationRepository.DeleteStation(selectedStation.Id);
                    LoadStations();
                    await DisplayAlert("Sucesso", "Estação deletada com sucesso.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro", "Selecione uma estação para deletar.", "OK");
            }
        }

        private async void OnUploadFileClicked(object sender, EventArgs e)
        {
            try
            {
                // Define os tipos de arquivo aceitos
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text", "com.microsoft.excel.xlsx" } },
                    { DevicePlatform.Android, new[] { "text/csv", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } },
                    { DevicePlatform.WinUI, new[] { ".csv", ".xlsx" } },
                    { DevicePlatform.Tizen, new[] { ".csv", ".xlsx" } }
                });

                // Opções de seleção do arquivo
                var pickOptions = new PickOptions
                {
                    PickerTitle = "Selecione um arquivo de dados",
                    FileTypes = customFileType
                };

                // Abre o seletor de arquivos
                var result = await FilePicker.PickAsync(pickOptions);

                if (result != null)
                {
                    // Exibir o nome do arquivo no Label
                    FileNameLabel.Text = $"Arquivo selecionado: {result.FullPath}";
                    _filePath = result.FullPath;
                    // Aqui você pode implementar a lógica de upload ou processamento do arquivo
                }
                else
                {
                    // Caso o usuário cancele a seleção do arquivo
                    FileNameLabel.Text = "";
                }
            }
            catch (Exception ex)
            {
                // Tratar possíveis erros na seleção de arquivo
                await DisplayAlert("Erro", $"Falha ao selecionar o arquivo: {ex.Message}", "OK");
            }
        }      
        private async void OnStartQualityProcessClicked(object sender, EventArgs e)
        {
            try
            {
                this.IsBusy = true;
                #region Tratando possíveis erros de usuário
                if (string.IsNullOrWhiteSpace(FileNameLabel.Text))
                {
                    await DisplayAlert("Erro", $"O arquivo precisa ser selecionado.", "OK");
                    return;
                }
                else if (!File.Exists(_filePath))
                {
                    await DisplayAlert("Erro", $"O arquivo não existe.", "OK");
                    return;
                }
                else if (DataModelPicker.SelectedItem is not DataModelInformation)
                {
                    await DisplayAlert("Erro", $"O modelo de dado não foi selecionado.", "OK");
                    return;
                }   
                else if (StationPicker.SelectedItem is not StationModel)
                {
                    await DisplayAlert("Erro", $"A estação não foi selecionado.", "OK");
                    return;
                }
                #endregion

                DataModelInformation dataModelInformation = (DataModelInformation)DataModelPicker.SelectedItem;
                StationModel stationModel = (StationModel)StationPicker.SelectedItem;


                (var datamodels, var dataAnalisys) = FileService.StartQualityProcess(_filePath, dataModelInformation, stationModel);

                string historicalName = $"{Path.GetFileNameWithoutExtension(_filePath)} - {DateTime.Now.ToString(CultureInfo.CurrentCulture)}";
                _historicalAnalisysRepository.AddHistorical(historicalName, datamodels, dataAnalisys, dataModelInformation);

                _dashBoardPage.UpdateData(historicalName, datamodels, dataAnalisys);
                await Navigation.PushAsync(_dashBoardPage);
                this.IsBusy = false;
                
            }
            catch (Exception ex)
            {
                // Tratar possíveis erros na seleção de arquivo
                await DisplayAlert("Erro", $"Falha ao processar o arquivo: {ex.Message}", "OK");
            }
        }
    }
}

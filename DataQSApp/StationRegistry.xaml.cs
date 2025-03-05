using DataQS.Core.Models;
using DataQS.Infraestructure.Repositories;

namespace DataQS.App;

public partial class StationRegistry : ContentPage
{
    private readonly StationRepository _stationRepository;

    public StationRegistry(StationRepository stationRepository)
    {
        InitializeComponent();
        _stationRepository = stationRepository; // Instancia o repositório
    }

    private async void OnSaveStationClicked(object sender, EventArgs e)
    {
        try
        {
            // Captura os dados do formulário
            var station = new StationModel
            {
                Name = NameEntry.Text,
                Latitude = float.Parse(LatitudeEntry.Text),
                Longitude = float.Parse(LongitudeEntry.Text),
                Height = float.Parse(HeightEntry.Text),
                MaxTemperature = float.Parse(MaxTemperatureEntry.Text),
                MinTemperature = float.Parse(MinTemperatureEntry.Text),
                MaxRainfall = float.Parse(MaxRainfallEntry.Text)
            };

            // Salva a estação no banco de dados
            _stationRepository.AddStation(station);

            // Retorna para a página principal e atualiza a lista
            await DisplayAlert("Sucesso", "Estação cadastrada com sucesso!", "OK");
            await Navigation.PopAsync(); // Volta para a página principal
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao salvar estação: {ex.Message}", "OK");
        }
    }
}
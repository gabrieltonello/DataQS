using DataQS.Core.Models;
using DataQS.Infraestructure.Repositories;

namespace DataQS.App;

public partial class StationRegistry : ContentPage
{
    private readonly StationRepository _stationRepository;

    public StationRegistry(StationRepository stationRepository)
    {
        InitializeComponent();
        _stationRepository = stationRepository; // Instancia o reposit�rio
    }

    private async void OnSaveStationClicked(object sender, EventArgs e)
    {
        try
        {
            // Captura os dados do formul�rio
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

            // Salva a esta��o no banco de dados
            _stationRepository.AddStation(station);

            // Retorna para a p�gina principal e atualiza a lista
            await DisplayAlert("Sucesso", "Esta��o cadastrada com sucesso!", "OK");
            await Navigation.PopAsync(); // Volta para a p�gina principal
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao salvar esta��o: {ex.Message}", "OK");
        }
    }
}
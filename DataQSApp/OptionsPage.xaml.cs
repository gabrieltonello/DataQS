using DataQS.Infraestructure.Repositories;
using Microsoft.Maui.Controls;

namespace DataQSApp
{
    public partial class OptionsPage : ContentPage
    {
        private readonly HistoricalAnalisysRepository _historicalAnalisysRepository;

        public OptionsPage(HistoricalAnalisysRepository historicalAnalisysRepository)
        {
            InitializeComponent();

            _historicalAnalisysRepository = historicalAnalisysRepository;
        }

        private async void OnDeleteProcessedDataClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirma��o",
                "Tem certeza de que deseja excluir todos os dados processados?",
                "Sim",
                "N�o");

            if (confirm)
            {
                try
                {
                    _historicalAnalisysRepository.DeleteAllHistoricalData();

                    await DisplayAlert("Sucesso",
                        "Os dados processados foram exclu�dos com sucesso.",
                        "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro",
                        $"Ocorreu um erro ao excluir os dados: {ex.Message}",
                        "OK");
                }
            }
        }
    }
}

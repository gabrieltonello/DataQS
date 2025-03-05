namespace DataQS.App;

using DataQS.Core.Models;
using System.Collections.ObjectModel;

public partial class DataModelRegistry : ContentPage
{

    public DataModelRegistry()
    {
        InitializeComponent();

        // Define o BindingContext para a página
        BindingContext = this;
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            // Obter os valores do formulário
            DataModelInformation dataModel = new()
            {
                Name = NameEntry.Text,
                Delimiter = DelimiterEntry.Text,
                Year = int.TryParse(YearEntry.Text, out var year) ? year : -1,
                Day = int.TryParse(DayEntry.Text, out var day) ? day : -1,
                DateTm = int.TryParse(DateTmEntry.Text, out var dateTm) ? dateTm : -1,
                Min = int.TryParse(MinEntry.Text, out var min) ? min : -1,
                GloAvg = int.TryParse(GloAvgEntry.Text, out var gloAvg) ? gloAvg : -1,
                DirAvg = int.TryParse(DirAvgEntry.Text, out var dirAvg) ? dirAvg : -1,
                DiffAvg = int.TryParse(DiffAvgEntry.Text, out var diffAvg) ? diffAvg : -1,
                LwAvg = int.TryParse(LwAvgEntry.Text, out var lwAvg) ? lwAvg : -1,
                ParAvg = int.TryParse(ParAvgEntry.Text, out var parAvg) ? parAvg : -1,
                LuxAvg = int.TryParse(LuxAvgEntry.Text, out var luxAvg) ? luxAvg : -1,
                TpSfc = int.TryParse(TpSfcEntry.Text, out var tpSfc) ? tpSfc : -1,
                Humid = int.TryParse(HumidEntry.Text, out var humid) ? humid : -1,
                Press = int.TryParse(PressEntry.Text, out var press) ? press : -1,
                Rain = int.TryParse(RainEntry.Text, out var rain) ? rain : -1,
                Ws10m = int.TryParse(Ws10mEntry.Text, out var ws10m) ? ws10m : -1,
                Wd10m = int.TryParse(Wd10mEntry.Text, out var wd10m) ? wd10m : -1,
                Ws25 = int.TryParse(Ws25Entry.Text, out var ws25) ? ws25 : -1,
                Wd25 = int.TryParse(Wd25Entry.Text, out var wd25) ? wd25 : -1,
                Tp25 = int.TryParse(Tp25Entry.Text, out var tp25) ? tp25 : -1,
                Ws50 = int.TryParse(Ws50Entry.Text, out var ws50) ? ws50 : -1,
                Wd50 = int.TryParse(Wd50Entry.Text, out var wd50) ? wd50 : -1,
                Tp50 = int.TryParse(Tp50Entry.Text, out var tp50) ? tp50 : -1
            };

            // Exemplo: salvar dados no banco ou exibir um alerta
            DisplayAlert("Sucesso", "Dados cadastrados com sucesso!", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Erro ao salvar os dados: {ex.Message}", "OK");
        }
    }
}
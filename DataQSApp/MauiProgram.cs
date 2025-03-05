using CommunityToolkit.Maui;
using DataQS.App;
using DataQS.Infraestructure.Repositories;
using LiteDB;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DataQSApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()

                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseSkiaSharp();
            builder.Services.AddTransient<StationRepository>();
            builder.Services.AddTransient<DataModelRepository>();
            builder.Services.AddTransient<DataModelRepository>();
            builder.Services.AddTransient<HistoricalAnalisysRepository>();
            builder.Services.AddTransient<DataModelRegistry>();
            builder.Services.AddTransient<StationRegistry>(); 
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<OptionsPage>();
            builder.Services.AddTransient<DashBoardPage1>();
            builder.Services.AddTransient<FileQualityDownload>();

            // Registra a instância compartilhada de LiteDatabase
            var databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DataQS_LiteDB.db");
            var liteDatabase = new LiteDatabase($"Filename={databasePath};Mode=Shared");
            builder.Services.AddSingleton(liteDatabase); 

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

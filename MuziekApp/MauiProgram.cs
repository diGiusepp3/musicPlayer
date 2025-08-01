﻿using Microsoft.Extensions.Logging;
using MuziekApp.Services;
using MuziekApp.ViewModels;
using MuziekApp.Views; 

namespace MuziekApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // === Dependency Injection registraties ===
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginView>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterView>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainView>();
        builder.Services.AddSingleton<StartupCheckService>();

        // === Search functionaliteit ===
        builder.Services.AddSingleton<SearchService>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<SearchView>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
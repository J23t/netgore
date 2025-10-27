using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System;
using System.Linq;
using Avalonia.Markup.Xaml;
using DemoGame.Editor.Avalonia.ViewModels;
using DemoGame.Editor.Avalonia.Views;
using DemoGame.Editor.Avalonia.Services;

namespace DemoGame.Editor.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            
            // Initialize NetGore content system
            Console.WriteLine("🚀 Initializing NetGore content system...");
            ContentService.Initialize();
            
            // Use full docking window (panels proven to work!)
            desktop.MainWindow = new MainWindow();
            // Can switch back to simplified for testing: new MainWindow_Simple();
            
            // Cleanup on exit
            desktop.ShutdownRequested += (s, e) =>
            {
                ContentService.Shutdown();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
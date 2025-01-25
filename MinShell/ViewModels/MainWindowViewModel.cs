namespace MinShell.ViewModels;
using System.Text.Json;
using System.Threading.Tasks;

using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MinShell.Models;

internal partial class MainWindowViewModel : ViewModelBase
{
    private const string SettingsFileName = "settings.json";

    [ObservableProperty]
    private AvaloniaList<UserAppViewModel> _applications = new();

    [RelayCommand]
    private async Task LoadUserApps()
    {
        var settingsFilePath = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData),
            nameof(MinShell),
            SettingsFileName);
        var settings = Array.Empty<UserAppViewModel>();
        if (File.Exists(settingsFilePath))
        {
            settings = JsonSerializer.Deserialize<UserAppViewModel[]>(
                await File.ReadAllTextAsync(settingsFilePath))!;
            Applications.AddRange(settings);
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(
                settingsFilePath)!);
            await File.WriteAllTextAsync(settingsFilePath,
                JsonSerializer.Serialize(settings));
        }
    }

    [RelayCommand]
    private async Task SaveUserApps()
    {
        var settingsFilePath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
                nameof(MinShell),
                SettingsFileName);
        await File.WriteAllTextAsync(settingsFilePath,
            JsonSerializer.Serialize(Applications.ToArray()));
    }

    [RelayCommand]
    private async Task AddApplication()
    {
        TopLevel? topLevel = GetTopLevel();

        // Start async operation to open the dialog.
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select file or app to launch...",
            AllowMultiple = false
        });

        if (files.Count == 1)
        {
            Applications.Add(new UserAppViewModel()
            {
                Name = Path.GetFileNameWithoutExtension(files[0].Name),
                Path = files[0].TryGetLocalPath(),
            });

        }
    }

    private static TopLevel? GetTopLevel()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var appLifetime = App.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;

        var topLevel = TopLevel.GetTopLevel(appLifetime!.MainWindow);
        return topLevel;
    }

    [RelayCommand]
    private void RemoveApplication()
    {
        if (SelectedApplication is not null)
        {
            Applications.Remove(SelectedApplication);
        }
    }

    [ObservableProperty]
    private UserAppViewModel? _selectedApplication;
}

namespace MinShell.Models;

using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class UserAppViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string? _path;
    [ObservableProperty]
    private string? _arguments;
    [ObservableProperty]
    private string? _workingDirectory;

    [ObservableProperty]
    [JsonIgnore]
    [IgnoreDataMember]
    private string? _errorMessage;

    private void Launch()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path,
                Arguments = Arguments,
                WorkingDirectory = WorkingDirectory,
            }
        };
        try
        {
            process.Start();
        }
        catch (InvalidOperationException e)
        {
            ErrorMessage = e.Message;
        }
    }

    [IgnoreDataMember]
    [JsonIgnore]
    public RelayCommand LaunchCommand => new(() => Launch());
}

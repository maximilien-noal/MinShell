using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using MinShell.ViewModels;

namespace MinShell
{
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
                var splashScreen = new SplashScreen();

                splashScreen.Show();

                desktop.MainWindow = new MainWindow();

                desktop.MainWindow.Loaded += (_, _) =>
                {
                    var vm = new MainWindowViewModel();
                    desktop.MainWindow.DataContext = vm;
                    vm.LoadUserAppsCommand.Execute(null);
                    splashScreen.Close();
                };

                desktop.Exit += (_, _) =>
                {
                    if (desktop.MainWindow.DataContext is MainWindowViewModel vm)
                    {
                        vm.SaveUserAppsCommand.Execute(null);
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
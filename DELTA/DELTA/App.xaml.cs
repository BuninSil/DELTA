using System.Windows;

namespace DELTA
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {


            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Завершаем работу CefSharp при закрытии приложения

            base.OnExit(e);
        }
    }
}

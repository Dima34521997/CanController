using Scenario_Editor.Models;
using Scenario_Editor.ViewModels;
using System.Windows;

namespace Scenario_Editor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ScenariosBook scenariosBook;

    public App()
    {
        scenariosBook = new ScenariosBook("[Название Книги]");

        Scenario scenario = new Scenario("[Название Сценария]");
        scenario.Tasks.Add(new Task("[Название Задачи]", "[Описание]"));
        scenariosBook.AddScenario(scenario);
    }
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow()
        {
            DataContext = new MainVM(scenariosBook)
        };
        MainWindow.Show();


        base.OnStartup(e);
    }
}

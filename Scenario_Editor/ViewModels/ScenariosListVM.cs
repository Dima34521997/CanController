using Scenario_Editor.Commands.ScenariosList;
using Scenario_Editor.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Scenario_Editor.ViewModels
{
    public class ScenariosListVM : VMBase
    {
        private readonly ObservableCollection<ScenarioVM> scenarios;
        public IEnumerable<ScenarioVM> Scenarios => scenarios;

        private readonly ScenariosBook scenariosBook;

        private VMBase tasksListCurrentVM;
        public VMBase TasksListCurrentVM
        {
            get
            {
                return tasksListCurrentVM;
            }
            set
            {
                tasksListCurrentVM = value;
                OnPropertyChanged(nameof(TasksListCurrentVM));
            }
        }

        private string bookName;
        public string BookName
        {
            get
            {
                return bookName;
            }
            set
            {
                bookName = value;
                scenariosBook.Name = bookName;
                OnPropertyChanged(nameof(BookName));
            }
        }

        private string scenarioName;
        public string ScenarioName
        {
            get
            {
                return scenarioName;
            }
            set
            {
                Set(ref scenarioName, value);
            }
        }

        private int selectedScenario = 0;
        public int SelectedScenario
        {
            get
            {
                return selectedScenario;
            }
            set
            {
                selectedScenario = value;
                OnPropertyChanged(nameof(SelectedScenario));
                TasksListCurrentVM = new TasksListVM(scenariosBook, selectedScenario);
            }
        }


        public ICommand SaveScenario { get; }
        public ICommand SaveScenariosBook { get; }
        public ICommand CreateScenario { get; }
        public ICommand CreateEmptyScenario { get; }
        public ICommand ScenarioSelectChangedCommand { get; }

        public ScenariosListVM(ScenariosBook scenariosBook)
        {
            this.scenariosBook = scenariosBook;
            BookName = scenariosBook.Name;

            scenarios = new ObservableCollection<ScenarioVM>();
            foreach (Scenario scenario in scenariosBook.Scenarios)
            {
                scenarios.Add(new ScenarioVM(scenario));
            }

            TasksListCurrentVM = new TasksListVM(this.scenariosBook, selectedScenario);

            SaveScenario = new SaveScenario(scenariosBook, this);
            SaveScenariosBook = new SaveScenariosBook(scenariosBook);
            CreateScenario = new CreateScenario(this, scenariosBook);
            CreateEmptyScenario = new CreateEmptyScenario(this, scenariosBook);
            ScenarioSelectChangedCommand = new ScenarioSelectChangedCommand(this);

            UpdateScenarios();
            SelectedScenario = 0;
        }

        public void UpdateScenarios()
        {
            TasksListCurrentVM = null;
            scenarios.Clear();

            foreach (Scenario scenario in scenariosBook.Scenarios)
            {
                scenarios.Add(new ScenarioVM(scenario));
            }
            SelectedScenario = Scenarios.Count() - 1;

            TasksListCurrentVM = new TasksListVM(scenariosBook, SelectedScenario);
        }

        public void ScenarioSelectChanged()
        {
            if (Scenarios.Count() > 0)
                TasksListCurrentVM = new TasksListVM(scenariosBook, selectedScenario);
        }
    }
}

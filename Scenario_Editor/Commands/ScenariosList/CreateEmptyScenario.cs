using Scenario_Editor.Exceptions;
using Scenario_Editor.Models;
using Scenario_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scenario_Editor.Commands.ScenariosList
{
    public class CreateEmptyScenario : CommandBase
    {
        private readonly ScenariosListVM scenariosListViewModel;
        private readonly ScenariosBook scenarioBook;

        public CreateEmptyScenario(ScenariosListVM scenariosListViewModel, ScenariosBook scenariosBook)
        {
            this.scenariosListViewModel = scenariosListViewModel;
            scenarioBook = scenariosBook;

            this.scenariosListViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return
                !string.IsNullOrEmpty(scenariosListViewModel.ScenarioName) &&
                base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            Scenario scenario = new Scenario(
                scenariosListViewModel.ScenarioName
                );
            scenario.AddTask(new Task("Task 1", "Dis 1"));

            try
            {
                scenarioBook.AddScenario(scenario);
                scenariosListViewModel.UpdateScenarios();
            }
            catch (ScenarioNameConflictException)
            {

            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScenariosListVM.ScenarioName)) OnCanExecuteChange();
        }
    }
}

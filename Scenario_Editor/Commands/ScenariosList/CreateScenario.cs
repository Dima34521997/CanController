using Scenario_Editor.Exceptions;
using Scenario_Editor.Models;
using Scenario_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.Commands.ScenariosList
{
    public class CreateScenario : CommandBase
    {
        private readonly ScenariosListVM scenariosListViewModel;
        private readonly ScenariosBook scenarioBook;

        public CreateScenario(ScenariosListVM scenariosListViewModel, ScenariosBook scenarioBook)
        {
            this.scenariosListViewModel = scenariosListViewModel;
            this.scenarioBook = scenarioBook;

            scenariosListViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override void Execute(object parameter)
        {
            Scenario scenario = new Scenario(
                scenariosListViewModel.ScenarioName
                );
            scenario.Tasks = scenarioBook.Scenarios[scenariosListViewModel.SelectedScenario].Tasks;

            try
            {
                scenarioBook.AddScenario(scenario);
                scenariosListViewModel.UpdateScenarios();
            }
            catch (ScenarioNameConflictException)
            {

            }
        }

        public override bool CanExecute(object parameter)
        {
            return
                !string.IsNullOrEmpty(scenariosListViewModel.ScenarioName) &&
                base.CanExecute(parameter);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ScenariosListVM.ScenarioName)) OnCanExecuteChange();
        }
    }
}

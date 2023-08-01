using Scenario_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.Commands.ScenariosList
{
    public class ScenarioSelectChangedCommand : CommandBase
    {
        private readonly ScenariosListVM scenarioListingViewModel;

        public ScenarioSelectChangedCommand(ScenariosListVM scenarioListingViewModel)
        {
            this.scenarioListingViewModel = scenarioListingViewModel;
        }

        public override void Execute(object parameter)
        {
            scenarioListingViewModel.ScenarioSelectChanged();
        }
    }
}

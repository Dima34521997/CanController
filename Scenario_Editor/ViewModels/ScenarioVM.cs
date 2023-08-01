using Scenario_Editor.Models;
using System.Collections.Generic;

namespace Scenario_Editor.ViewModels
{
    public class ScenarioVM : VMBase
    {
        private readonly Scenario scenario;

        public string Name => scenario.Name;
        public string Description => scenario.Description;
        public List<Task> Tasks => scenario.Tasks;

        public ScenarioVM(Scenario scenario)
        {
            this.scenario = scenario;
        }

        public void AddTask(Task task)
        {
            scenario.Tasks.Add(task);
        }
    }
}

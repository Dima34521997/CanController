using Scenario_Editor.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.Models
{
    public class ScenariosBook
    {
        public List<Scenario> Scenarios;
        public string Name { get; set; }

        public ScenariosBook(string name)
        {
            Scenarios = new List<Scenario>();
            Name = name;
        }

        public void AddScenario(Scenario scenario)
        {
            foreach (Scenario existingScenario in Scenarios)
            {
                if (existingScenario.Name == scenario.Name) throw new ScenarioNameConflictException(scenario, existingScenario);
            }

            this.Scenarios.Add(scenario);
        }
    }
}

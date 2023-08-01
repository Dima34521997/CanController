using Scenario_Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.Exceptions
{
    public class ScenarioNameConflictException : Exception
    {
        public Scenario IncomingScenario { get; }
        public Scenario ExistingScenario { get; }
        public ScenarioNameConflictException(Scenario incomingScenario, Scenario existingScenario)
        {
            IncomingScenario = incomingScenario;
            ExistingScenario = existingScenario;
        }

        public ScenarioNameConflictException(string message, Scenario incomingScenario, Scenario existingScenario) : base(message)
        {
            IncomingScenario = incomingScenario;
            ExistingScenario = existingScenario;
        }

        public ScenarioNameConflictException(string message, Exception innerException, Scenario incomingScenario, Scenario existingScenario) : base(message, innerException)
        {
            IncomingScenario = incomingScenario;
            ExistingScenario = existingScenario;
        }
    }
}

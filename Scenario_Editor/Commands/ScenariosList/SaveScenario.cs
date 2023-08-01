using Newtonsoft.Json;
using Scenario_Editor.Models;
using Scenario_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.Commands.ScenariosList
{
    public class SaveScenario : CommandBase
    {
        private readonly ScenariosListVM ScenariosListVM;
        private readonly ScenariosBook scenarioBook;

        public SaveScenario(ScenariosBook scenarioBook, ScenariosListVM ScenariosListVM)
        {
            this.scenarioBook = scenarioBook;
            this.ScenariosListVM = ScenariosListVM;
        }

        public override void Execute(object parameter)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Сценарии"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Сценарии");
            }

            using (StreamWriter file = File.CreateText($@"Сценарии\[Сценарий] {scenarioBook.Scenarios[ScenariosListVM.SelectedScenario].Name}.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, scenarioBook.Scenarios[ScenariosListVM.SelectedScenario]);
            }
        }
    }
}

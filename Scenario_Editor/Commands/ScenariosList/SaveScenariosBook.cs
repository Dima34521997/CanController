using Newtonsoft.Json;
using Scenario_Editor.Models;
using Scenario_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Scenario_Editor.Commands.ScenariosList
{
    public class SaveScenariosBook : CommandBase
    {
        private readonly ScenariosBook scenarioBook;

        public SaveScenariosBook(ScenariosBook scenarioBook)
        {
            this.scenarioBook = scenarioBook;
        }

        public override void Execute(object parameter)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Сценарии"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Сценарии");
            }

            using (StreamWriter file = File.CreateText($@"Сценарии\[Книга] {scenarioBook.Name}.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, scenarioBook);
            }
        }
    }
}

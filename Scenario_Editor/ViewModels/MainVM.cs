using Scenario_Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.ViewModels;

public class MainVM : VMBase
{
    public VMBase CurrentViewModel { get; }

    public MainVM(ScenariosBook scenariosBook)
    {
        CurrentViewModel = new ScenariosListVM(scenariosBook);
    }
}

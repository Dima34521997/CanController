using Scenario_Editor.ViewModels;
using System;

namespace Scenario_Editor.Stores;

public class NavigationStore
{
    private VMBase currentViewModel;
    public VMBase CurrentVM
    {
        get => currentViewModel;
        set
        {
            currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}

using Scenario_Editor.ViewModels;
using System;

namespace DevNetEmulator.Stores;

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

using Scenario_Editor.Models;
using Scenario_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Scenario_Editor.Commands.TaskList
{
    public class AddTask : CommandBase
    {
        private readonly TasksListVM tasksListingViewModel;
        private readonly ScenariosBook scenariosBook;
        private readonly int scenarioIndex;

        public AddTask(TasksListVM tasksListingViewModel, ScenariosBook scenariosBook, int scenarioIndex)
        {
            this.tasksListingViewModel = tasksListingViewModel;
            this.scenariosBook = scenariosBook;
            this.scenarioIndex = scenarioIndex;

            this.tasksListingViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object parameter)
        {
            return
                !string.IsNullOrEmpty(tasksListingViewModel.TaskName) &&
                !string.IsNullOrEmpty(tasksListingViewModel.TaskDis) &&
                base.CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            Task task = new Task(
                tasksListingViewModel.TaskName,
                tasksListingViewModel.TaskDis
                );

            scenariosBook.Scenarios[scenarioIndex].AddTask(task);

            tasksListingViewModel.UpdateTasks();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TasksListVM.TaskName) ||
                e.PropertyName == nameof(TasksListVM.TaskDis))
                OnCanExecuteChange();
        }
    }
}

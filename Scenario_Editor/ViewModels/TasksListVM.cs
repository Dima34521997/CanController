using Scenario_Editor.Commands.TaskList;
using Scenario_Editor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Scenario_Editor.ViewModels
{
    public class TasksListVM : VMBase
    {
        private readonly ObservableCollection<TaskVM> tasks;
        public IEnumerable<TaskVM> Tasks => tasks;

        private readonly ScenariosBook scenariosBook;
        private readonly int scenarioIndex;
        public ICommand AddTask { get; }

        private string taskName;
        public string TaskName
        {
            get
            {
                return taskName;
            }
            set
            {
                Set(ref taskName, value);
            }
        }

        private string taskDis;
        public string TaskDis
        {
            get
            {
                return taskDis;
            }
            set
            {
                Set(ref taskDis, value);
            }
        }

        public TasksListVM(ScenariosBook scenariosBook, int scenarioIndex)
        {
            this.scenarioIndex = scenarioIndex;
            this.scenariosBook = scenariosBook;
            tasks = new ObservableCollection<TaskVM>();

            AddTask = new AddTask(this, scenariosBook, this.scenarioIndex);

            if (this.scenarioIndex >= 0)
                UpdateTasks();
        }

        public void UpdateTasks()
        {
            tasks.Clear();

            foreach (Task task in scenariosBook.Scenarios[scenarioIndex].Tasks)
            {
                tasks.Add(new TaskVM(task));
            }
        }
    }
}

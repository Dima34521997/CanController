using Scenario_Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scenario_Editor.ViewModels
{
    public class TaskVM : VMBase
    {
        private readonly Task task;

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                task.Name = name;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                task.Description = description;
                OnPropertyChanged(nameof(Description));
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public TaskVM(Task task)
        {
            this.task = task;

            name = task.Name;
            description = task.Description;
        }

    }
}

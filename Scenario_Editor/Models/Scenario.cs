using Scenario_Editor.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scenario_Editor.Models
{
    public class Scenario
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Task> Tasks;

        public Scenario(string name)
        {
            Name = name;
            Tasks = new List<Task>();
        }

        public IEnumerable<Task> GetTasksList()
        {
            return Tasks;
        }

        public void AddTask(Task task)
        {
            foreach (Task existingTask in Tasks)
            {
                if (existingTask.Name == task.Name) throw new TaskNameConflictException(existingTask, task);
            }

            Tasks.Add(task);
        }
    }
}

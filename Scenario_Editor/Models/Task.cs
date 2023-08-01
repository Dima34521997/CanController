using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scenario_Editor.Models
{
    public class Task
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }

        public Task(string name, string description)
        {
            Name = name;
            Description = description;
            IsChecked = false;
        }

        public void Rename(string new_name)
        {
            this.Name = new_name;
        }

        public void ChangeDiscription(string new_descript)
        {
            this.Description = new_descript;
        }

        public void ChangeCheck()
        {
            this.IsChecked = !this.IsChecked;
        }
    }
}

using Scenario_Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scenario_Editor.Exceptions
{
    public class TaskNameConflictException : Exception
    {
        public Task ExistingTaks { get; }
        public Task IncomingTask { get; }

        public TaskNameConflictException(Task existingTaks, Task incomingTask)
        {
            ExistingTaks = existingTaks;
            IncomingTask = incomingTask;
        }

        public TaskNameConflictException(string message, Task existingTaks, Task incomingTask) : base(message)
        {
            ExistingTaks = existingTaks;
            IncomingTask = incomingTask;
        }

        public TaskNameConflictException(string message, Exception innerException, Task existingTaks, Task incomingTask) : base(message, innerException)
        {
            ExistingTaks = existingTaks;
            IncomingTask = incomingTask;
        }
    }
}

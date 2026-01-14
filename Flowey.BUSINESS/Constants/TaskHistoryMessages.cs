using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.Constants
{
    public static class TaskHistoryMessages
    {
        public const string TaskCreated = "Task created.";
        public const string StepChanged = "Step changed from '{0}' to '{1}'";
        public const string AssigneeChanged = "Assignee changed from '{0}' to '{1}'";
        public const string TaskAssigned = "Task assigned to '{0}'";
        public const string TaskUnassigned = "Task unassigned from '{0}'";
        public const string TaskUpdated = "Task details updated";
    }
}

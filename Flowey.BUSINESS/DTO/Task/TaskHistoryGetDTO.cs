using Flowey.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.DTO.Task
{
    public class TaskHistoryGetDTO
    {
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; }
        public HistoryChangeType ChangeType { get; set; }
        public string PropertyName { get; set; } 
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string DisplayMessage { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduling
{
    /// <summary>
    /// Could be a structure but creating a class so that in case behavior is required
    /// in future this can be added
    /// 
    /// To store start time, end time and recurrence duration, TimeSpan is used
    /// </summary>
    class TaskRecurrence
    {
        #region public properties
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan RecurrenceDuration { get; set; }
        #endregion
    }
}

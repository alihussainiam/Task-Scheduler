using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace TaskScheduling
{
    /// <summary>
    /// Implements the Free Space task to find out the free space of a disk drive
    /// in bytes
    /// </summary>
    class FreeSpaceTask : Task
    {
        System.Timers.Timer oTimer = null;
        
        private double interval;
        #region private data
        private string _driveName;
        #endregion

        #region contructor

        internal FreeSpaceTask(string taskName, string taskType, TaskRecurrence recurrence)
            : base(taskName, taskType, recurrence)
        {
            
            // do nothing; just call the base class constructor to set base data correctly
        }
        
        #endregion

        #region overrides of abstract and virtual methods

        /// <summary>
        /// Executes the free space computation task and provide the result in xml
        /// fragment in the output parameter
        /// </summary>
        /// <param name="result">the free space on the disk is provide in the xml
        /// fragment - [provide sample xml fragment here>]</param>
        /// <returns>returns true if the task is executed successfully else false</returns>

        public override bool ExecuteTask(out string result,DateTime start)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == _driveName)
                {
                    result = d.TotalFreeSpace.ToString();
                    new FileWriter().WriteResult(result, start, this);
                    return true;
                }
                
             }
            result = "";
            return false;
        }

        public override void Start()
        {
            interval = this.Recurrence.RecurrenceDuration.TotalMilliseconds;

            oTimer = new System.Timers.Timer(interval);
            oTimer.Elapsed += timer_Elapsed;

            // We don't want the timer to start ticking again till we tell it to.
            oTimer.AutoReset = false;

            while (true)
            {
                if (DateTime.Now.TimeOfDay > this.Recurrence.StartTime && DateTime.Now.TimeOfDay < this.Recurrence.EndTime)
                    oTimer.Start();
            }


        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string result;

            try
            {

                ExecuteTask(out result, DateTime.Now);

                // do amazing awesome mind blowing cool stuff
            }
            finally
            {
                //We put this in a finally block so it will still happen, even if an exception is thrown.
            }

            if (DateTime.Now.TimeOfDay < this.Recurrence.EndTime)
            {
                oTimer.Start();
            }
            else
                oTimer.Stop();

        }


        /// <summary>
        /// Sets the task specific details
        /// </summary>
        /// <param name="taskSpecificDetails">xml fragment of the task specific details</param>
        /// <returns>return true if the task specific details are provided and are valid</returns>

        public override bool ReadTaskSpecificDetails(string taskSpecificDetails)
        {
            // TODO: set value of drive name here _driveName and validate that the input
            // is a valid drive letter
            string drive = "";
            _driveName = taskSpecificDetails+":\\";
             drive=Path.GetPathRoot(_driveName);        
            return Directory.Exists(drive);
        }
        #endregion
        
    }
}

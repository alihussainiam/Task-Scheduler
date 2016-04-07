using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Xml;
using System.Configuration;
using System.Timers;

namespace TaskScheduling
{
   public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }

class MemoryUsedTask : Task
    {

        System.Timers.Timer oTimer = null;
       
        private double interval;

        #region private data       
        #endregion

        #region contructor
        internal MemoryUsedTask(string taskName, string taskType, TaskRecurrence recurrence)
            : base(taskName, taskType, recurrence)
        {
                        
            // do nothing; just call the base class constructor to set base data correctly
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
                  Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
                Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
                result = (tot - phav).ToString();
                result += " MiB";
            new FileWriter().WriteResult(result, start, this);
           
            return true;
        }
        public override void Start()
        {
            interval = this.Recurrence.RecurrenceDuration.TotalMilliseconds;
            
            oTimer = new Timer(interval);
            oTimer.Elapsed += timer_Elapsed;

            // We don't want the timer to start ticking again till we tell it to.
            oTimer.AutoReset = false;

            while (true)
            {
                if (DateTime.Now.TimeOfDay > this.Recurrence.StartTime && DateTime.Now.TimeOfDay < this.Recurrence.EndTime)
                    oTimer.Start();
            }
                       
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
            return true;
        }
        #endregion
        
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduling
{
   /// <summary>
    /// Abstract class to store information about a task and to execute a task
    /// </summary>
    abstract class Task
    {
        #region data members and properties
        private string _taskName;
        private string _taskType; // not needed probably
        public readonly TaskRecurrence Recurrence;
        #endregion

        public string getName()
        {
            return _taskName;
        }
        public string getTaskType()
        {
            return _taskType;
        }
        #region constructor

        /// <summary>
        /// Sets the privdate data and recurrence of the task
        /// </summary>
        /// <param name="taskName">The name of the task</param>
        /// <param name="taskType">task type - probably not necessary</param>
        /// <param name="recurrence">task recurrence</param>

        protected Task(string taskName, string taskType, TaskRecurrence recurrence)
        {
            _taskName = taskName;
            _taskType = taskType;
            Recurrence = recurrence;
        }
        
        #endregion

        #region virtual and abstract methods

        /// <summary>
        /// Reads task specific details from the given xml fragment and set data members
        /// accordingly.  Child classes should override if there are any task specific
        /// details expected.
        /// </summary>
        /// <param name="taskSpecificDetails">xml fragment of the task specific details</param>
        /// <returns>true is returned when task specific details are successfully loaded,
        /// else false is returned</returns>

        public virtual void Start()
        {

        }


        public virtual bool ReadTaskSpecificDetails(string taskSpecificDetails)
        {
            // by default do nothing
            return true;
        }

        /// <summary>
        /// Child classes must implement and execute the task and provide the result
        /// of the task as the output parameter
        /// </summary>
        /// <param name="result">The xml fragment of the result of the task</param>
        /// <returns>returns true if the task is execute successfully, else false</returns>
        public abstract bool ExecuteTask(out string result,DateTime start);


        #endregion

        #region static methods

        /// <summary>
        /// factory method to create the correct instance of the concrete Task object, i.e., 
        /// a valid sub-class of the Task object based on the tasktype
        /// </summary>
        /// <param name="taskName">the name of the task</param>
        /// <param name="taskType">the type of the task; this determines the sub-class
        /// to be created</param>
        /// <param name="taskSpecificDetails">task specific details, if any to be
        /// populated/set-up</param>
        /// <param name="recurrence">task recurrence details</param>
        /// <returns>returns the object of the sub-class of task being created, or null
        /// if task cannot be identified</returns>

        public static Task CreateTaskObject(string taskName, string taskType, 
            string taskSpecificDetails, TaskRecurrence recurrence)
        {
            taskType = taskType.ToLower();

            if (taskType == "freespace")
            {
                FreeSpaceTask freeSpaceTask = new FreeSpaceTask(taskName, taskType, recurrence);
                if (!freeSpaceTask.ReadTaskSpecificDetails(taskSpecificDetails))
                {
                    // TODO: raise an exeption or log somewhere
                    return null;
                }
                return freeSpaceTask;
            }

            if (taskType == "memoryused")
            {
                MemoryUsedTask memoryTask = new MemoryUsedTask(taskName, taskType, recurrence);
                if (!memoryTask.ReadTaskSpecificDetails(taskSpecificDetails))
                {
                    // TODO: raise an exeption or log somewhere
                    return null;
                }
                return memoryTask;
            }
            return null;
        }
        #endregion 
    }
}

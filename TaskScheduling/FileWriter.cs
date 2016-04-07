using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TaskScheduling
{
    class FileWriter
    {
        private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();

        public void WriteResult(string result, DateTime starttime,Task t)
        {
            
            lock_.EnterWriteLock();
            try
            {

                XmlDocument output = new XmlDocument();

                output.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["OutputFile"].ToString()));

                XmlElement newTaskRun = output.CreateElement("TaskRun");

                newTaskRun.SetAttribute("name", t.getName());//Name of TaskRun

                newTaskRun.SetAttribute("type", t.getTaskType()); //Type of Task Run

                newTaskRun.SetAttribute("time", XmlConvert.ToString(starttime, "HH:mm:ss"));


                XmlElement room = output.CreateElement("TaskResult");

                room.SetAttribute("result", result); //Value of Task to be Added Here

                newTaskRun.AppendChild(room);

                output.DocumentElement.InsertAfter(newTaskRun, output.DocumentElement.LastChild);

                output.Save(ConfigurationManager.AppSettings["OutputFile"].ToString());

            }
            finally
            {
                lock_.ExitWriteLock();
            }
        }

    }
}

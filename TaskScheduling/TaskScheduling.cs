using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace TaskScheduling
{
    public partial class TaskScheduling : ServiceBase
    {
        private List<Thread> threadPool = new List<Thread>();

        public TaskScheduling()
        {
            InitializeComponent();
            //eventLog1 = new System.Diagnostics.EventLog();
            //if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            //{
            //    System.Diagnostics.EventLog.CreateEventSource(
            //        "MySource", "MyNewLog");
            //}
            //eventLog1.Source = "MySource";
            //eventLog1.Log = "MyNewLog";
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            List<Task> taskList = new List<Task>();
            List<Thread> threadPool = new List<Thread>();
            string taskName = "";

            string taskType = "";
            string driveName = "";

            Task newTask = null;

            TaskRecurrence recurrence = null;

            XmlDocument xml = new XmlDocument();
            

            // Loading File and Getting Task Information to Perform
        
            xml.Load(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["InputFile"].ToString()));

            XmlElement root = xml.DocumentElement;
            
            
            foreach (XmlNode node in root.ChildNodes)
            {
                    taskName= node.Attributes["name"].Value;
                    taskType = node.Attributes["type"].Value;// node.Attributes["location"].Value;                             

                foreach (XmlNode temp in node.ChildNodes)
                {


                    if (temp.Name == "FreeSpace")
                    {
                            
                        driveName = temp.Attributes["disk"].Value;
                        //Console.WriteLine("Type {0} of {1}", text, text2);
                    }
                    else
                    {
                            //orderDate = XmlConvert.ToDateTime();
                        recurrence=new TaskRecurrence();
                        recurrence.StartTime = TimeSpan.Parse(temp.Attributes["starttime"].Value);
                        if (temp.Attributes["repeat"].Value != "none")
                        {
                            recurrence.RecurrenceDuration = TimeSpan.Parse(temp.Attributes["repeat"].Value);
                            if (temp.Attributes["endtime"].Value != "none")
                                recurrence.EndTime = TimeSpan.Parse(temp.Attributes["endtime"].Value);

                        }                            
                        else
                        {
                            recurrence.RecurrenceDuration = TimeSpan.Parse("0");
                            recurrence.EndTime = TimeSpan.Parse("0");
                        }
                           
                       
                    }                  

               }
                
                    newTask = Task.CreateTaskObject(taskName,taskType,driveName, recurrence);
                
                if (newTask != null)
                        taskList.Add(newTask);
                           
            }


            foreach (Task t in taskList)

            {
                //Console.WriteLine("{0} {1} {2}",t.Recurrence.EndTime.ToString(),t.Recurrence.StartTime.ToString(),t.Recurrence.RecurrenceDuration.ToString());
                Thread mythread = new Thread(t.Start);

                mythread.IsBackground = false;

                mythread.Start();

                threadPool.Add(mythread);


            }
            Thread.Sleep(600000);

            //Thread.WhenAll(taskList.ToArray());
           

        }

        protected override void OnStop()
        {
            foreach (Thread t in threadPool)
            {
                t.Abort();
            }
        }
    }
}

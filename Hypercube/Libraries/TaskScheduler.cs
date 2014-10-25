using System;
using System.Collections.Generic;
using System.Threading;
using Hypercube.Core;

namespace Hypercube.Libraries {
    public class Task {
        public DateTime LastRun { get; set; }
        public TimeSpan RunInterval { get; set; }
        public TaskMethod Method { get; set; }
    }

    public delegate void TaskMethod();

    public static class TaskScheduler {
        public static Dictionary<string, Task> ScheduledTasks = new Dictionary<string, Task>(StringComparer.InvariantCultureIgnoreCase);
        public static Thread TaskThread;
        public static object TaskLock = new object();

        public static void CreateTask(string taskName, TimeSpan interval, TaskMethod method) {
            lock (TaskLock) {
                if (ScheduledTasks.ContainsKey(taskName))
                    ScheduledTasks[taskName] = new Task {
                        LastRun = ServerCore.UnixEpoch,
                        Method = method,
                        RunInterval = interval
                    };
                else
                    ScheduledTasks.Add(taskName,
                        new Task { LastRun = ServerCore.UnixEpoch, Method = method, RunInterval = interval });
            }
        }

        public static void RunTasks() {
            while (ServerCore.Running) {
                lock (TaskLock) {
                    foreach (var task in ScheduledTasks) {
                        try {
                            if ((DateTime.UtcNow - task.Value.LastRun) < task.Value.RunInterval) 
                                continue;

                            task.Value.Method();
                            ScheduledTasks[task.Key].LastRun = DateTime.UtcNow;
                        } catch (Exception e) {
                            ServerCore.Logger.Log("Tasks", "Error occured: " + e.Message, LogType.Error);
                            ServerCore.Logger.Log("Tasks", e.StackTrace, LogType.Debug);
                        }
                    }
                }
                Thread.Sleep(5);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
                    for (int i = 0; i < ScheduledTasks.Count; i++) {
                        try {
                            var task = ScheduledTasks.ElementAt(i);
                            if ((DateTime.UtcNow - task.Value.LastRun) >= task.Value.RunInterval) {
                                task.Value.Method();
                                ScheduledTasks[task.Key].LastRun = DateTime.UtcNow;
                            }
                        }
                        catch {
                            
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}

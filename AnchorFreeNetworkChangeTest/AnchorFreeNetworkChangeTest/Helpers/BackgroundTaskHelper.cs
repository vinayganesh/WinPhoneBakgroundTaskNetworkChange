using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace AnchorFreeNetworkChangeTest.Helpers
{
    public static class BackgroundTaskHelper
    {
        public const string BackgroundTaskEntryPoint = "BackgroundTaskTest.NwTask";
        public const string BackgroundTaskName = "NwChangeBackgroundTask";
        private static bool  BackgroundTaskRegistered = false;
      
        //Register background task for network changes
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(String taskEntryPoint, String name, IBackgroundTrigger trigger,IBackgroundCondition condition)
        {
            if (TaskRequiresBackgroundAccess())
            {
                await BackgroundExecutionManager.RequestAccessAsync();
            }

            var builder = new BackgroundTaskBuilder();
            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            BackgroundTaskRegistration task = builder.Register();

            UpdateBackgroundTaskStatus(true);

            return task;
        }

        //Unregister background task
        public static void UnregisterBackgroundTasks(string name)
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == name)
                {
                    cur.Value.Unregister(true);
                }
            }
            UpdateBackgroundTaskStatus(false);
        }

        //Request access as all windows phone store apps should do this
        private static bool TaskRequiresBackgroundAccess()
        {
            return true;
        }

        //Set Backgroundtask status
        public static void UpdateBackgroundTaskStatus(bool registered)
        {
             BackgroundTaskRegistered = registered;
        }

        //Get Background task status
        public static bool GetBackgroundTaskStatus(String name)
        {
            var registered = false;
            registered = BackgroundTaskRegistered;
         
            var status = registered ? true : false;

            var settings = ApplicationData.Current.LocalSettings;
            
            if (settings.Values.ContainsKey(name))
            {
                status = (bool)settings.Values[name];
            }

            return status;
        }
    }
}

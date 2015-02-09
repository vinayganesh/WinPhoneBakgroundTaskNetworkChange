using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTaskTest
{
    public sealed class NwTask :IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            try
            {
                // Get connection profile
                ConnectionProfile internetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();

                if (internetConnectionProfile != null && internetConnectionProfile.IsWlanConnectionProfile)
                {
                    // Get Network SSID
                    var SSID = NetworkInformation.GetInternetConnectionProfile().WlanConnectionProfileDetails.GetConnectedSsid();
                    
                    // Notify the user with SSID
                    DisplayToast(SSID);

                    //Save SSID for UI
                    ApplicationData.Current.LocalSettings.Values["SSID"] = SSID;
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values.Clear();
                }
            }
            
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                deferral.Complete();
            }

        }

        //Notify the user 
        private static void DisplayToast(string message)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            XmlNodeList textElements = toastXml.GetElementsByTagName("text");
            textElements[0].AppendChild(toastXml.CreateTextNode("Network SSID IS"));
            textElements[1].AppendChild(toastXml.CreateTextNode(message));
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toastXml));
        }
    }
}

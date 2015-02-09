using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AnchorFreeNetworkChangeTest.Helpers;

namespace AnchorFreeNetworkChangeTest
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == BackgroundTaskHelper.BackgroundTaskName)
                {
                    BackgroundTaskHelper.UpdateBackgroundTaskStatus(true);
                    break;
                }
            }
          
            UpdateUI();
        }

        //Update the UI of all the changes
        private async void UpdateUI()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                CbxNetworkChange.IsChecked = BackgroundTaskHelper.GetBackgroundTaskStatus(BackgroundTaskHelper.BackgroundTaskName);
               
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SSID"))
                {
                    TblSSIDBlock.Text = SettingsHelper.SSID;
                    TblSSIDBlock.Visibility = Visibility.Visible;
                    TblSSIDLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    TblSSIDBlock.Visibility = Visibility.Collapsed;
                    TblSSIDLabel.Visibility = Visibility.Collapsed;
                }
            });
        }

        //Register background task when checkbox checked
        private async void CbxNetworkChange_OnChecked(object sender, RoutedEventArgs e)
        {
            var task = BackgroundTaskHelper.RegisterBackgroundTask(BackgroundTaskHelper.BackgroundTaskEntryPoint,
                BackgroundTaskHelper.BackgroundTaskName,
                new SystemTrigger(SystemTriggerType.NetworkStateChange, false),
                null);

            await task;
            task.Result.Completed += Result_Completed;
            UpdateUI();
        }

        void Result_Completed(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            UpdateUI();
        }

        //Unregister background task when checkbox unchecked
        private void CbxNetworkChange_OnUnchecked(object sender, RoutedEventArgs e)
        {
            BackgroundTaskHelper.UnregisterBackgroundTasks(BackgroundTaskHelper.BackgroundTaskName);
            Debug.WriteLine("BackgroundTask Unregistered");
            UpdateUI();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WishList.Models;
using WishList.ViewModels;
using Microsoft.Live; 
using Microsoft.Live.Controls; 

namespace WishList
{
    public partial class MainPage : PhoneApplicationPage
    {
        private LiveConnectClient liveClient; 
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }

        private void newItemAppBarButton_Click(object sender, EventArgs e)
        {
            (DataContext as WishViewModel).AddWish(); 
            NavigationService.Navigate(new Uri("/Views/WishPage.xaml", UriKind.Relative));
            

        }

        private void WishListListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var button = (sender as ListBox).SelectedItem as Wish;
            App.ViewModel.SelectedWish = button;
            if (button != null)
            {
                NavigationService.Navigate(new Uri("/Views/WishPage.xaml", UriKind.Relative));
            }
            return;

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            (DataContext as WishViewModel).SelectedWish = null;
            WishListListBox.SelectedItem = null; 
            base.OnNavigatedTo(e);
        }

        private void aboutMenuButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/About.xaml", UriKind.Relative));
        }

        private void OnSessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Status == LiveConnectSessionStatus.Connected)
                {

                    this.liveClient = new LiveConnectClient(e.Session);

                    this.GetMe();

                    /*var client = new LiveConnectClient(e.Session);
                    client.GetCompleted += (o, args) =>
                    {
                        if (args.Error == null)
                        {
                            MessageBox.Show(Convert.ToString(args.Result["name"]));
                        }
                    };
                    client.GetAsync("me"); */

                }
                else
                {
                    this.liveClient = null; 

                }
            }

        }

        private void GetMe()
        {
            this.liveClient.GetCompleted += OnGetMe;
            this.liveClient.GetAsync("me", null);
        }

        private void OnGetMe(object sender, LiveOperationCompletedEventArgs e)
        {
            this.liveClient.GetCompleted -= OnGetMe;
            if (e.Error == null)
            {
                string firstName = e.Result.ContainsKey("first_name") ? e.Result["first_name"] as string : string.Empty;
                string lastName = e.Result.ContainsKey("last_name") ? e.Result["last_name"] as string : string.Empty;

                //this.tbGreeting.Text = "Welcome " + firstName + " " + lastName;
            }
            else
            {
                //this.tbGreeting.Text = e.Error.ToString();
            }
        }

        private void uploadToSkyDriveButton_Click(object sender, EventArgs e)
        {

        }
    }

}
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

namespace WishList.Views
{
    public partial class WishPage : PhoneApplicationPage
    {
        

        public WishPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            
        }

        #region Save button click 
        private void saveWishAppBarButton_Click(object sender, EventArgs e)
        {
            // Confirm a title is provided 
            if (wishTxtBox.Text.Length == 0)
            {
                MessageBox.Show("Please give your wish a name.");
                return; 
            }
            else
            {
                (DataContext as WishViewModel).SelectedWish.wishTitle = wishTxtBox.Text;
                (DataContext as WishViewModel).SelectedWish.wishWhy = whyTxtBox.Text;
                //(DataContext as WishViewModel).SelectedWish.wishCost = costTxtBox.Text; 
                App.ViewModel.SaveChangesToDB();
            }

            if(NavigationService.CanGoBack)
            {
                NavigationService.GoBack(); 
            }

        }
        #endregion 

        #region Cancel button click 
        private void cancelAppBarButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

        }
        #endregion 

        #region Delete button click 
        private void deleteWishButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Are you sure you want to delete this wish?", "", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.Cancel)
            {
                return;
            }
            // Cast the parameter as a button.
            (DataContext as WishViewModel).DeleteWish();

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

        }
        #endregion 

        #region On Navigated To: Load controls with respective data 
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            if (DataContext != null)
            {
                if ((DataContext as WishViewModel).SelectedWish.wishTitle != null)
                {
                    wishTxtBox.Text = (DataContext as WishViewModel).SelectedWish.wishTitle; 
                }
                if((DataContext as WishViewModel).SelectedWish.wishWhy != null)
                    whyTxtBox.Text = (DataContext as WishViewModel).SelectedWish.wishWhy;

                //if ((DataContext as WishViewModel).SelectedWish.wishCost != null)
                //    costTxtBox.Text = (DataContext as WishViewModel).SelectedWish.wishCost; 
            }
            base.OnNavigatedTo(e);
        }
        #endregion 

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if ((DataContext as WishViewModel).SelectedWish != null)
            {
                if (((DataContext as WishViewModel).SelectedWish.wishTitle == null) && ((DataContext as WishViewModel).SelectedWish.wishWhy == null))
                {
                    // If both the name and why field are empty, delete the friggin' thing 
                    (DataContext as WishViewModel).DeleteWish();

                }
            }

            base.OnNavigatedFrom(e);
        }
    }
}
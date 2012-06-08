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

#region Directives for camera
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
#endregion

using Microsoft.Phone;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;
using WishList.Models;
using WishList.ViewModels;
using ExifLib;

namespace WishList.Views
{
    public partial class WishPage : PhoneApplicationPage
    {

        ExifLib.ExifOrientation _orientation;
        int _width;
        int _height; 
        int _angle;
        Stream capturedImage;

        #region A list for different ways a user can share a wish
        /* -------------------------------------------------------------
         * There's got to be a better way to do this but this is what I did.... 
         * 
         * This way, we can set the ItemsSource property of a ListPicker 
         * to the shareOptions list, then create a template and bind the items
         * to OptionName thus displaying all of the options for sharing a wish 
         ----------------------------------------------------------------*/ 
        List<ShareOption> shareOptions = new List<ShareOption>
        {
            new ShareOption
            { 
                OptionName = ""
            }, 

            new ShareOption
            {
                OptionName = "Social networks"
            }, 

            new ShareOption 
            {
                OptionName = "Email"
            } 
        };
        #endregion 

        public WishPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            //image = new Image();
            this.defaultListPicker.ItemsSource = shareOptions; 


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


                //byte[] byteArray = GetImageBytes(imageStream);
                //(DataContext as WishViewModel).SelectedWish.Image = _imageBytes; 
                //(DataContext as WishViewModel).SelectedWish.wishCost = costTxtBox.Text; 

                App.ViewModel.SaveChangesToDB();
            }

            if (NavigationService.CanGoBack)
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

            // Delete the image, don't want no memory leaks! 
            string wishFolder = (DataContext as WishViewModel).SelectedWish.wishTitle;
            string fileName = wishFolder + "_image";
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(fileName))
                {
                    myIsolatedStorage.DeleteFile(fileName);
                }

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
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
                if ((DataContext as WishViewModel).SelectedWish.wishWhy != null)
                    whyTxtBox.Text = (DataContext as WishViewModel).SelectedWish.wishWhy;

                string fileName = (DataContext as WishViewModel).SelectedWish.wishTitle + "_image";     // Load the image 
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (myIsolatedStorage.FileExists(fileName))
                    {
                        viewImageButton.Visibility = Visibility.Visible; 
                    }
                }
               
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

        private void shareWishList_Click(object sender, EventArgs e)
        {
            
            defaultListPicker.Open(); 

            
            
            
            
        }

        #region Camera click events
        private void cameraButton_Click(object sender, EventArgs e)
        {
            ShowCameraCaptureTask();    // prompt the camera 

        }

        private void ShowCameraCaptureTask()
        {
            var cameraTask = new CameraCaptureTask();       // Start the camera capture task 
            cameraTask.Completed += new EventHandler<PhotoResult>(cameraTask_Completed);
            cameraTask.Show();
        }

        private void cameraTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {

                string wishFolder = (DataContext as WishViewModel).SelectedWish.wishTitle;
                
                // Figure out the orientation from the EXIF data 
                e.ChosenPhoto.Position = 0;
                JpegInfo info = ExifReader.ReadJpeg(e.ChosenPhoto, e.OriginalFileName);

                _width = info.Width;
                _height = info.Height;
                _orientation = info.Orientation; 

                switch (info.Orientation)
                {
                    case ExifOrientation.TopLeft: 
                    case ExifOrientation.Undefined:
                        _angle = 0; 
                        break; 
                    case ExifOrientation.TopRight:
                        _angle = 90; 
                        break; 
                    case ExifOrientation.BottomRight:
                        _angle = 180; 
                        break;
                    case ExifOrientation.BottomLeft:
                        _angle = 270; 
                        break; 
                }
                if (_angle > 0d)
                {
                    capturedImage = RotateStream(e.ChosenPhoto, _angle);
                }
                else
                {
                    capturedImage = e.ChosenPhoto;
                }
                
                SaveToLocalStorage(capturedImage, wishFolder + "_image"); 
            }
        }


        // Having captured the image bytes, save the image to isolated storage 
        // ImageFolder = WishName 
        // imageFileName = [WishName]Image 
        private void SaveToLocalStorage(Stream imageStream, string fileName)
        {
            // Create virtual store and file stream. 
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(fileName))
                {
                    myIsolatedStorage.DeleteFile(fileName);
                }
                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName);
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(imageStream);

                WriteableBitmap wb = new WriteableBitmap(bitmap);
                wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);

                fileStream.Close();
                viewImageButton.Visibility = Visibility.Visible; 
            }
        }

        private WriteableBitmap LoadFromLocalStorage(string fileName)
        {

         
            WriteableBitmap bitmap = new WriteableBitmap(200, 200); 
            using(IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!myIsolatedStorage.FileExists(fileName))
                {
                    return null; 
                }
                using(IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Decode the JPEG stream.
                    bitmap = PictureDecoder.DecodeJpeg(fileStream);
                    return bitmap; 
                }
            }
            //this.img.Source = bitmap; 
        }
        #endregion

        private void viewImageButton_Click(object sender, RoutedEventArgs e)
        {
            // The image is already showing and the button is clicked 
            if ((viewImageButton.Content.ToString()) == "Return to wish")
            {
                viewImageButton.Content = "View image";   // Change button content back to normal 

                #region Make all of underlying controls visible
                PageTitle.Visibility = Visibility.Visible;
                textBlock1.Visibility = Visibility.Visible;
                textBlock2.Visibility = Visibility.Visible;
                wishTxtBox.Visibility = Visibility.Visible; 
                whyTxtBox.Visibility = Visibility.Visible;
                #endregion 

                this.img.Source = null;         // Stop displaying the image 
                
            }
            // The image is not being shown, the button content is "View image" 
            else
            {   
                
                string fileName = (DataContext as WishViewModel).SelectedWish.wishTitle + "_image";     // Load the image 
                WriteableBitmap bitmap = LoadFromLocalStorage(fileName);

                #region Hide all of the underlying controls for asthetic purposes 
                PageTitle.Visibility = Visibility.Collapsed;
                textBlock1.Visibility = Visibility.Collapsed;
                textBlock2.Visibility = Visibility.Collapsed;
                wishTxtBox.Visibility = Visibility.Collapsed; 
                whyTxtBox.Visibility = Visibility.Collapsed;
                #endregion 

                this.img.Source = bitmap;       // Display the image
                viewImageButton.Content = "Return to wish";
            }

            
        }
        private Stream RotateStream(Stream stream, int angle)
        {
            stream.Position = 0;
            if (angle % 90 != 0 || angle < 0) throw new ArgumentException();
            if (angle % 360 == 0) return stream;

            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            WriteableBitmap wbSource = new WriteableBitmap(bitmap);

            WriteableBitmap wbTarget = null;
            if (angle % 180 == 0)
            {
                wbTarget = new WriteableBitmap(wbSource.PixelWidth, wbSource.PixelHeight);
            }
            else
            {
                wbTarget = new WriteableBitmap(wbSource.PixelHeight, wbSource.PixelWidth);
            }

            for (int x = 0; x < wbSource.PixelWidth; x++)
            {
                for (int y = 0; y < wbSource.PixelHeight; y++)
                {
                    switch (angle % 360)
                    {
                        case 90:
                            wbTarget.Pixels[(wbSource.PixelHeight - y - 1) + x * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 180:
                            wbTarget.Pixels[(wbSource.PixelWidth - x - 1) + (wbSource.PixelHeight - y - 1) * wbSource.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                        case 270:
                            wbTarget.Pixels[y + (wbSource.PixelWidth - x - 1) * wbTarget.PixelWidth] = wbSource.Pixels[x + y * wbSource.PixelWidth];
                            break;
                    }
                }
            }
            MemoryStream targetStream = new MemoryStream();
            wbTarget.SaveJpeg(targetStream, wbTarget.PixelWidth, wbTarget.PixelHeight, 0, 100);
            return targetStream;
        }

        private void defaultPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();
            
            if (defaultListPicker.SelectedIndex == 1)
            {
                shareStatusTask.Status = "One of my wishes is " + (DataContext as WishViewModel).SelectedWish.wishTitle + " via Wish List http://www.windowsphone.com/en-US/apps/45badf1a-bfe0-48a1-bd4c-9d3a50289e0d";
                shareStatusTask.Show();
            }
            else if (defaultListPicker.SelectedIndex == 2)
            {
                // Compose an email 
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                string Wish = "One of my wishes is ";

                Wish += (DataContext as WishViewModel).SelectedWish.wishTitle + " because " + (DataContext as WishViewModel).SelectedWish.wishWhy; 
                emailComposeTask.Subject = "I wish...";
                emailComposeTask.Body = Wish + "\n\nvia Wish List for Windows Phone \n http://www.windowsphone.com/en-US/apps/45badf1a-bfe0-48a1-bd4c-9d3a50289e0d";

                emailComposeTask.Show(); 
            }
            else
            {
                return;
            } 
        }

    }
}
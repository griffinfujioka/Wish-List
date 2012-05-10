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
using Microsoft.Phone.Controls.Primitives; 
using Microsoft.Phone.Tasks; 

namespace WishList.Views
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        private void hyperlinkButton1_Click(object sender, RoutedEventArgs e)
        {

            WebBrowserTask wbt = new WebBrowserTask
            {
                URL = "http://www.twitter.com/griffinfujioka"
            };
            wbt.Show(); 
               

        }

        private void hyperlinkButton2_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask
            {
                URL = "http://www.twitter.com/DavidHeyduck"
            };
            wbt.Show(); 
               
        }

       
    }
}
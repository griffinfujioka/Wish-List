using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Devices;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls;
using System.IO;
using System.IO.IsolatedStorage;
using WishList.Views;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging; 

namespace WishList.Models
{
    [Table]
    public class Wish : INotifyPropertyChanged, INotifyPropertyChanging
    {
        
        // Define ID: private field, public property, and database column.
        private int _wishId;
        

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int wishId
        {
            get { return _wishId; }
            set
            {
                if (_wishId != value)
                {
                    NotifyPropertyChanging("_wishId");
                    _wishId = value;
                    NotifyPropertyChanged("_wishId");
                }
            }
        }

        private string _wishTitle; 
        [Column]
        public string wishTitle
        {
            get { return _wishTitle; }
            set
            {
                if (_wishTitle != value)
                {
                    NotifyPropertyChanging("wishTitle");
                    _wishTitle = value;
                    NotifyPropertyChanged("wishTitle");
                }
            }

        }

        private string _wishWhy;
        [Column]
        public string wishWhy
        {
            get { return _wishWhy; }
            set
            {
                if (_wishWhy != value)
                {
                    NotifyPropertyChanging("wishWhy");
                    _wishWhy = value;
                    NotifyPropertyChanged("wishWhy");
                }
            }

        }


        //private string _wishCost;
        //[Column]
        //public string wishCost
        //{
        //    get { return _wishCost; }
        //    set
        //    {
        //        if (_wishCost != value)
        //        {
        //            NotifyPropertyChanging("wishCost");
        //            _wishCost = value;
        //            NotifyPropertyChanging("wishCost");
        //        }
        //    }
        //}

        private byte[] _image;
        [Column]
        public byte[] Image
        {
            get { return _image; }
            set 
            {
                
                _image = value;
                NotifyPropertyChanging("Image");
                NotifyPropertyChanged("Image"); 
            } 
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

        


    }

    public class WishDataContext : DataContext
    {
        public WishDataContext(string connectionString)
            : base(connectionString) { }

        public Table<Wish> Wishes; 
    }
}
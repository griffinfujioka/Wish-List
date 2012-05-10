using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using WishList.Models;

namespace WishList.ViewModels
{
    public class WishViewModel : INotifyPropertyChanged
    {
        //private Wish _Model; 
        private WishDataContext WishesDB; 
       

        // Class constructor, create the data context object.
        public WishViewModel(string DBConnectionString)
        {
            // App.ViewModel.Model; 

            // Open up a connection to the database 
            
            WishesDB = new WishDataContext(DBConnectionString);
            
        }

        public WishViewModel(WishDataContext db)
        {
            WishesDB = db;
        }

        public void SaveChangesToDB()
        {
            WishesDB.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        private ObservableCollection<Wish> _Wishes; 
        public ObservableCollection<Wish> Wishes
        {
            get { return _Wishes; }
            set
            {
                _Wishes = value;
                NotifyPropertyChanged("Wishes");
            }

        }

        private Wish _selectedWish;
        public Wish SelectedWish
        {
            get { return _selectedWish; }
            set
            {
                _selectedWish = value;
                NotifyPropertyChanged("SelectedWish");
            }
        }



        // Query database and load the collections and list used by the pivot pages.
        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all to-do items in the database.
            var WishesInDB = from Wish wish in WishesDB.Wishes select wish; 

            // Query the database and load all to-do items.
            Wishes = new ObservableCollection<Wish>(WishesInDB); 

        }

        public void AddWish()
        {
            var newWish = new Wish();
            SelectedWish = newWish;
            WishesDB.Wishes.InsertOnSubmit(newWish);
            WishesDB.SubmitChanges();
            Wishes.Add(newWish); 


        }

        public void AddWish(Wish newWish)
        {
            
            // Add to data context 
            WishesDB.Wishes.InsertOnSubmit(newWish);

            // Save changes to db
            WishesDB.SubmitChanges();

            // Add to observable collection
            Wishes.Add(newWish);
        }

        public void DeleteWish()
        {
            var wishForDelete = this.SelectedWish; 
            
            // Remove from the observable collection 
            Wishes.Remove(wishForDelete); 

            // Remove from the data context 
            WishesDB.Wishes.DeleteOnSubmit(wishForDelete); 

            // Save the database 
            WishesDB.SubmitChanges(); 

        }
    }

}
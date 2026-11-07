using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Library_CP.database;

namespace Library_CP.viewmodels
{
    public partial class BooksViewModel : ObservableObject
    {
        private ObservableCollection<Books> _booksList;
        public ObservableCollection<Books> BooksList
        {
            get { return _booksList; }
            set { SetProperty(ref _booksList, value); }
        }

        public BooksViewModel()
        {
            using (var db = new LibraryModel())
            {
                var booksFromDb = db.Books.ToList();
                BooksList = new ObservableCollection<Books>(booksFromDb);
            }
        }
    }
}
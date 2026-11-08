using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;
using Library_CP.helpers;

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
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                LoadBooks();
            }
        }

        public IRelayCommand LoadBooksCommand { get; }
        public IRelayCommand<Books> BorrowBookCommand { get; }

        public BooksViewModel()
        {
            LoadBooksCommand = new RelayCommand(LoadBooks);
            BorrowBookCommand = new RelayCommand<Books>(BorrowBook);
            LoadBooks();
        }

        private void LoadBooks()
        {
            using (var db = new BiblioSysDBEntities())
            {
                var query = db.Books.AsQueryable();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    string search = SearchText.ToLower();
                    query = query.Where(b => b.Title.ToLower().Contains(search) ||
                                             b.Writers.WriterName.ToLower().Contains(search));
                }

                var booksFromDb = query.ToList();
                BooksList = new ObservableCollection<Books>(booksFromDb);
            }
        }

        private void BorrowBook(Books selectedBook)
        {
            if (selectedBook == null) return;

            if (Session.CurrentUser == null)
            {
                MessageBox.Show("Пожалуйста, войдите в систему!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedBook.QuantityInStock <= 0)
            {
                MessageBox.Show("Этой книги сейчас нет в наличии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                try
                {
                    var newOrder = new Orders
                    {
                        UserID = Session.CurrentUser.UserID,
                        OrderDate = System.DateTime.Now,
                        StatusID = 1
                    };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    var newOrderItem = new OrderItems
                    {
                        OrderID = newOrder.OrderID,
                        BookID = selectedBook.BookID,
                        Quantity = 1
                    };
                    db.OrderItems.Add(newOrderItem);

                    var bookInDb = db.Books.Find(selectedBook.BookID);
                    if (bookInDb != null)
                    {
                        bookInDb.QuantityInStock -= 1;
                    }

                    db.SaveChanges();

                    MessageBox.Show($"Книга «{selectedBook.Title}» успешно забронирована!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBooks();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при бронировании:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;
using Library_CP.helpers;

namespace Library_CP.viewmodels
{
    public class MyBookItem
    {
        public string Title { get; set; }
        public DateTime? OrderDate { get; set; }
        public string StatusName { get; set; }
    }

    public partial class MyBooksViewModel : ObservableObject
    {
        private ObservableCollection<MyBookItem> _myBooks;
        public ObservableCollection<MyBookItem> MyBooks
        {
            get { return _myBooks; }
            set { SetProperty(ref _myBooks, value); }
        }

        public IRelayCommand LoadMyBooksCommand { get; }

        public MyBooksViewModel()
        {
            LoadMyBooksCommand = new RelayCommand(LoadMyBooks);
            LoadMyBooks();
        }

        private void LoadMyBooks()
        {
            if (Session.CurrentUser == null) return;

            MyBooks = new ObservableCollection<MyBookItem>();

            using (var db = new BiblioSysDBEntities())
            {
                var orders = db.Orders
                    .Where(o => o.UserID == Session.CurrentUser.UserID)
                    .ToList();

                foreach (var order in orders)
                {
                    var orderItems = db.OrderItems.Where(oi => oi.OrderID == order.OrderID).ToList();

                    foreach (var item in orderItems)
                    {
                        var book = db.Books.Find(item.BookID);
                        var status = db.Statuses.Find(order.StatusID);

                        if (book != null)
                        {
                            MyBooks.Add(new MyBookItem
                            {
                                Title = book.Title,
                                OrderDate = order.OrderDate,
                                StatusName = status != null ? status.StatusName : "Неизвестно"
                            });
                        }
                    }
                }
            }
        }
    }
}

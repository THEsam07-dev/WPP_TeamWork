using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;
using Library_CP.helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Library_CP.viewmodels
{
    public class MyBookItem
    {
        public int OrderID { get; set; }
        public int BookID { get; set; }
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
        public IRelayCommand<MyBookItem> ReturnBookCommand { get; }

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
        private void ReturnBook(MyBookItem selectedItem)
        {
            if(selectedItem == null) return;

            if (selectedItem.StatusName == "Возвращена" || selectedItem.StatusName == "Returned")
            {
                MessageBox.Show("Эта книга уже возвращена!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                try
                {
                    var order = db.Orders.Find(selectedItem.OrderID);
                    if (order != null)
                    {
                        order.StatusID = 2;
                    }
                    var book = db.Books.Find(selectedItem.BookID);
                    if (book != null)
                    {
                        book.QuantityInStock += 1;
                    }

                    db.SaveChanges();

                    MessageBox.Show($"Книга «{selectedItem.Title}» успешно возвращена на склад!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadMyBooks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при возврате:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

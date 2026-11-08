using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Library_CP.database;

namespace Library_CP.viewmodels
{
    public class LoanItem
    {
        public int OrderID { get; set; }
        public string ReaderName { get; set; }
        public string BookTitle { get; set; }
        public DateTime? OrderDate { get; set; }
        public string StatusName { get; set; }
    }
    public partial class AllLoansViewModel : ObservableObject
    {
        private ObservableCollection<LoanItem> _loansList;
        public ObservableCollection<LoanItem> LoansList
        {
            get { return _loansList; }
            set { SetProperty(ref _loansList, value); }
        }
        public AllLoansViewModel()
        {
            LoadAllLoans();
        }

        private void LoadAllLoans()
        {
            LoansList = new ObservableCollection<LoanItem>();

            using (var db = new BiblioSysDBEntities())
            {
                var orders = db.Orders.OrderByDescending(o => o.OrderDate).ToList();

                foreach (var order in orders)
                {
                    var user = db.Users.Find(order.UserID);

                    var status = db.Statuses.Find(order.StatusID);

                    var orderItems = db.OrderItems.Where(oi => oi.OrderID == order.OrderID).ToList();

                    foreach (var item in orderItems)
                    {
                        var book = db.Books.Find(item.BookID);
                        LoansList.Add(new LoanItem
                        {
                            OrderID = order.OrderID,
                            ReaderName = user != null ? $"{user.LastName} {user.FirstName}" : "Неизвестный",
                            BookTitle = book != null ? book.Title : "Книга удалена",
                            OrderDate = order.OrderDate,
                            StatusName = status != null ? status.StatusName : "Неизвестно"
                        });
                    }
                }
            }
        }
    }
}

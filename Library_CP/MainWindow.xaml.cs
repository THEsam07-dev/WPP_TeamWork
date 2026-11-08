using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Library_CP.views.pages;
using Library_CP.helpers;

namespace Library_CP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (Session.CurrentUser != null)
            {
                TxtUserName.Text = $"{Session.CurrentUser.FirstName} {Session.CurrentUser.LastName}";
                TxtUserRole.Text = Session.CurrentUser.Roles?.RoleName ?? "Пользователь";
                if (Session.CurrentUser.RoleID == 1)
                {
                    BtnAddBook.Visibility = Visibility.Visible;
                    BtnAllLoans.Visibility = Visibility.Visible;
                }
            }

            MainFrame.Navigate(new BooksPage());
        }

        private void BtnBooks_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new BooksPage());
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new MyBooksPage());
        private void BtnAddBook_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AddBookPage());
        private void BtnAllLoans_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AllLoansPage());
    }
}

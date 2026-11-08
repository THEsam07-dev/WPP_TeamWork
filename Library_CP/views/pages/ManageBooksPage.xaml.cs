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
using Library_CP.database;
using Library_CP.viewmodels;

namespace Library_CP.views.pages
{
    /// <summary>
    /// Логика взаимодействия для ManageBooksPage.xaml
    /// </summary>
    public partial class ManageBooksPage : Page
    {
        private ManageBooksViewModel _viewModel;

        public ManageBooksPage()
        {
            InitializeComponent();
            _viewModel = new ManageBooksViewModel();
            DataContext = _viewModel;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button?.DataContext as Books;

            if (book != null)
            {
                EditBookWindow editWindow = new EditBookWindow(book);
                editWindow.Owner = Window.GetWindow(this);

                if (editWindow.ShowDialog() == true)
                {
                    _viewModel.LoadBooks();
                }
            }
        }
    }
}

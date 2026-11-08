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
using System.Windows.Shapes;
using Library_CP.database;

namespace Library_CP
{
    /// <summary>
    /// Логика взаимодействия для EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        private Books _book;
        public EditBookWindow(Books book)
        {
            InitializeComponent();
            _book = book;

            TxtTitle.Text = _book.Title;
            TxtISBN.Text = _book.ISBN;
            TxtQuantity.Text = _book.QuantityInStock.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text) || string.IsNullOrWhiteSpace(TxtISBN.Text))
            {
                MessageBox.Show("Название и ISBN не могут быть пустыми!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int qty))
            {
                MessageBox.Show("Количество должно быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                var bookToUpdate = db.Books.Find(_book.BookID);
                if (bookToUpdate != null)
                {
                    bookToUpdate.Title = TxtTitle.Text;
                    bookToUpdate.ISBN = TxtISBN.Text;
                    bookToUpdate.QuantityInStock = qty;

                    db.SaveChanges();
                }
            }
            this.DialogResult = true;
            this.Close();
        }
    }
}

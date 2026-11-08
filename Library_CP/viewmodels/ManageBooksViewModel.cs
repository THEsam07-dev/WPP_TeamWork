using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;

namespace Library_CP.viewmodels
{
    public partial class ManageBooksViewModel : ObservableObject
    {
        private ObservableCollection<Books> _booksList;
        public ObservableCollection<Books> BooksList
        {
            get { return _booksList; }
            set { SetProperty(ref _booksList, value); }
        }

        public IRelayCommand LoadBooksCommand { get; }
        public IRelayCommand<Books> DeleteBookCommand { get; }

        public ManageBooksViewModel()
        {
            LoadBooksCommand = new RelayCommand(LoadBooks);
            DeleteBookCommand = new RelayCommand<Books>(DeleteBook);
            LoadBooks();
        }
        public void LoadBooks()
        {
            using (var db = new BiblioSysDBEntities())
            {
                var books = db.Books.ToList();
                BooksList = new ObservableCollection<Books>(books);
            }
        }

        private void DeleteBook(Books selectedBook)
        {
            if (selectedBook == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить книгу «{selectedBook.Title}»?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new BiblioSysDBEntities())
            {
                try
                {
                    var hasOrders = db.OrderItems.Any(oi => oi.BookID == selectedBook.BookID);
                    if (hasOrders)
                    {
                        MessageBox.Show("Нельзя удалить книгу, так как она фигурирует в истории выдач. Вы можете изменить её количество на 0.",
                                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var bookToDelete = db.Books.Find(selectedBook.BookID);
                    if (bookToDelete != null)
                    {
                        db.Books.Remove(bookToDelete);
                        db.SaveChanges();
                    }

                    MessageBox.Show("Книга успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBooks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
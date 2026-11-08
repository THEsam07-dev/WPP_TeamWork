using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;

namespace Library_CP.viewmodels
{
    public partial class AddBookViewModel : ObservableObject
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public byte[] ImageData { get; set; }

        public Writers SelectedWriter { get; set; }
        public Publishers SelectedPublisher { get; set; }
        public Genres SelectedGenre
        {
            get; set;
        }

        public ObservableCollection<Writers> WritersList { get; set; }
        public ObservableCollection<Publishers> PublishersList { get; set; }
        public ObservableCollection<Genres> GenresList { get; set; }

        public IRelayCommand SaveBookCommand { get; }

        public AddBookViewModel()
        {
            SaveBookCommand = new RelayCommand(SaveBook);

            using (var db = new BiblioSysDBEntities())
            {
                WritersList = new ObservableCollection<Writers>(db.Writers.ToList());
                PublishersList = new ObservableCollection<Publishers>(db.Publishers.ToList());
                GenresList = new ObservableCollection<Genres>(db.Genres.ToList());
            }
        }

        private void SaveBook()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(ISBN) ||
                SelectedWriter == null || SelectedPublisher == null || SelectedGenre == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                if (db.Books.Any(b => b.ISBN == ISBN))
                {
                    MessageBox.Show("Книга с таким ISBN уже существует в базе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                try
                {
                    var newBook = new Books
                    {
                        ISBN = ISBN,
                        Title = Title,
                        Description = Description,
                        QuantityInStock = Quantity,
                        WriterID = SelectedWriter.WriterID,
                        PublisherID = SelectedPublisher.PublisherID,
                        GenreID = SelectedGenre.GenreID,
                        UnitID = 1,
                        CoverImage = ImageData
                    };

                    db.Books.Add(newBook);
                    db.SaveChanges();

                    MessageBox.Show($"Книга «{Title}» успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

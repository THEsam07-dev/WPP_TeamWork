using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Library_CP.viewmodels;

namespace Library_CP.views.pages
{
    public partial class AddBookPage : Page
    {
        private AddBookViewModel _viewModel;

        public AddBookPage()
        {
            InitializeComponent();
            _viewModel = new AddBookViewModel();
            DataContext = _viewModel;
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите обложку книги"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _viewModel.ImageData = File.ReadAllBytes(openFileDialog.FileName);
                TxtImageStatus.Text = Path.GetFileName(openFileDialog.FileName);
            }
        }
    }
}
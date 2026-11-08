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
using Library_CP.helpers;

namespace Library_CP
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                TxtError.Text = "Пожалуйста, заполните все поля!";
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                var user = db.Users.FirstOrDefault(u => u.Login.ToLower() == login.ToLower() && u.PasswordHash == password);

                if (user != null)
                {
                    Session.CurrentUser = user;
                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    TxtError.Text = "Неверный логин или пароль!";
                }
            }
        }
    }
}

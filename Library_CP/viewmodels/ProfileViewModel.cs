using System;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;
using Library_CP.helpers;

namespace Library_CP.viewmodels
{
    public partial class ProfileViewModel : ObservableObject
    {
        public string FullName { get; }
        public string LibraryCardNumber { get; }
        public string RoleName { get; }
        public string Login { get; }

        public ProfileViewModel()
        {
            if (Session.CurrentUser != null)
            {
                FullName = $"{Session.CurrentUser.LastName} {Session.CurrentUser.FirstName}";
                LibraryCardNumber = Session.CurrentUser.LibraryCardNumber;
                RoleName = Session.CurrentUser.Roles?.RoleName ?? "Пользователь";
                Login = Session.CurrentUser.Login;
            }
        }

        public void ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (Session.CurrentUser == null) return;

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля для смены пароля!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Новые пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (newPassword.Length < 3)
            {
                MessageBox.Show("Пароль должен быть не менее 3 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                var user = db.Users.Find(Session.CurrentUser.UserID);

                if (user != null)
                {
                    if (user.PasswordHash != oldPassword)
                    {
                        MessageBox.Show("Старый пароль введен неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    user.PasswordHash = newPassword;
                    db.SaveChanges();

                    Session.CurrentUser.PasswordHash = newPassword;

                    MessageBox.Show("Пароль успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
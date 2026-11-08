using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_CP.database;

namespace Library_CP.viewmodels
{
    public partial class ManageUsersViewModel : ObservableObject
    {
        private ObservableCollection<Users> _usersList;
        public ObservableCollection<Users> UsersList
        {
            get { return _usersList; }
            set { SetProperty(ref _usersList, value); }
        }

        public IRelayCommand LoadUsersCommand { get; }
        public IRelayCommand<Users> DeleteUserCommand { get; }
        public IRelayCommand<Users> ToggleRoleCommand { get; }

        public ManageUsersViewModel()
        {
            LoadUsersCommand = new RelayCommand(LoadUsers);
            DeleteUserCommand = new RelayCommand<Users>(DeleteUser);
            ToggleRoleCommand = new RelayCommand<Users>(ToggleRole);
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (var db = new BiblioSysDBEntities())
            {
                var users = db.Users.ToList();
                UsersList = new ObservableCollection<Users>(users);
            }
        }

        private void ToggleRole(Users selectedUser)
        {
            if (selectedUser == null) return;

            if (selectedUser.UserID == helpers.Session.CurrentUser.UserID)
            {
                MessageBox.Show("Вы не можете изменить свою собственную роль!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new BiblioSysDBEntities())
            {
                var userToUpdate = db.Users.Find(selectedUser.UserID);
                if (userToUpdate != null)
                {
                    userToUpdate.RoleID = userToUpdate.RoleID == 2 ? 1 : 2;
                    db.SaveChanges();

                    MessageBox.Show("Роль пользователя успешно изменена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                }
            }
        }
        private void DeleteUser(Users selectedUser)
        {
            if (selectedUser == null) return;

            if (selectedUser.UserID == helpers.Session.CurrentUser.UserID)
            {
                MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Удалить пользователя {selectedUser.LastName} {selectedUser.FirstName}?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using (var db = new BiblioSysDBEntities())
            {
                try
                {
                    var hasOrders = db.Orders.Any(o => o.UserID == selectedUser.UserID);
                    if (hasOrders)
                    {
                        MessageBox.Show("Нельзя удалить пользователя, так как у него есть история выдач!",
                                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    var userToDelete = db.Users.Find(selectedUser.UserID);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                    }
                    
                    MessageBox.Show("Пользователь удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

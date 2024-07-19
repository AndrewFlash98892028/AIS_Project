using AIS.Entities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    /// <summary>
    /// Логика взаимодействия для UserManagment.xaml
    /// </summary>
    public partial class UserManagment : Page
    {
        private ObservableCollection<Users> users;

        public UserManagment()
        {
            InitializeComponent();
            LoadUsers();
           
        }
        private void LoadUsers()
        {
            using (var context = new AISCEntities1())
            {
                users = new ObservableCollection<Users>(context.Users.ToList());
                dgUsers.ItemsSource = users;
            }
        }

        

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUsersWindow addUserWindow = new AddUsersWindow();
            if (addUserWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is Users selectedUser)
            {
                EditUsersWindow editUserWindow = new EditUsersWindow(selectedUser);
                if (editUserWindow.ShowDialog() == true)
                {
                    LoadUsers();
                }
            }
        }

        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is Users selectedUser)
            {
                if (MessageBox.Show($"Вы уверенны что хотите удалить пользователя '{selectedUser.Username}'?",
                                    "Подтверждаю", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var context = new AISCEntities1())
                    {
                        var user = context.Users.Find(selectedUser.UserID);
                        if (user != null)
                        {
                            context.Users.Remove(user);
                            context.SaveChanges();
                            LoadUsers();
                        }
                    }
                }
            }

        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditUser.IsEnabled = dgUsers.SelectedItem != null;
            btnDeleteUser.IsEnabled = dgUsers.SelectedItem != null;

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            using (var context = new AISCEntities1())
            {
                users = new ObservableCollection<Users>(context.Users
                                    .Where(u => u.Username.ToLower().Contains(searchText))
                                    .ToList());
                dgUsers.ItemsSource = users;
            }

        }
    }
}

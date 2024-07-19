using AIS.Entities;
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

namespace AIS.Admin
{
    /// <summary>
    /// Логика взаимодействия для AddUsersWindow.xaml
    /// </summary>
    public partial class AddUsersWindow : Window
    {
        public AddUsersWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                using (var context = new AISCEntities1())
                {
                    var newUser = new Users
                    {
                        Username = txtUsername.Text,
                        Password = HashPassword(txtPassword.Password), 
                        RoleID = int.Parse(txtRoleID.Text)
                    };

                    context.Users.Add(newUser);
                    context.SaveChanges();

                    DialogResult = true;
                    this.Close();
                }
            }

        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Пожалуйста введите имя пользователя.");
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Пожалуйста введите пароль.");
                return false;
            }

            if (!int.TryParse(txtRoleID.Text, out int roleId))
            {
                MessageBox.Show("Пожалуйста введите номер роли.");
                return false;
            }

           

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();

        }
        private string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }
    }
}

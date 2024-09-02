using AIS.Entities;
using System.Windows;

namespace AIS.Admin
{
    /// <summary>
    /// Логика взаимодействия для EditUsersWindow.xaml
    /// </summary>
    public partial class EditUsersWindow : Window
    {
        private Users userToEdit;
        public EditUsersWindow(Users user) 
        {
            InitializeComponent();
            userToEdit = user;

            
            txtUsername.Text = userToEdit.Username;
            txtPassword.Password = userToEdit.Password;  
            txtRoleID.Text = userToEdit.RoleID.ToString();
        }
        public EditUsersWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                using (var context = new AISCEntities1())
                {
                   
                    var existingUser = context.Users.Find(userToEdit.UserID);

                    if (existingUser != null)
                    {
                       
                        existingUser.Username = txtUsername.Text;
                        existingUser.Password = HashPassword(txtPassword.Password);
                        existingUser.RoleID = int.Parse(txtRoleID.Text);

                      
                        context.SaveChanges();

                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден.");
                    }
                }
            }



        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Введите имя пользователя.");
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Введите пароль.");
                return false;
            }

            if (!int.TryParse(txtRoleID.Text, out int roleId))
            {
                MessageBox.Show("Введите верный номер роли.");
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

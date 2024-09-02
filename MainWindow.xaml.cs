using AIS.Admin;
using AIS.Employee;
using AIS.HR;
using System.Linq;
using System.Windows;
using System;
using System.Windows.Navigation;


namespace AIS
{
    public partial class MainWindow : Window
    {
        private int employeeID;

        public MainWindow()
        {
            InitializeComponent();
            RehashAllPasswordsPerMonth();
        }

        private void RehashAllPasswordsPerMonth()
        {
            try
            {
                using (var context = new Entities.AISCEntities1())
                {
                    var passwordHashInfo = context.PasswordHashInfo.FirstOrDefault();

                    if (passwordHashInfo == null || passwordHashInfo.LastHashDate.Month != DateTime.Now.Month)
                    {
                        RehashAllPasswords(context);

                        if (passwordHashInfo == null)
                        {
                            passwordHashInfo = new Entities.PasswordHashInfo { LastHashDate = DateTime.Now };
                            context.PasswordHashInfo.Add(passwordHashInfo);
                        }
                        else
                        {
                            passwordHashInfo.LastHashDate = DateTime.Now;
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void RehashAllPasswords(Entities.AISCEntities1 context)
        {
            var users = context.Users.ToList();

            foreach (var user in users)
            {
                user.Password = HashPassword(user.Password);
            }

            context.SaveChanges();
            Console.WriteLine("Пароли перехэшированы");
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (!chkAgree.IsChecked.Value)
            {
                MessageBox.Show("Вы несогласились с политикой безопасности");
                return;
            }

            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.");
                return;
            }

            using (var context = new Entities.AISCEntities1())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    var employee = context.Employees.FirstOrDefault(emp => emp.EmployeeID == user.EmployeeID);

                    if (employee == null)
                    {
                        MessageBox.Show("Запись не найдена.");
                        return;
                    }
                    if (!employee.IsActive)
                    {
                        MessageBox.Show("Отказанно в доступе");
                        return;
                    }
                    if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                    {
                        MessageBox.Show("Вход выполнен успешно!");

                       
                        int employeeID = (int)user.EmployeeID;

                        switch (user.RoleID)
                        {
                            case 1: // Админ 
                                AdminWindow adminWindow = new AdminWindow(employeeID);
                                adminWindow.Show();
                                break;
                            //case 2: // Тестовая роль
                            //    ManagerWindow managerWindow = new ManagerWindow(employeeID);
                            //    managerWindow.Show();
                               // break;
                            case 3: // Руководитель перенесен в админа
                                HRWindow hrWindow = new HRWindow(employeeID);
                                hrWindow.Show();
                                break;
                            case 4: // Сотрудник
                                EmployeeWindow employeeWindow = new EmployeeWindow(employeeID);
                                employeeWindow.Show();
                                break;
                            default:
                                MessageBox.Show("Неверная роль.");
                                break;
                        }

                        this.Close();
                    }
                    else
                    {
                        var result = MessageBox.Show("Неверный логин или пароль.\n\n" + "Забыли пароль? Нажмите \"Да\", чтобы перейти к странице восстановления пароля.", "Ошибка входа",
                           MessageBoxButton.YesNo,
                           MessageBoxImage.Information,
                           MessageBoxResult.Yes,
                           MessageBoxOptions.ServiceNotification);

                        if (result == MessageBoxResult.Yes)
                        {
                          
                            PasswordRestorationWindow passwordRestorationWindow = new PasswordRestorationWindow();
                            passwordRestorationWindow.Show();

                            
                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.");
                }
            }
        }

        public static string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            AdminWindow adminWindow = new AdminWindow(employeeID);//для обхода на страницу админа кнопка в xaml
            adminWindow.Show();

        }
    }
}
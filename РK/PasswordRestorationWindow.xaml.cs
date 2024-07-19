using System;
using System.Linq;
using System.Windows;

namespace AIS
{
    /// <summary>
    /// Логика взаимодействия для PasswordRestorationWindow.xaml
    /// </summary>
    public partial class PasswordRestorationWindow : Window
    {
        private Entities.AISCEntities1 context = new Entities.AISCEntities1();
        public string email;

        public PasswordRestorationWindow()
        {
            InitializeComponent();
        }

        private void btnSendResetLink_Click(object sender, RoutedEventArgs e)
        {
            string email = txtUsernameOrEmail.Text;
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Пожалуйста, введите корректный адрес электронной почты.");
                return;
            }

           
            var user = context.Employees.FirstOrDefault(u => u.ContactInfo == email);
            if (user == null)
            {
                MessageBox.Show("Пользователь с таким адресом электронной почты не найден.");
                return;
            }

           
            string resetToken = Guid.NewGuid().ToString();

            
           
            context.SaveChanges();

           
            EmailSending(user.ContactInfo, resetToken);

            MessageBox.Show("Письмо с инструкцией по сбросу пароля отправлено.");
            this.Close();

        }

        private void EmailSending(string toEmail, string resetToken)
        {
            

          
            string testEmailAddress = context.Employees.FirstOrDefault()?.ContactInfo;

            if (string.IsNullOrEmpty(testEmailAddress))
            {
                MessageBox.Show("Тест.");
                return;
            }

           
            string message = $@"
            *** Сообщение ***
            К: {testEmailAddress}
            Тема: Сброс пароля

            Здравствуйте,

            Чтобы сбросить пароль, перейдите по ссылке ниже: 
           
            http://your-app-domain.com/resetpassword?token={resetToken}
        ";
            MessageBox.Show(message, "Сообщение отправлено");
        }



        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

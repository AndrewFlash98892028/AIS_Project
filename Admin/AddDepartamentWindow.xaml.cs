using AIS.Entities; 
using System.Windows; 

namespace AIS.Admin // Пространство имен для административной части приложения AIS
{
    /// <summary>
    /// Логика взаимодействия для AddDepartamentWindow.xaml 
    /// </summary>
    public partial class AddDepartamentWindow : Window 
    {
        public AddDepartamentWindow() 
        {
            InitializeComponent(); 
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) 
        {
            if (ValidateInput()) 
            {
                using (var context = new AISCEntities1()) 
                {
                    var newDepartment = new Departments 
                    {
                        DepartmentName = txtDepartmentName.Text 
                    };

                    context.Departments.Add(newDepartment); 
                    context.SaveChanges(); 

                    DialogResult = true; 
                    this.Close(); 
                }
                
            }
        }

        private bool ValidateInput() 
        {
            if (string.IsNullOrEmpty(txtDepartmentName.Text)) 
            {
                MessageBox.Show("Введите название отдела"); 
                return false; 
            }
            return true; //
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) 
        {
            DialogResult = false; 
            this.Close(); 

        }
    }
}
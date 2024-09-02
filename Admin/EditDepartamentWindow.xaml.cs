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
    /// Логика взаимодействия для EditDepartamentWindow.xaml
    /// </summary>
    public partial class EditDepartamentWindow : Window
    {
        private Departments departmentToEdit;
        private Departments department;

        public EditDepartamentWindow(Entities.Departments selectedDepartment)
        {
            InitializeComponent();
            departmentToEdit = selectedDepartment;

            txtDepartmentName.Text = departmentToEdit.DepartmentName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                using (var context = new AISCEntities1())
                {
                    var department = context.Departments.Find(departmentToEdit.DepartmentID);

                    if (department != null)
                    {
                        department.DepartmentName = txtDepartmentName.Text;
                        context.SaveChanges();

                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Отдел не найден.");
                    }
                }
            }

        }
        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtDepartmentName.Text))
            {
                MessageBox.Show("Пожалуйста введите название отдела.");
                return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();

        }
    }
}

using System;
using System.Linq;
using System.Windows;
using AIS.Entities;

namespace AIS.Admin
{
    public partial class AddEmployeeWindow : Window
    {
        public AddEmployeeWindow()
        {
            InitializeComponent();
            LoadDepartments();
            LoadJobTitles();
            dpHireDate.SelectedDate = DateTime.Now; 
        }

        private void LoadDepartments()
        {
            using (var context = new AISCEntities1())
            {
                cmbDepartment.ItemsSource = context.Departments.ToList();
                cmbDepartment.DisplayMemberPath = "DepartmentName";
                cmbDepartment.SelectedValuePath = "DepartmentID";
            }
        }

        private void LoadJobTitles()
        {
            using (var context = new AISCEntities1())
            {
                cmbJobTitle.ItemsSource = context.JobTitles.ToList();
                cmbJobTitle.DisplayMemberPath = "JobTitle";
                cmbJobTitle.SelectedValuePath = "JobTitleID";
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                using (var context = new AISCEntities1())
                {
                   
                    

                    var newEmployee = new Employees
                    {
                         
                        FirstName = txtFirstName.Text,
                        LastName = txtLastName.Text,
                        DepartmentID = (int)cmbDepartment.SelectedValue,
                        JobTitleID = (int)cmbJobTitle.SelectedValue,
                        HireDate = dpHireDate.SelectedDate.Value,
                        IsActive = true
                    };

                    context.Employees.Add(newEmployee);
                    context.SaveChanges();

                    DialogResult = true;
                    this.Close();
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                MessageBox.Show("Пожалуйста введите имя и фамилию.");
                return false;
            }

            if (string.IsNullOrEmpty(txtLastName.Text))
            {
                MessageBox.Show("Пожалуйста введите отчество.");
                return false;
            }

            if (cmbDepartment.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста введите отдел.");
                return false;
            }

            if (cmbJobTitle.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста введите должность.");
                return false;
            }

            if (dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста введите дату назначения.");
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

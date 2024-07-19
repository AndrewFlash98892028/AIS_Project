using System;
using System.Linq;
using System.Windows;
using AIS.Entities;

namespace AIS.Admin
{
    public partial class EditEmployeeWindow : Window
    {
        private Employees employeeToEdit;

        public EditEmployeeWindow(Employees employee)
        {
            InitializeComponent();
            employeeToEdit = employee;
            LoadDepartments();
            LoadJobTitles();

           
            txtFirstName.Text = employeeToEdit.FirstName;
            txtLastName.Text = employeeToEdit.LastName;
            cmbDepartment.SelectedValue = employeeToEdit.DepartmentID;
            cmbJobTitle.SelectedValue = employeeToEdit.JobTitleID;
            dpHireDate.SelectedDate = employeeToEdit.HireDate;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                using (var context = new AISCEntities1())
                {
                    var employee = context.Employees.Find(employeeToEdit.EmployeeID);

                    if (employee != null)
                    {
                        
                        employee.FirstName = txtFirstName.Text;
                        employee.LastName = txtLastName.Text;
                        employee.DepartmentID = (int)cmbDepartment.SelectedValue;
                        employee.JobTitleID = (int)cmbJobTitle.SelectedValue;
                        employee.HireDate = dpHireDate.SelectedDate.Value;
                        

                        context.SaveChanges();
                        DialogResult = true; 
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник не найден.");
                    }
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
                MessageBox.Show("Пожалуйста выберете отдел.");
                return false;
            }

            if (cmbJobTitle.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете должность.");
                return false;
            }

            if (dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста выберете дату найма.");
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
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using AIS.Entities; 

namespace AIS.Admin
{
    public partial class EmployeePage : Page
    {
        private ObservableCollection<Employees> employees;

        public EmployeePage()
        {
            InitializeComponent();
            LoadEmployees();
            LoadDepartments();
            LoadJobTitles();
        }

        private void LoadEmployees()
        {
            using (var context = new AISCEntities1())
            {
                employees = new ObservableCollection<Employees>(context.Employees.ToList());
                dgEmployees.ItemsSource = employees;
            }
        }

        private void LoadDepartments()
        {
            using (var context = new AISCEntities1())
            {
                dgEmployees.Columns.OfType<DataGridComboBoxColumn>()
                    .First(c => c.Header.ToString() == "Отдел")
                    .ItemsSource = context.Departments.ToList();
            }
        }

        private void LoadJobTitles()
        {
            using (var context = new AISCEntities1())
            {
                dgEmployees.Columns.OfType<DataGridComboBoxColumn>()
                    .First(c => c.Header.ToString() == "Должность")
                    .ItemsSource = context.JobTitles.ToList();
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
           
            AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
            if (addEmployeeWindow.ShowDialog() == true)
            {
               
                LoadEmployees();
            }
        }

        private void btnEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employees selectedEmployee)
            {
               
                EditEmployeeWindow editEmployeeWindow = new EditEmployeeWindow(selectedEmployee);
                if (editEmployeeWindow.ShowDialog() == true)
                {
                    
                    LoadEmployees();
                }
            }
        }

        private void btnDeactivateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employees selectedEmployee)
            {
                if (MessageBox.Show($"Отключить сотрудника {selectedEmployee.FirstName} {selectedEmployee.LastName}?",
                                    "Подтверждаю", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var context = new AISCEntities1())
                    {
                        var employee = context.Employees.Find(selectedEmployee.EmployeeID);
                        if (employee != null)
                        {
                            employee.IsActive = false;
                            context.SaveChanges();
                            LoadEmployees(); 
                        }
                    }
                }
            }
        }

        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            btnEditEmployee.IsEnabled = dgEmployees.SelectedItem != null;
            btnDeactivateEmployee.IsEnabled = dgEmployees.SelectedItem != null;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            string searchText = txtSearch.Text.ToLower();

            using (var context = new AISCEntities1())
            {
                employees = new ObservableCollection<Employees>(context.Employees
                                .Where(emp => emp.FirstName.ToLower().Contains(searchText) ||
                                             emp.LastName.ToLower().Contains(searchText))
                                .ToList());
                dgEmployees.ItemsSource = employees;
            }
        }

        private void btnActivateEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employees selectedEmployee)
            {
                if (MessageBox.Show($"Активировать сотрудника {selectedEmployee.FirstName} {selectedEmployee.LastName}?",
                                     "Подтверждаю", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (var context = new AISCEntities1())
                    {
                        var employee = context.Employees.Find(selectedEmployee.EmployeeID);
                        if (employee != null)
                        {
                            employee.IsActive = true; 
                            context.SaveChanges();
                            LoadEmployees(); 
                        }
                    }
                }
            }
        }
    }
}
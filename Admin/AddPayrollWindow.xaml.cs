using AIS.Entities;
using System;
using System.Linq;
using System.Windows;

namespace AIS.Admin
{
    public partial class AddPayrollWindow : Window
    {
        public Payroll NewPayrollRecord { get; private set; }

        public AddPayrollWindow()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                using (var db = new AISCEntities1())
                {
                    var employees = db.Employees.Select(e => new
                    {
                        EmployeeID = e.EmployeeID,
                        FullName = e.FirstName + " " + e.LastName
                    }).ToList();

                    cmbEmployee.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сотрудников: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (cmbEmployee.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете сотрудника.");
                return;
            }

            if (dpPayPeriodStart.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста выберете начальную дату.");
                return;
            }

            if (dpPayPeriodEnd.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйства выберете конечную дату.");
                return;
            }
           

           
            NewPayrollRecord = new Payroll
            {
                EmployeeID = (int)cmbEmployee.SelectedValue,
                PayPeriodStart = dpPayPeriodStart.SelectedDate.Value,
                PayPeriodEnd = dpPayPeriodEnd.SelectedDate.Value,
                GrossPay = 0,
                NetPay = 0,
                Deductions = 0,
                Taxes = 0
            };

            DialogResult = true; 
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
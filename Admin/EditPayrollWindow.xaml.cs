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
    /// Логика взаимодействия для EditPayrollWindow.xaml
    /// </summary>
    public partial class EditPayrollWindow : Window
    {
        private Payroll _payrollRecordToEdit;
       

        public EditPayrollWindow(Payroll selectedPayroll)
        {
            InitializeComponent();
            _payrollRecordToEdit = selectedPayroll;
            LoadEmployees();

            
            cmbEmployee.SelectedValue = _payrollRecordToEdit.EmployeeID;
            dpPayPeriodStart.SelectedDate = _payrollRecordToEdit.PayPeriodStart;
            dpPayPeriodEnd.SelectedDate = _payrollRecordToEdit.PayPeriodEnd;
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
            _payrollRecordToEdit.PayPeriodStart = dpPayPeriodStart.SelectedDate.Value;
            _payrollRecordToEdit.PayPeriodEnd = dpPayPeriodEnd.SelectedDate.Value;

            
            try
            {
                using (var db = new AISCEntities1())
                {
                    db.Entry(_payrollRecordToEdit).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    DialogResult = true;
                    MessageBox.Show("Запись изменена!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка изменения записи: {ex.Message}");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

        }
    }
}

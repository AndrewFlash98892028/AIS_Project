using AIS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    public partial class PayrollPage : Page
    {
        public PayrollPage()
        {
            InitializeComponent();
            LoadPayrollData();
        }

        private void LoadPayrollData()
        {
            try
            {
                using (var db = new AISCEntities1())
                {
                   
                    var payrollData = db.Payroll
                        .Include("Employees") 
                        .ToList();

                    dgPayroll.ItemsSource = payrollData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки зарплаты: {ex.Message}");
            }
        }

        private void btnAddPayroll_Click(object sender, RoutedEventArgs e)
        {
          
            AddPayrollWindow addWindow = new AddPayrollWindow();
            if (addWindow.ShowDialog() == true)
              
          
            {
                try
                {
                   
                    using (var db = new AISCEntities1())
                    {
                        db.Payroll.Add(addWindow.NewPayrollRecord);
                        db.SaveChanges();
                        LoadPayrollData();
                        MessageBox.Show("Запись о зарплате сохранена успешно!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка создания новой записи о зарплате: {ex.Message}");
                }
            }
        }

        private void btnEditPayroll_Click(object sender, RoutedEventArgs e)
        {
            Payroll selectedPayroll = dgPayroll.SelectedItem as Payroll;
            if (selectedPayroll == null)
            {
                MessageBox.Show("Пожалуйста выберете какую запись нужно отредактировать.");
                return;
            }

            EditPayrollWindow editWindow = new EditPayrollWindow(selectedPayroll);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    using (var db = new AISCEntities1())
                    {
                        var payrollToUpdate = db.Payroll.Find(selectedPayroll.PayrollID);
                        if (payrollToUpdate != null)
                        {
                            
                            db.SaveChanges();
                            LoadPayrollData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновлеия записи: {ex.Message}");
                }
            }
        }

        private void btnGeneratePayroll_Click(object sender, RoutedEventArgs e)
        {
            if (dpPayPeriodStart.SelectedDate == null || dpPayPeriodEnd.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйтса выберете оба начало расчетного пееода и конец расчетного переода.");
                return;
            }

            DateTime payPeriodStart = dpPayPeriodStart.SelectedDate.Value;
            DateTime payPeriodEnd = dpPayPeriodEnd.SelectedDate.Value;

            try
            {
                using (var db = new AISCEntities1())
                {
                    var payrollService = new PayrollService(db);
                    var payrollRecords = payrollService.GeneratePayroll(payPeriodStart, payPeriodEnd);

                    
                    dgPayroll.ItemsSource = payrollRecords;

                    MessageBox.Show("Запись сохранена успешно!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания записи: {ex.Message}");
            }

        }
    }
}
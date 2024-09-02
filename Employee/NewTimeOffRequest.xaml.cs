using AIS.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Employee
{
    public partial class NewTimeOffRequestWindow : Window
    {
        public TimeOffRequests NewTimeOffRequest { get; private set; }
        private int _employeeId;

        public NewTimeOffRequestWindow(int employeeID)
        {
            InitializeComponent();
            _employeeId = employeeID;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            
            if (dpStartDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста выберете дату начала.");
                return;
            }

            if (dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста выберете дату конца.");
                return;
            }

            if (dpEndDate.SelectedDate < dpStartDate.SelectedDate)
            {
                MessageBox.Show("Конечная дата должна быть позже начальной.");
                return;
            }

            if (string.IsNullOrEmpty(txtReason.Text))
            {
                MessageBox.Show("Пожалуйста напишите причину запроса.");
                return;
            }

           
            NewTimeOffRequest = new TimeOffRequests
            {
                EmployeeID = _employeeId,
                StartDate = dpStartDate.SelectedDate.Value,
                EndDate = dpEndDate.SelectedDate.Value,
                Reason = txtReason.Text,
                Status = "Ожидание", 
                ApprovedBy = null 
               
            };

           
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
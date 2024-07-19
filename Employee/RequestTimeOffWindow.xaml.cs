using AIS.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Employee
{
    public partial class RequestTimeOffWindow : Window
    {
        public TimeOffRequests NewTimeOffRequest { get; private set; }
        private int _employeeId;

        public RequestTimeOffWindow(int employeeID)
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
                MessageBox.Show("Дата конца не может быть позже даты начала.");
                return;
            }

            if (string.IsNullOrEmpty(txtReason.Text))
            {
                MessageBox.Show("Пожалуйста укажите причину.");
                return;
            }

         
            NewTimeOffRequest = new TimeOffRequests
            {
                EmployeeID = _employeeId,
                StartDate = dpStartDate.SelectedDate.Value,
                EndDate = dpEndDate.SelectedDate.Value,
                Reason = txtReason.Text,
                Status = "Ожидание",
            };

          
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
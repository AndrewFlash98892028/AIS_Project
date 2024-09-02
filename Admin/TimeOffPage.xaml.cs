using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    public partial class TimeOffPage : Page
    {
        public TimeOffPage()
        {
            InitializeComponent();
            LoadTimeOffRequests();
        }

        private void LoadTimeOffRequests()
        {
            try
            {
                using (var db = new AISCEntities1())
                {
                  
                    var timeOffRequests = db.TimeOffRequests
                        .Include("Employees")
                        .Include("Users") 
                        .ToList();

                    dgTimeOffRequests.ItemsSource = timeOffRequests;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отпусков: {ex.Message}");
            }
        }

        private void btnApproveRequest_Click(object sender, RoutedEventArgs e)
        {
            TimeOffRequests selectedRequest = dgTimeOffRequests.SelectedItem as TimeOffRequests;
            if (selectedRequest == null)
            {
                MessageBox.Show("Пожалуйста выберете отпуск.");
                return;
            }

            try
            {
                using (var db = new AISCEntities1())
                {
                    var requestToUpdate = db.TimeOffRequests.Find(selectedRequest.RequestID);
                    if (requestToUpdate != null)
                    {
                        requestToUpdate.Status = "Одобрен";
                     
                        db.SaveChanges();
                        LoadTimeOffRequests(); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void btnRejectRequest_Click(object sender, RoutedEventArgs e)
        {
            TimeOffRequests selectedRequest = dgTimeOffRequests.SelectedItem as TimeOffRequests;
            if (selectedRequest == null)
            {
                MessageBox.Show("Пожалуйста выберете отпуск.");
                return;
            }

            try
            {
                using (var db = new AISCEntities1())
                {
                    var requestToUpdate = db.TimeOffRequests.Find(selectedRequest.RequestID);
                    if (requestToUpdate != null)
                    {
                        requestToUpdate.Status = "Отказанно";
                       
                        db.SaveChanges();
                        LoadTimeOffRequests();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
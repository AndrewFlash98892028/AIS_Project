using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    public partial class SchedulesPage : Page
    {
        public SchedulesPage()
        {
            InitializeComponent();
            LoadSchedules();
        }

        private void LoadSchedules()
        {
            try
            {
                using (var db = new AISCEntities1())
                {
                   
                    var schedules = db.Schedules
                        .Include("Employees") 
                        .ToList(); 

                    dgSchedules.ItemsSource = schedules;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки графиков: {ex.Message}");
            }
        }

        private void btnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
           
            var selectedItem = dgSchedules.SelectedItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете сотрудника.");
                return;
            }

          
            int employeeID = (int)selectedItem.GetType().GetProperty("EmployeeID").GetValue(selectedItem);

           
            AddScheduleWindow addWindow = new AddScheduleWindow(employeeID);
            if (addWindow.ShowDialog() == true)
            {
                try
                {
                    using (var db = new AISCEntities1())
                    {
                        db.Schedules.Add(addWindow.NewSchedules);
                        db.SaveChanges();
                        LoadSchedules(); 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}");
                }
            }
        }

        private void btnEditSchedule_Click(object sender, RoutedEventArgs e)
        {
            
            var selectedItem = dgSchedules.SelectedItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете график для редактирования.");
                return;
            }

            
            int scheduleID = (int)selectedItem.GetType().GetProperty("ScheduleID").GetValue(selectedItem);

            Schedules scheduleToEdit;
            using (var db = new AISCEntities1())
            {
                scheduleToEdit = db.Schedules.Find(scheduleID);
            }

            if (scheduleToEdit != null)
            {
                EditScheduleWindow editWindow = new EditScheduleWindow(scheduleToEdit);
                if (editWindow.ShowDialog() == true)
                {
                    try
                    {
                        using (var db = new AISCEntities1())
                        {
                           
                            db.SaveChanges();
                            LoadSchedules(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка обновления: {ex.Message}");
                    }
                }
            }
        }
    }
}
using AIS.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    public partial class TimeEntriesPage : Page
    {
        private AISCEntities1 dbContext = new AISCEntities1();

        public TimeEntriesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var timeEntries = dbContext.TimeEntries
        .Include("Employee") 
        .Select(t => new TimeEntryGridModel
        {
            TimeEntryID = t.TimeEntryID,
            EmployeeID = t.EmployeeID,
            EmployeeName = t.Employees.FirstName + " " + t.Employees.LastName,
            Date = (DateTime)DbFunctions.TruncateTime(t.ClockInTime),
            ClockInTime = t.ClockInTime,
            ClockOutTime = t.ClockOutTime
        })
        .ToList();

            foreach (var entry in timeEntries)
            {
                entry.HoursWorked = entry.ClockOutTime.HasValue
                    ? (entry.ClockOutTime.Value - entry.ClockInTime).ToString(@"hh\:mm")
                    : "";
            }

            timeEntriesDataGrid.ItemsSource = timeEntries;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            
            AddTimeEntryWindow addWindow = new AddTimeEntryWindow();
            addWindow.ShowDialog();

            
            LoadData();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            TimeEntryGridModel selectedTimeEntry = timeEntriesDataGrid.SelectedItem as TimeEntryGridModel;
            if (selectedTimeEntry != null)
            {
                
                EditTimeEntryWindow editWindow = new EditTimeEntryWindow(selectedTimeEntry.TimeEntryID);
                editWindow.ShowDialog();

                
                LoadData();
            }
            else
            {
                MessageBox.Show("Пожалуйста выберете запись для редактирования.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            TimeEntryGridModel selectedTimeEntry = timeEntriesDataGrid.SelectedItem as TimeEntryGridModel;
            if (selectedTimeEntry != null)
            {
                if (MessageBox.Show("Вы уверены что хотите удалить запись?",
                                    "Подтверждаю", MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var timeEntryToDelete = dbContext.TimeEntries.Find(selectedTimeEntry.TimeEntryID);
                        if (timeEntryToDelete != null)
                        {
                            dbContext.TimeEntries.Remove(timeEntryToDelete);
                            dbContext.SaveChanges();
                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления записи: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста выберете запись для удаления.");
            }
        }
    }

    public class TimeEntryGridModel
    {
        public int TimeEntryID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public DateTime ClockInTime { get; set; }
        public DateTime? ClockOutTime { get; set; }
        public string HoursWorked { get; set; }
    }
}
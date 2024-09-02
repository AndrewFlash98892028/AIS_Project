using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    public partial class EditScheduleWindow : Window
    {
        public Schedules Schedule { get; private set; } 

        public EditScheduleWindow(Schedules scheduleToEdit)
        {
            InitializeComponent();
            Schedule = scheduleToEdit; 
            LoadEmployees();

            
            cmbEmployee.SelectedValue = Schedule.EmployeeID;

            DateTime shiftStartDateTime = DateTime.Today + Schedule.ShiftStart;
            DateTime shiftEndDateTime = DateTime.Today + Schedule.ShiftEnd;

            tpShiftStart.Value = shiftStartDateTime;
            tpShiftEnd.Value = shiftEndDateTime;

            cmbDayOfWeek.SelectedIndex = Schedule.DayOfWeek - 1;
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
                MessageBox.Show($"Ошибка загрузки загрузки сотрудников: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           
            if (tpShiftStart.Value == null)
            {
                MessageBox.Show("Пожалуйста введите начало смены.");
                return;
            }

            if (tpShiftEnd.Value == null)
            {
                MessageBox.Show("Пожалуйста введите конец смены.");
                return;
            }

            if (cmbDayOfWeek.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете день недели.");
                return;
            }

            if (tpShiftEnd.Value <= tpShiftStart.Value)
            {
                MessageBox.Show("Конец смены должен быть после начала смены.");
                return;
            }
            Schedule.ShiftStart = tpShiftStart.Value.Value.TimeOfDay;
            Schedule.ShiftEnd = tpShiftEnd.Value.Value.TimeOfDay;
            Schedule.DayOfWeek = cmbDayOfWeek.SelectedIndex + 1;
            Schedule.ScheduleType = (cmbScheduleType.SelectedItem as ComboBoxItem)?.Content.ToString();

          
            try
            {
                using (var db = new AISCEntities1())
                {
                    db.Entry(Schedule).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                DialogResult = true;
                MessageBox.Show("График обновлен успешно!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления графика: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
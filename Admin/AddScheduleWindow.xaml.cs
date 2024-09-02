using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Admin
{
    public partial class AddScheduleWindow : Window
    {
        public Schedules NewSchedules { get; private set; }
        private int _employeeId;

        public AddScheduleWindow(int employeeID)
        {
            InitializeComponent();
            _employeeId = employeeID;
            LoadEmployees();
            cmbEmployee.SelectedValue = _employeeId;
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
                MessageBox.Show("Выберете сотрудника.");
                return;
            }

            if (tpShiftStart.Value == null)
            {
                MessageBox.Show("Пожалуйста выберете начало смены.");
                return;
            }

            if (tpShiftEnd.Value == null)
            {
                MessageBox.Show("Пожалуйста выберете конец смены.");
                return;
            }

            if (cmbDayOfWeek.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете день недели.");
                return;
            }

            if (tpShiftEnd.Value <= tpShiftStart.Value)
            {
                MessageBox.Show("Время окончания смены должно быть позже времени начала смены.");
                return;
            }

            NewSchedules = new Schedules();
            NewSchedules.EmployeeID = ((dynamic)cmbEmployee.SelectedValue).EmployeeID; 
            NewSchedules.ShiftStart = tpShiftStart.Value.Value.TimeOfDay;
            NewSchedules.ShiftEnd = tpShiftEnd.Value.Value.TimeOfDay;
            NewSchedules.DayOfWeek = cmbDayOfWeek.SelectedIndex + 1;
            NewSchedules.ScheduleType = (cmbScheduleType.SelectedItem as ComboBoxItem)?.Content.ToString();

          
            try
            {
                using (var db = new AISCEntities1())
                {
                    db.Schedules.Add(NewSchedules);
                    db.SaveChanges();
                }
                DialogResult = true; 
                MessageBox.Show("График добавлен успешно!");
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления графика: {ex.Message}");
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
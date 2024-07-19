using AIS.Entities;
using System;
using System.Linq;
using System.Windows;

namespace AIS.Admin
{
    public partial class AddTimeEntryWindow : Window
    {
        public TimeEntries NewTimeEntry { get; private set; }

        public AddTimeEntryWindow()
        {
            InitializeComponent();
            NewTimeEntry = new TimeEntries();
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            using (var db = new AISCEntities1())
            {
                var employees = db.Employees.ToList();
                cmbEmployee.ItemsSource = employees
                    .Select(e => new { e.EmployeeID, FullName = $"{e.FirstName} {e.LastName}" })
                    .ToList();
                cmbEmployee.DisplayMemberPath = "FullName";
                cmbEmployee.SelectedValuePath = "EmployeeID";

                cmbProject.ItemsSource = db.Projects.ToList();
                cmbProject.DisplayMemberPath = "ProjectName";
                cmbProject.SelectedValuePath = "ProjectID";

                cmbTask.ItemsSource = db.Tasks.ToList();
                cmbTask.DisplayMemberPath = "TaskName";
                cmbTask.SelectedValuePath = "TaskID";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEmployee.SelectedItem == null ||
                dpDate.SelectedDate == null ||
                cmbProject.SelectedItem == null ||
                cmbTask.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете сотрудника, дату, проект, и задачу.");
                return;
            }

            NewTimeEntry.EmployeeID = ((dynamic)cmbEmployee.SelectedItem).EmployeeID;

            DateTime selectedDate = dpDate.SelectedDate.Value;
            DateTime clockInTime = selectedDate.Date;
            if (tpClockIn.Value.HasValue)
            {
                clockInTime = clockInTime + tpClockIn.Value.Value.TimeOfDay;
            }

            DateTime? clockOutTime = null;
            if (tpClockOut.Value.HasValue)
            {
                clockOutTime = selectedDate.Date + tpClockOut.Value.Value.TimeOfDay;
            }

            NewTimeEntry.ClockInTime = clockInTime;
            NewTimeEntry.ClockOutTime = clockOutTime;

            if (clockOutTime.HasValue && clockOutTime < clockInTime)
            {
                MessageBox.Show("Время конца работы должно быть позже времени начала работы.");
                return;
            }

            NewTimeEntry.ProjectID = ((dynamic)cmbProject.SelectedItem).ProjectID;
            NewTimeEntry.TaskID = ((dynamic)cmbTask.SelectedItem).TaskID;

            try
            {
                using (var db = new AISCEntities1())
                {
                    db.TimeEntries.Add(NewTimeEntry);
                    db.SaveChanges();
                }

                DialogResult = true;
                MessageBox.Show("Запись добавлена успешно!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения записи: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
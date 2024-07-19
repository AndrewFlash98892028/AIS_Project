using AIS.Entities;
using System;
using System.Linq;
using System.Windows;

namespace AIS.Admin
{
    public partial class EditTimeEntryWindow : Window
    {
        private AISCEntities1 dbContext = new AISCEntities1();
        private TimeEntries existingTimeEntry;

        public EditTimeEntryWindow(int timeEntryId)
        {
            InitializeComponent();
            LoadComboBoxData();

            existingTimeEntry = dbContext.TimeEntries.Find(timeEntryId);

            if (existingTimeEntry == null)
            {
                MessageBox.Show("Запись не найдена!");
                this.Close();
                return;
            }

            PopulateControls();
        }

        private void LoadComboBoxData()
        {
           
            var employees = dbContext.Employees.ToList();
            cmbEmployee.ItemsSource = employees
                .Select(e => new { e.EmployeeID, FullName = $"{e.FirstName} {e.LastName}" })
                .ToList();
            cmbEmployee.DisplayMemberPath = "FullName";
            cmbEmployee.SelectedValuePath = "EmployeeID";

            cmbProject.ItemsSource = dbContext.Projects.ToList();
            cmbProject.DisplayMemberPath = "ProjectName"; 
            cmbProject.SelectedValuePath = "ProjectID";

           
            cmbTask.ItemsSource = dbContext.Tasks.ToList();
            cmbTask.DisplayMemberPath = "TaskName";   
            cmbTask.SelectedValuePath = "TaskID";
        }

        private void PopulateControls()
        {
           
            cmbEmployee.SelectedValue = existingTimeEntry.EmployeeID;
            cmbProject.SelectedValue = existingTimeEntry.ProjectID;
            cmbTask.SelectedValue = existingTimeEntry.TaskID;

            dpDate.SelectedDate = existingTimeEntry.ClockInTime.Date;
            tpClockIn.Value = existingTimeEntry.ClockInTime;
            tpClockOut.Value = existingTimeEntry.ClockOutTime;
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

            existingTimeEntry.EmployeeID = ((dynamic)cmbEmployee.SelectedItem).EmployeeID;

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

            existingTimeEntry.ClockInTime = clockInTime;
            existingTimeEntry.ClockOutTime = clockOutTime;

            if (clockOutTime.HasValue && clockOutTime < clockInTime)
            {
                MessageBox.Show("Конец работы должен быть после начала работы.");
                return;
            }

            existingTimeEntry.ProjectID = ((Projects)cmbProject.SelectedItem).ProjectID;
            existingTimeEntry.TaskID = ((Tasks)cmbTask.SelectedItem).TaskID;

            try
            {
               
                dbContext.SaveChanges();

                DialogResult = true;
                MessageBox.Show("Запись обновлена успешно!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления записи: {ex.Message}");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
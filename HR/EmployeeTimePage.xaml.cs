using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.HR
{
    public partial class EmployeeTimePage : Page
    {
        private AISCEntities1 context;

        public EmployeeTimePage(AISCEntities1 context)
        {
            InitializeComponent();
            this.context = context;
            LoadDepartments();
            LoadTimeEntries();
        }

        private void LoadDepartments()
        {
            var departments = context.Departments.ToList();
            cmbDepartmentFilter.ItemsSource = departments;
        }

        private void LoadTimeEntries(int? departmentID = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var timeEntries = context.TimeEntries.AsQueryable();

            if (departmentID != null)
            {
                timeEntries = timeEntries.Where(te => te.Employees.DepartmentID == departmentID);
            }

            if (startDate != null && endDate != null)
            {
                timeEntries = timeEntries.Where(te => te.ClockInTime.Date >= startDate.Value.Date &&
                                                    te.ClockInTime.Date <= endDate.Value.Date);
            }

            dgTimeEntries.ItemsSource = timeEntries.ToList();
        }

        private void cmbDepartmentFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int? departmentID = cmbDepartmentFilter.SelectedValue as int?;
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            LoadTimeEntries(departmentID, startDate, endDate);
        }

        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            int? departmentID = cmbDepartmentFilter.SelectedValue as int?;
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;

            LoadTimeEntries(departmentID, startDate, endDate);
        }


       
    }
}
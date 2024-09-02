using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.HR
{
    public partial class EmployeeManagementPage : Page
    {
        private AISCEntities1 _context;

        public EmployeeManagementPage(AISCEntities1 context)
        {
            InitializeComponent();
            _context = context;
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                
                var employees = _context.Employees
                    .Select(e => new
                    {
                        e.EmployeeID,
                        e.FirstName,
                        e.LastName,
                        Department = e.Departments.DepartmentName,
                        JobTitle = e.JobTitles.JobTitle,
                        e.HireDate,
                        e.IsActive,
                        e.ContactInfo,
                        e.TerminationDate,
                        e.HourlyRate
                    })
                    .ToList();

                dgEmployees.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошиб: {ex.Message}");
            }
        }
    }
}
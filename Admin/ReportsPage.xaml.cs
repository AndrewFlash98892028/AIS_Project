using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using AIS.Entities;
using System.Data.Entity;

namespace AIS.Admin
{
    public partial class ReportsPage : Page
    {
        private AISCEntities1 context;

        public string ChartTitle { get; set; }
       
        public ReportsPage(AISCEntities1 context)
        {
            InitializeComponent();
            this.context = context;
            dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEndDate.SelectedDate = DateTime.Now;
            cmbReportType.SelectedIndex = 0;
        }





        public Func<double, string> YFormatter { get; set; } = value => value.ToString("N2");

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {

            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show(".");
                return;
            }

            if (cmbReportType.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста выберете тип отчета .");
                return;
            }

            DateTime startDate = dpStartDate.SelectedDate.Value;
            DateTime endDate = dpEndDate.SelectedDate.Value;

           
            string selectedReportType = (cmbReportType.SelectedItem as ComboBoxItem)?.Content.ToString();

            switch (selectedReportType)
            {
                case "Часы сотрудников":
                    GenerateEmployeeHoursReport(context, startDate, endDate);
                    break;
                case "Часы отдела":
                    GenerateDepartmentHoursReport(context, startDate, endDate);
                    break;
                default:
                    MessageBox.Show("Некорректный тип отчета.");
                    break;
            }

        }

        private void GenerateEmployeeHoursReport(AISCEntities1 context, DateTime startDate, DateTime endDate)
        {

            var employeeHours = context.TimeEntries
                .Where(te => te.ClockInTime >= startDate && te.ClockInTime <= endDate && te.ClockOutTime != null)
                .GroupBy(te => new { te.Employees.FirstName, te.Employees.LastName })
                .Select(g => new
                {
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    TotalHours = g.Sum(te => DbFunctions.DiffHours(te.ClockInTime, te.ClockOutTime) ?? 0.0)
                })
                .ToList();

           
            chartReport.Series = new SeriesCollection();

           
            foreach (var empHour in employeeHours)
            {
                chartReport.Series.Add(new ColumnSeries
                {
                    Title = $"{empHour.FirstName} {empHour.LastName}",  
                    Values = new ChartValues<double> { empHour.TotalHours }  
                });
            }

           
            chartReport.AxisX.Clear();
            chartReport.AxisX.Add(new Axis
            {
                Title = "Сотрудники", 
                Labels = employeeHours.Select(eh => $"{eh.FirstName} {eh.LastName}").ToList() 
            });

           
            txtChartTitle.Text = "Отчет по сотрудникам";

        }

        private void GenerateDepartmentHoursReport(AISCEntities1 context, DateTime startDate, DateTime endDate)
        {
            var departmentHours = context.TimeEntries
                .Where(te => te.ClockInTime >= startDate && te.ClockInTime <= endDate && te.ClockOutTime != null)
                .GroupBy(te => te.Employees.Departments.DepartmentName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalHours = g.Sum(te => DbFunctions.DiffHours(te.ClockInTime, te.ClockOutTime) ?? 0.0)
                })
                .ToList();

            chartReport.Series = new SeriesCollection();
            foreach (var deptHour in departmentHours)
            {
                chartReport.Series.Add(new ColumnSeries
                {
                    Title = deptHour.Name,
                    Values = new ChartValues<double> { deptHour.TotalHours } 
                });
            }

            chartReport.AxisX.Clear();
            chartReport.AxisX.Add(new Axis
            {
                Title = "Отделы",
                Labels = departmentHours.Select(dh => dh.Name).ToList()
            });

            txtChartTitle.Text = "Отчёт по отделам";
        }

        private void cmbReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chartReport.Series.Clear();
        }
    }
}
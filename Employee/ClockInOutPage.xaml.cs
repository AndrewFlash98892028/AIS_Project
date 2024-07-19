using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Employee
{
    public partial class ClockInOutPage : Page
    {
        private AISCEntities1 context;
        private DateTime clockInTime;

        public ClockInOutPage(AISCEntities1 context, DateTime clockInTime, int currentEmployeeID)
        {
            InitializeComponent();
            this.context = context;
            this.clockInTime = clockInTime;
            LoadProjects();
        }

        private void LoadProjects()
        {
            var projects = context.Projects.ToList();
            cmbProjects.ItemsSource = projects;
        }

       
    }
}
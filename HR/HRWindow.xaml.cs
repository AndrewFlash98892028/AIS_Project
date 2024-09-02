using AIS.Admin;
using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.HR
{
    public partial class HRWindow : Window
    {
        private AISCEntities1 context;

        public HRWindow(int employeeID)
        {
            InitializeComponent();
            context = new AISCEntities1(); 
        }

        private void btnEmployeeTime_Click(object sender, RoutedEventArgs e)
        {
            frmHRContent.Navigate(new EmployeeTimePage(context));
        }

        private void btnTimeOffRequests_Click(object sender, RoutedEventArgs e)
        {
            frmHRContent.Navigate(new TimeOffRequestsPage(context));
        }

        private void btnEmployeeManagement_Click(object sender, RoutedEventArgs e)
        {
           
            frmHRContent.Navigate(new EmployeeManagementPage(context));
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            frmHRContent.Navigate(new ReportsPage(context));
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
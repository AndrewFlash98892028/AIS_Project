using AIS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AIS.Employee
{
    public partial class ViewPayrollPage : Page
    {
        public ViewPayrollPage(List<Payroll> payrollData)
        {
            InitializeComponent();
            dgPayroll.ItemsSource = payrollData;
        }
    }
}
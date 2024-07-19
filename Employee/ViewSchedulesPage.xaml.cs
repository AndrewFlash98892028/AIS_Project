using AIS.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AIS.Employee
{
    public partial class ViewSchedulesPage : Page
    {
        public ViewSchedulesPage(List<Schedules> schedules)
        {
            InitializeComponent();
            dgSchedules.ItemsSource = schedules;
        }
    }
}
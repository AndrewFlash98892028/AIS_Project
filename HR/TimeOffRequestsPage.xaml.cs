using AIS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIS.HR
{
    /// <summary>
    /// Логика взаимодействия для TimeOffRequestsPage.xaml
    /// </summary>
    public partial class TimeOffRequestsPage : Page
    {
        private AISCEntities1 context;

        public TimeOffRequestsPage(AISCEntities1 context)
        {
            InitializeComponent();
            this.context = context;
            LoadTimeOffRequests();
        }
        private void LoadTimeOffRequests()
        {
            var timeOffRequests = context.TimeOffRequests
                                        .ToList(); 

            dgTimeOffRequests.ItemsSource = timeOffRequests;
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeOffRequests.SelectedItem is TimeOffRequests request)
            {
                request.Status = "Одобрено"; 

              
                context.SaveChanges();

                LoadTimeOffRequests(); 
            }
        }

        private void btnDeny_Click(object sender, RoutedEventArgs e)
        {
            if (dgTimeOffRequests.SelectedItem is TimeOffRequests request)
            {
                request.Status = "Отклонено"; 

              
                context.SaveChanges();

                LoadTimeOffRequests(); 
            }
        }
    }
}

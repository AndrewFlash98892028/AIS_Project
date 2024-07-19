using AIS.Entities;
using System.Linq;
using System.Windows.Controls;

namespace AIS.Employee
{
    /// <summary>
    /// Логика взаимодействия для ViewTimeEntriesPage.xaml
    /// </summary>
    public partial class ViewTimeEntriesPage : Page
    {
        public ViewTimeEntriesPage(System.Collections.Generic.List<TimeEntries> timeEntries)
        {
            InitializeComponent();
            LoadTimeEntries();
        }
        private void LoadTimeEntries()
        {
            using (var context = new AISCEntities1())
            {
               
                int employeeID = 1; 

                var timeEntries = context.TimeEntries
                    .Where(te => te.EmployeeID == employeeID)
                    .ToList();

                dgTimeEntries.ItemsSource = timeEntries;
            }
        }
    }
}

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AIS.Entities;

namespace AIS.Admin
{
    public partial class DepartmentPage : Page
    {
        private ObservableCollection<Departments> departments;

        public DepartmentPage()
        {
            InitializeComponent();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            using (var context = new AISCEntities1())
            {
                departments = new ObservableCollection<Departments>(context.Departments.ToList());
                dgDepartments.ItemsSource = departments;
            }
        }

        private void btnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            AddDepartamentWindow addDepartmentWindow = new AddDepartamentWindow();
            if (addDepartmentWindow.ShowDialog() == true)
            {
                LoadDepartments(); 
            }
        }

        private void btnEditDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (dgDepartments.SelectedItem is Departments selectedDepartment)
            {
                EditDepartamentWindow editDepartmentWindow = new EditDepartamentWindow(selectedDepartment);
                if (editDepartmentWindow.ShowDialog() == true)
                {
                    LoadDepartments(); 
                }
            }
        }

        private void btnDeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (dgDepartments.SelectedItem is Departments selectedDepartment)
            {
                if (MessageBox.Show($"Вы уверенны что хотите удалить отдел '{selectedDepartment.DepartmentName}'?",
                                    "Подтверждаю", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var context = new AISCEntities1())
                    {
                        var department = context.Departments.Find(selectedDepartment.DepartmentID);
                        if (department != null)
                        {
                            context.Departments.Remove(department);
                            context.SaveChanges();
                            LoadDepartments(); 
                        }
                    }
                }
            }
        }

        private void dgDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditDepartment.IsEnabled = dgDepartments.SelectedItem != null;
            btnDeleteDepartment.IsEnabled = dgDepartments.SelectedItem != null;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            using (var context = new AISCEntities1())
            {
                departments = new ObservableCollection<Departments>(context.Departments
                                    .Where(dept => dept.DepartmentName.ToLower().Contains(searchText))
                                    .ToList());
                dgDepartments.ItemsSource = departments;
            }
        }
    }
}
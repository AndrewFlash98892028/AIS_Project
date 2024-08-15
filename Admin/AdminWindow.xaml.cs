using AIS.Employee;
using AIS.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AIS.Admin
{
    public partial class AdminWindow : Window
    {
        private DateTime onlineTime;
        private DispatcherTimer onlineTimer;

        private DispatcherTimer overtimeTimer;
        private bool isOvertime = false;
        private TimeSpan totalOvertime = TimeSpan.Zero;
        private TimeSpan startTime = TimeSpan.Zero;

        private DispatcherTimer inactivityTimer;
        private DateTime? clockInTime;
        private DateTime lastActivityTime;
        private AISCEntities1 context;
        private int currentEmployeeID;
        private static int clockedInEmployeesCount = 0;
        private TimeSpan workDayStartTime;
        private TimeSpan lunchBreakStartTime;
        private TimeSpan workDayEndTime;
        private int inactivityThresholdMinutes;
        private DateTime appStartTime;
        private TimeSpan totalTimeElapsed;
        private DispatcherTimer appTimer;

        private bool endOfWorkdayNotificationShown = false;
        private Dictionary<int, DateTime?> employeeClockInTimes = new Dictionary<int, DateTime?>();
        private AISCEntities1 dbContext = new AISCEntities1();
        private string connectionString = @"Data Source=DESKTOP-3VRKVMQ\SQLEXPRESS01;Initial Catalog=AISC;Integrated Security=True";
        private DispatcherTimer dashboardTimer;
        private Dictionary<string, List<SqlParameter>> procedureParameters = new Dictionary<string, List<SqlParameter>>
        {
            {"GetActiveEmployeesByDepartment", new List<SqlParameter> { new SqlParameter("@DepartmentName", System.Data.SqlDbType.VarChar) }},
            {"GetAverageHoursWorkedByDepartment", new List<SqlParameter>
                {
                    new SqlParameter("@DepartmentID", System.Data.SqlDbType.Int),
                    new SqlParameter("@StartDate", System.Data.SqlDbType.Date),
                    new SqlParameter("@EndDate", System.Data.SqlDbType.Date)
                }
            },
            {"GetDepartmentsWithActiveProjects", new List<SqlParameter>() },
            {"GetEmployeesByJobTitle", new List<SqlParameter> { new SqlParameter("@JobTitleID", System.Data.SqlDbType.Int) }},
            {"GetEmployeesByProject", new List<SqlParameter> { new SqlParameter("@ProjectID", System.Data.SqlDbType.Int) }},
            {"GetEmployeesWithCompletedTasks", new List<SqlParameter> { new SqlParameter("@ProjectID", System.Data.SqlDbType.Int) }},
            {"GetEmployeesWithoutSchedules", new List<SqlParameter>() },
            {"GetEmployeesWithOverlappingTimeOff", new List<SqlParameter>
                {
                    new SqlParameter("@StartDate", System.Data.SqlDbType.Date),
                    new SqlParameter("@EndDate", System.Data.SqlDbType.Date)
                }
            },
            {"GetPayrollByEmployeeAndPeriod", new List<SqlParameter>
                {
                    new SqlParameter("@EmployeeID", System.Data.SqlDbType.Int),
                    new SqlParameter("@PayPeriodStart", System.Data.SqlDbType.Date),
                    new SqlParameter("@PayPeriodEnd", System.Data.SqlDbType.Date)
                }
            },
            {"GetPendingTimeOffRequests", new List<SqlParameter>() },
            {"GetTotalOvertimeByEmployee", new List<SqlParameter>
                {
                    new SqlParameter("@EmployeeID", System.Data.SqlDbType.Int),
                    new SqlParameter("@StartDate", System.Data.SqlDbType.Date),
                    new SqlParameter("@EndDate", System.Data.SqlDbType.Date)
                }
            },
            {"GetUpcomingYear", new List<SqlParameter>
                {
                    new SqlParameter("@TargetMonth", System.Data.SqlDbType.Int),
                    new SqlParameter("@TargetDay", System.Data.SqlDbType.Int)
                }
            },
            {"GetProjectsByDepartment", new List<SqlParameter> { new SqlParameter("@DepartmentID", System.Data.SqlDbType.Int) }},
            {"GetTasksByProjectAndStatus", new List<SqlParameter>
                {
                    new SqlParameter("@ProjectID", System.Data.SqlDbType.Int),
                    new SqlParameter("@Status", System.Data.SqlDbType.VarChar, 50)
                }
            },
            {"GetTotalTimeOffTakenByEmployee", new List<SqlParameter>
                {
                    new SqlParameter("@EmployeeID", System.Data.SqlDbType.Int),
                    new SqlParameter("@Year", System.Data.SqlDbType.Int)
                }
            }
        };

        private Dictionary<string, string> russianProcedureNames = new Dictionary<string, string>
        {
           {"GetActiveEmployeesByDepartment", "Активные сотрудники отдела"},
    {"GetAverageHoursWorkedByDepartment", "Среднее количество отработанных часов (по отделу)"},
    {"GetDepartmentsWithActiveProjects", "Отделы с активными проектами"},
    {"GetEmployeesByJobTitle", "Сотрудники по должности"},
    {"GetEmployeesByProject", "Сотрудники проекта"},
    {"GetEmployeesWithCompletedTasks", "Сотрудники с выполненными задачами"},
    {"GetEmployeesWithoutSchedules", "Сотрудники без графиков"},
    {"GetEmployeesWithOverlappingTimeOff", "Сотрудники с перекрывающимися отпусками"},
    {"GetPayrollByEmployeeAndPeriod", "Расчет зарплаты (сотрудник и период)"},
    {"GetPendingTimeOffRequests", "Ожидающие запросы на отпуск"},
    {"GetTotalOvertimeByEmployee", "Сверхурочные часы сотрудника"},
    {"GetUpcomingYear", "Предстоящие рабочие годовщины"},
    {"GetProjectsByDepartment", "Проекты отдела"},
    {"GetTasksByProjectAndStatus", "Задачи по проекту и статусу"},
    {"GetTotalTimeOffTakenByEmployee", "Общее количество отгулов сотрудника"}
        };
        private Dictionary<string, string> russianParameterNames = new Dictionary<string, string>
{
    {"@DepartmentID", "ID отдела"},
     {"@DepartmentName", "Имя отдела"},
    {"@StartDate", "Дата начала"},
    {"@EndDate", "Дата окончания"},
    {"@JobTitleID", "ID должности"},
    {"@ProjectID", "ID проекта"},
    {"@EmployeeID", "ID сотрудника"},
    {"@PayPeriodStart", "Начало расчетного периода"},
    {"@PayPeriodEnd", "Конец расчетного периода"},
    {"@TargetMonth", "Месяц"},
    {"@TargetDay", "День"},
    {"@Status", "Статус"},
    {"@Year", "Год"}
};
        private Dictionary<string, string> russianOutputColumnNames = new Dictionary<string, string>
{
    {"DepartmentID", "ID отдела"},
    {"StartDate", "Дата начала"},
     {"@DepartmentName", "Имя отдела"},
    {"EndDate", "Дата окончания"},
    {"JobTitleID", "ID должности"},
    {"ProjectID", "ID проекта"},
    {"EmployeeID", "ID сотрудника"},
    {"PayPeriodStart", "Начало расчетного периода"},
    {"PayPeriodEnd", "Конец расчетного периода"},
    {"TargetMonth", "Месяц"},
    {"TargetDay", "День"},
    {"Status", "Статус"},
    {"Year", "Год"},
    {"FirstName", "Имя Фамилия"},
    {"LastName", "Отчество"},
    {"HireDate", "Дата найма"}

};
        private bool lunchBreakNotificationShown;

        public AdminWindow(int employeeID)
        {

            InitializeComponent();
            context = new AISCEntities1();
            currentEmployeeID = employeeID;

            LoadEmployeeName();
            LoadSettings();

            //workTimer = new DispatcherTimer();
            // workTimer.Interval = TimeSpan.FromSeconds(1);
            // workTimer.Tick += WorkTimer_Tick;
            workTimer = new DispatcherTimer();
            workTimer.Interval = TimeSpan.FromSeconds(1);
            workTimer.Tick += WorkTimer_Tick;
            inactivityTimer = new DispatcherTimer();
            inactivityTimer.Interval = TimeSpan.FromMinutes(1);
            inactivityTimer.Tick += InactivityTimer_Tick;

            lastActivityTime = DateTime.UtcNow;
            _ = frmAdminContent.Navigate(new EmployeePage());

            dashboardTimer = new DispatcherTimer();
            dashboardTimer.Interval = TimeSpan.FromSeconds(5);
            dashboardTimer.Tick += DashboardTimer_Tick;
            dashboardTimer.Start();
            UpdateDashboard();
            PopulateComboBox();

            
            appStartTime = DateTime.Now;
            appTimer = new DispatcherTimer();
            appTimer.Interval = TimeSpan.FromSeconds(1);
            appTimer.Tick += AppTimer_Tick;
            appTimer.Start();

           
            onlineTimer = new DispatcherTimer();
            onlineTimer.Interval = TimeSpan.FromSeconds(1);
            onlineTimer.Tick += OnlineTimer_Tick;



        }

        private void AppTimer_Tick(object sender, EventArgs e)
        {
            totalTimeElapsed = DateTime.Now - appStartTime;
            txtTimeElapsed.Text = $"(Офлайн) {totalTimeElapsed:hh\\:mm\\:ss}";
        }

        private void OnlineTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan onlineDuration = DateTime.Now - onlineTime;
            txtTimeElapsed.Text = $" {onlineDuration:hh\\:mm\\:ss}";
        }

        private Dictionary<int, TimeSpan> employeeOvertime = new Dictionary<int, TimeSpan>();
        private DispatcherTimer workTimer;

        private void EmployeeWindow_EmployeeOvertimeUpdated(object sender, EmployeeOvertimeEventArgs e)
        {

            if (employeeOvertime.ContainsKey(e.EmployeeId))
            {
                employeeOvertime[e.EmployeeId] = e.Overtime;
            }
            else
            {
                employeeOvertime.Add(e.EmployeeId, e.Overtime);
            }


            Dispatcher.Invoke(() =>
            {
                UpdateDashboard();
            });
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            if (clockInTime != null)
            {
                TimeSpan elapsed = DateTime.Now - clockInTime.Value;
                if (isOvertime)
                {
                    lblOvertimeHours.Text = elapsed.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    txtTimeElapsed.Text = elapsed.ToString(@"hh\:mm\:ss");
                    if (DateTime.Now.TimeOfDay >= workDayEndTime)
                    {
                        isOvertime = true;
                    }
                }

                if (DateTime.Now.TimeOfDay > lunchBreakStartTime + GetLunchBreakDuration() &&
                    clockInTime.Value.TimeOfDay < lunchBreakStartTime)
                {
                    _ = GetLunchBreakDuration();
                }

                // txtClockedInStatus.Text = elapsed.ToString(@"hh\:mm\:ss");


                if (DateTime.Now.TimeOfDay >= lunchBreakStartTime &&
                    DateTime.Now.TimeOfDay < lunchBreakStartTime + GetLunchBreakDuration() &&
                    !lunchBreakNotificationShown)
                {
                    ShowLunchBreakReminder();
                    lunchBreakNotificationShown = true;
                }

                else if (DateTime.Now.TimeOfDay > lunchBreakStartTime + GetLunchBreakDuration())
                {
                    lunchBreakNotificationShown = false;
                }


                if (DateTime.Now.TimeOfDay >= workDayEndTime && !endOfWorkdayNotificationShown)
                {
                    ShowEndOfWorkdayNotification();
                    //  workTimer.Stop(); 
                    endOfWorkdayNotificationShown = true;
                }
            }
        }
        private TimeSpan GetLunchBreakDuration()
        {
            using (var db = new AISCEntities1())
            {
                return TimeSpan.FromMinutes(
                    int.Parse(db.Settings.FirstOrDefault(s => s.SettingName == "DefaultLunchBreak")?.SettingValue ?? "60")
                );
            }
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.UtcNow - lastActivityTime;
            if (elapsed.TotalMinutes >= inactivityThresholdMinutes && clockInTime != null)
            {
                _ = MessageBox.Show("Внимание: Вы были неактивны более 5 минут.", "Предупреждение о неактивности");
                lastActivityTime = DateTime.UtcNow;
            }
        }

        private void ShowLunchBreakReminder()
        {
            _ = MessageBox.Show("Напоминание: Время обеденного перерыва!", "Обеденный перерыв");
        }

        private void ShowEndOfWorkdayNotification()
        {
            _ = MessageBox.Show("Рабочий день окончен.", "Окончание рабочего дня");
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            lastActivityTime = DateTime.UtcNow;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            lastActivityTime = DateTime.UtcNow;
        }
        private void LoadSettings()
        {
            try
            {
                using (var db = new AISCEntities1())
                {
                    workDayStartTime = TimeSpan.Parse(db.Settings.FirstOrDefault(s => s.SettingName == "DefaultStartTime")?.SettingValue ?? "08:00");
                    lunchBreakStartTime = TimeSpan.Parse(db.Settings.FirstOrDefault(s => s.SettingName == "LunchBreakStartTime")?.SettingValue ?? "12:00");
                    workDayEndTime = TimeSpan.Parse(db.Settings.FirstOrDefault(s => s.SettingName == "DefaultEndTime")?.SettingValue ?? "17:00");
                    inactivityThresholdMinutes = int.Parse(db.Settings.FirstOrDefault(s => s.SettingName == "IdleThreshold")?.SettingValue ?? "5");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}");
                workDayStartTime = new TimeSpan(8, 0, 0);
                lunchBreakStartTime = new TimeSpan(12, 0, 0);
                workDayEndTime = new TimeSpan(17, 0, 0);
                inactivityThresholdMinutes = 5;
            }
        }







        private void LoadEmployeeName()
        {
            try
            {
                using (var db = new AISCEntities1())
                {
                    var employee = db.Employees.Find(currentEmployeeID);
                    if (employee != null)
                    {
                        txtEmployeeName.Text = $"{employee.FirstName} {employee.LastName}";
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Ошибка при загрузке имени сотрудника: {ex.Message}");
            }
        }

        private void DashboardTimer_Tick(object sender, EventArgs e)
        {
            UpdateDashboard();
        }
        private void RefreshDashboard()
        {
            Dispatcher.Invoke(() =>
            {
                UpdateDashboard();
            });
        }

        private void UpdateDashboard()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    using (var command = new SqlCommand("SELECT COUNT(*) FROM TimeEntries WHERE CAST(ClockInTime AS DATE) = CAST(GETDATE() AS DATE) AND ClockOutTime IS NULL", connection))
                    {
                        int clockedInEmployees = (int)command.ExecuteScalar();
                        lblClockedInEmployees.Text = clockedInEmployees.ToString();
                    }


                    using (var command = new SqlCommand("SELECT SUM(DATEDIFF(minute, ClockInTime, ClockOutTime)) / 60.0 FROM TimeEntries WHERE CAST(ClockInTime AS DATE) = CAST(GETDATE() AS DATE) AND ClockOutTime IS NOT NULL", connection))
                    {
                        var result = command.ExecuteScalar();
                        double totalHours = result is DBNull ? 0.0 : Convert.ToDouble(result);
                        lblTotalHoursWorked.Text = totalHours.ToString("F2");
                    }


                    using (var command = new SqlCommand(@"
                SELECT SUM(CASE 
                               WHEN DATEDIFF(minute, ClockInTime, ClockOutTime) > 480 
                               THEN DATEDIFF(minute, ClockInTime, ClockOutTime) - 480 
                               ELSE 0 
                           END) / 60.0
                FROM TimeEntries 
                WHERE CAST(ClockInTime AS DATE) = CAST(GETDATE() AS DATE)
                  AND ClockOutTime IS NOT NULL", connection))
                    {
                        var result = command.ExecuteScalar();
                        double overtimeHours = result is DBNull ? 0.0 : Convert.ToDouble(result);
                        lblOvertimeHours.Text = overtimeHours.ToString("F2");
                    }

                    using (var command = new SqlCommand("SELECT COUNT(*) FROM TimeOffRequests WHERE Status = 'Ожидание'", connection))
                    {
                        int pendingRequests = (int)command.ExecuteScalar();
                        lblPendingTimeOffRequests.Text = pendingRequests.ToString();
                    }
                    Dispatcher.Invoke(() =>
                    {


                        lblClockedInEmployees.Text = clockedInEmployeesCount.ToString();
                    });
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Ошибка обновления: {ex.Message}");
            }
        }
        private string GetDashboardValue(SqlConnection connection, string query, string format = null)
        {
            using (var command = new SqlCommand(query, connection))
            {
                var result = command.ExecuteScalar();
                if (result is DBNull)
                {
                    return "0";
                }
                else if (format == "F2")
                {
                    return Convert.ToDouble(result).ToString("F2");
                }
                else
                {
                    return result.ToString();
                }
            }
        }
        private string CalculateTotalHoursWorked(SqlConnection connection)
        {
            using (var command = new SqlCommand(@"
        SELECT SUM(DATEDIFF(minute, 
                       ClockInTime, 
                       CASE 
                           WHEN ClockOutTime IS NULL THEN GETDATE() 
                           ELSE ClockOutTime 
                       END
                      )) 
        FROM TimeEntries
        WHERE CAST(ClockInTime AS date) = CAST(GETDATE() AS date)", connection))
            {
                var result = command.ExecuteScalar();
                int totalMinutes = result is DBNull ? 0 : Convert.ToInt32(result);

                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;

                return $"{hours}:{minutes:D2}";
            }
        }

        private void btnEmployeeManagement_Click(object sender, RoutedEventArgs e)
        {
            NavigateAndHandleClockInOut(new EmployeePage());
            _ = frmAdminContent.Navigate(new EmployeePage());
            if (clockInTime == null)
            {
                StartWork();
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";
                employeeClockInTimes[currentEmployeeID] = DateTime.UtcNow;
                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                clockedInEmployeesCount++;
                //  workTimer.Start();
                inactivityTimer.Start();

                if (clockInTime.Value.TimeOfDay >= workDayEndTime)
                {
                    totalOvertime = clockInTime.Value.TimeOfDay - workDayEndTime;
                    Dispatcher.Invoke(() =>
                    {
                        lblOvertimeHours.Text = totalOvertime.ToString(@"hh\:mm");
                    });
                }


                ToggleClockInOutButton(false);
            }
            else
            {
                DateTime clockOutTime = DateTime.UtcNow;
                inactivityTimer.Stop();
                clockedInEmployeesCount++;
                employeeClockInTimes[currentEmployeeID] = null;
                if (clockOutTime.TimeOfDay > workDayEndTime)
                {
                    totalOvertime += clockOutTime.TimeOfDay - workDayEndTime;
                }

                totalOvertime = TimeSpan.Zero;

                RefreshDashboard();


                ToggleClockInOutButton(true);
            }
        }

        private void ToggleClockInOutButton(bool isEnabled)
        {
            Dispatcher.Invoke(() =>
            {
                // btnEmployeeManagement_Click.IsEnabled = isEnabled;
            });
        }

        private void StartWork()
        {
            if (clockInTime == null)
            {
                clockInTime = DateTime.Now;
                txtClockedInStatus.Text = "Да";
                txtClockedInStatus.UpdateLayout();
                _ = Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate { }));
                if (clockInTime.Value.TimeOfDay >= workDayEndTime)
                {
                    clockedInEmployeesCount++;
                    UpdateDashboard();
                    overtimeTimer = new DispatcherTimer();
                    overtimeTimer.Interval = TimeSpan.FromSeconds(1);
                    overtimeTimer.Tick += OvertimeTimer_Tick;
                    overtimeTimer.Start();
                }
                txtTimeElapsed.Text = $"(Online) {totalTimeElapsed:hh\\:mm\\:ss}";
                onlineTime = DateTime.Now;
                onlineTimer.Start();

                appTimer.Stop();

              
                inactivityTimer.Start();
            }
        }
        private void OvertimeTimer_Tick(object sender, EventArgs e)
        {
            if (clockInTime != null)
            {
                TimeSpan overtime = DateTime.Now - clockInTime.Value;
                lblOvertimeHours.Text = overtime.ToString(@"hh\:mm\:ss");
            }
        }
        private void btnDepartmentManagement_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new DepartmentPage());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                //workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }

        private void btnTimeEntries_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new TimeEntriesPage());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                //  workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }
        private void NavigateAndHandleClockInOut(Page page)
        {
            _ = frmAdminContent.Navigate(page);

            if (clockInTime == null)
            {
                StartWork();
            }
            else
            {
                ClockOut();
            }
        }

        private void ClockOut()
        {
            clockedInEmployeesCount--;
            UpdateDashboard();
            txtTimeElapsed.Text = $"(Online) {totalTimeElapsed:hh\\:mm\\:ss}";
            onlineTimer.Stop();

            txtTimeElapsed.Text = $"(Offline) {totalTimeElapsed:hh\\:mm\\:ss}";

            appTimer.Start();
        }

        private void btnTimeOff_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new TimeOffPage());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }


                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }

        private void btnSchedules_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new SchedulesPage());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                //  workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }

        private void btnPayroll_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new PayrollPage());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                //  workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {


            _ = frmAdminContent.Navigate(new ReportsPage(dbContext));
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                //  workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }

        }

        private void btnSystemSettings_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new SystemSettingsPage());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                // workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }

        private void btnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            _ = frmAdminContent.Navigate(new UserManagment());
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да";

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                // workTimer.Start();
                inactivityTimer.Start();



            }
            else
            {
                _ = DateTime.UtcNow;
                inactivityTimer.Stop();





            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void frmAdminContent_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
        private void PopulateComboBox()
        {
            foreach (var procedureName in procedureParameters.Keys)
            {
                _ = cmbStoredProcedures.Items.Add(russianProcedureNames.ContainsKey(procedureName)
                                      ? russianProcedureNames[procedureName]
                                      : procedureName);
            }
        }
        private void cmbStoredProcedures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedDisplayName = cmbStoredProcedures.SelectedItem?.ToString();
            string selectedProcedure = russianProcedureNames.FirstOrDefault(x => x.Value == selectedDisplayName).Key;
            ParameterInputs.Children.Clear();

            if (selectedProcedure != null && procedureParameters.ContainsKey(selectedProcedure))
            {
                foreach (var parameter in procedureParameters[selectedProcedure])
                {
                    TextBlock label = new TextBlock
                    {
                        Text = russianParameterNames.ContainsKey(parameter.ParameterName)
                                     ? russianParameterNames[parameter.ParameterName]
                                     : parameter.ParameterName + ":"
                    };

                    Control inputControl = parameter.SqlDbType == System.Data.SqlDbType.Date ? new DatePicker() : (Control)new TextBox();

                    _ = ParameterInputs.Children.Add(label);
                    _ = ParameterInputs.Children.Add(inputControl);
                    parameter.Value = inputControl;
                }
            }
        }
        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            string selectedDisplayName = cmbStoredProcedures.SelectedItem?.ToString();
            string selectedProcedure = russianProcedureNames.FirstOrDefault(x => x.Value == selectedDisplayName).Key;

            if (string.IsNullOrEmpty(selectedProcedure) || !procedureParameters.ContainsKey(selectedProcedure)) return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(selectedProcedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        foreach (var parameterTemplate in procedureParameters[selectedProcedure])
                        {
                            SqlParameter newParameter = new SqlParameter(parameterTemplate.ParameterName, parameterTemplate.SqlDbType);
                            if (parameterTemplate.Value is Control inputControl)
                            {
                                if (inputControl is DatePicker datePicker && datePicker.SelectedDate.HasValue)
                                    newParameter.Value = datePicker.SelectedDate.Value;
                                else if (inputControl is TextBox textBox)
                                    newParameter.Value = textBox.Text;
                            }
                            _ = command.Parameters.Add(newParameter);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            txtOutput.Clear();
                            while (reader.Read())
                            {
                                string rowData = "";
                                for (int i = 0; i < reader.FieldCount; i++)
                                    rowData += $"{(russianOutputColumnNames.ContainsKey(reader.GetName(i)) ? russianOutputColumnNames[reader.GetName(i)] : reader.GetName(i))}: {(reader[i] != DBNull.Value ? reader[i] : "N/A")}, ";

                                txtOutput.AppendText(rowData + "\n");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Ошибка: {ex.Message}");
            }

            if (clockInTime != null) return;

            clockInTime = DateTime.UtcNow;
            txtClockedInStatus.Text = "Да";

            if (clockInTime.Value.TimeOfDay > workDayStartTime)
                _ = MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");

            //  workTimer.Start();
            inactivityTimer.Start();
        }/// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnRefreshDashboard_Click(object sender, RoutedEventArgs e)
        {
            UpdateDashboard();
        }
    }


}

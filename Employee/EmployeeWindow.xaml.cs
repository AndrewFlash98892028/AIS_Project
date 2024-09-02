using AIS.Entities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AIS.Employee
{
    public partial class EmployeeWindow : Window
    {
        private bool isApplicationClosing = false;
        private bool lunchBreakNotificationShown = false;
        private DispatcherTimer workTimer;
        private DispatcherTimer inactivityTimer;
        private DateTime? clockInTime;
        private DateTime lastActivityTime;
        private AISCEntities1 context;
        private int currentEmployeeID;
        public static event EventHandler<EmployeeOvertimeEventArgs> EmployeeOvertimeUpdated;
        private TimeSpan workDayStartTime;
        private TimeSpan lunchBreakStartTime;
        private TimeSpan workDayEndTime;
        private int inactivityThresholdMinutes;

        private bool endOfWorkdayNotificationShown = false;

        public EmployeeWindow(int employeeID)
        {
            InitializeComponent();
            context = new AISCEntities1();
            currentEmployeeID = employeeID;

            LoadEmployeeName();
            LoadSettings();

            workTimer = new DispatcherTimer();
            workTimer.Interval = TimeSpan.FromSeconds(1);
            workTimer.Tick += WorkTimer_Tick;

            inactivityTimer = new DispatcherTimer();
            inactivityTimer.Interval = TimeSpan.FromMinutes(1);
            inactivityTimer.Tick += InactivityTimer_Tick;

            lastActivityTime = DateTime.UtcNow;
            Closing += EmployeeWindow_Closing;
        }
        private void OnEmployeeOvertimeUpdated(TimeSpan overtime)
        {
            EmployeeOvertimeUpdated?.Invoke(this, new EmployeeOvertimeEventArgs(currentEmployeeID, overtime));
        }
        private void EmployeeWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (clockInTime != null && !isApplicationClosing)
            {
                isApplicationClosing = true; 

                
                DateTime clockOutTime = DateTime.Now;
                SaveTimeEntry(clockInTime.Value, clockOutTime);

                TimeSpan overtime = clockOutTime.TimeOfDay > workDayEndTime
                  ? clockOutTime.TimeOfDay - workDayEndTime
                  : TimeSpan.Zero;

               
                OnEmployeeOvertimeUpdated(overtime);

                MessageBox.Show("Автоматический выход.");
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
                MessageBox.Show($"Ошибка при загрузке имени сотрудника: {ex.Message}");
            }
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
                MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}");
                workDayStartTime = new TimeSpan(8, 0, 0);
                lunchBreakStartTime = new TimeSpan(12, 0, 0);
                workDayEndTime = new TimeSpan(17, 0, 0);
                inactivityThresholdMinutes = 5;
            }
        }

        private void btnClockInOut_Click(object sender, RoutedEventArgs e)
        {
            if (clockInTime == null)
            {
                clockInTime = DateTime.UtcNow;
                txtClockedInStatus.Text = "Да"; 

                if (clockInTime.Value.TimeOfDay > workDayStartTime)
                {
                    MessageBox.Show("Внимание: Вы опоздали на работу!", "Опоздание");
                }

                workTimer.Start();
                inactivityTimer.Start();

                frmEmployeeContent.Navigate(new ClockInOutPage(context, clockInTime.Value, currentEmployeeID));
                btnClockInOut.Content = "Выйти из проектов";
            }
            else
            {
                DateTime clockOutTime = DateTime.UtcNow;
                inactivityTimer.Stop();

                SaveTimeEntry(clockInTime.Value, clockOutTime);

                TimeSpan overtime = clockOutTime.TimeOfDay > workDayEndTime
                    ? clockOutTime.TimeOfDay - workDayEndTime
                    : TimeSpan.Zero;

                string formattedOvertime = overtime.ToString(@"hh\:mm\:ss");
                MessageBox.Show($"Вы отметили выход. Сверхурочные: {formattedOvertime}", "Выход");

                clockInTime = null;
                txtClockedInStatus.Text = "Нет"; 
                frmEmployeeContent.Content = null;
                btnClockInOut.Content = "Проекты";
            }

            lastActivityTime = DateTime.UtcNow;
        }

        private void SaveTimeEntry(DateTime clockIn, DateTime clockOut)
        {
            try
            {
                var newTimeEntry = new TimeEntries
                {
                    EmployeeID = currentEmployeeID,
                    ClockInTime = clockIn,
                    ClockOutTime = clockOut,
                    Approved = false
                };

                TimeSpan lunchBreakDuration = GetLunchBreakDuration();

                if (clockIn.TimeOfDay < lunchBreakStartTime && clockOut.TimeOfDay > lunchBreakStartTime + lunchBreakDuration)
                {
                    newTimeEntry.ClockOutTime -= lunchBreakDuration;
                }

                context.TimeEntries.Add(newTimeEntry);
                context.SaveChanges();

                MessageBox.Show("Запись времени успешно сохранена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении записи времени: {ex.Message}");
            }
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            if (clockInTime != null)
            {
                TimeSpan elapsed = DateTime.UtcNow - clockInTime.Value;

                
                if (DateTime.UtcNow.TimeOfDay >= lunchBreakStartTime &&
                    DateTime.UtcNow.TimeOfDay < lunchBreakStartTime + GetLunchBreakDuration() &&
                    !lunchBreakNotificationShown)
                {
                    ShowLunchBreakReminder();
                    lunchBreakNotificationShown = true;
                }
              
                else if (DateTime.UtcNow.TimeOfDay > lunchBreakStartTime + GetLunchBreakDuration())
                {
                    lunchBreakNotificationShown = false;
                }

               
                if (elapsed > GetLunchBreakDuration() && clockInTime.Value.TimeOfDay < lunchBreakStartTime)
                {
                    elapsed -= GetLunchBreakDuration();
                }


                if (DateTime.UtcNow.TimeOfDay >= workDayEndTime && clockInTime.Value.TimeOfDay <= workDayEndTime)
                {
                    TimeSpan overtime = DateTime.UtcNow - clockInTime.Value.Date + workDayEndTime; 
                    txtTimeElapsed.Text = elapsed.ToString(@"hh\:mm\:ss") +
                                         " (Переработка: " + overtime.ToString(@"hh\:mm\:ss") + ")";
                }
                else
                {
                    txtTimeElapsed.Text = elapsed.ToString(@"hh\:mm\:ss");
                }


                if (DateTime.UtcNow.TimeOfDay >= workDayEndTime && !endOfWorkdayNotificationShown)
                {
                    ShowEndOfWorkdayNotification();
                    endOfWorkdayNotificationShown = true;
                }
            }
        }

        private TimeSpan GetLunchBreakDuration()
        {
            return TimeSpan.FromMinutes(
                int.Parse(context.Settings.FirstOrDefault(s => s.SettingName == "DefaultLunchBreak")?.SettingValue ?? "60")
            );
        }

        private void InactivityTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.UtcNow - lastActivityTime;
            if (elapsed.TotalMinutes >= inactivityThresholdMinutes && clockInTime != null)
            {
                MessageBox.Show("Внимание: Вы были неактивны более 5 минут.", "Предупреждение о неактивности");
                lastActivityTime = DateTime.UtcNow;
            }
        }

        private void ShowLunchBreakReminder()
        {
            MessageBox.Show("Напоминание: Время обеденного перерыва!", "Обеденный перерыв");
        }

        private void ShowEndOfWorkdayNotification()
        {
            MessageBox.Show("Рабочий день окончен.", "Окончание рабочего дня");
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            lastActivityTime = DateTime.UtcNow;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            lastActivityTime = DateTime.UtcNow;
        }

        private void btnViewTimeEntries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AISCEntities1())
                {

                    var timeEntries = db.TimeEntries
                        .Where(t => t.EmployeeID == currentEmployeeID)
                        .ToList();


                    frmEmployeeContent.Navigate(new ViewTimeEntriesPage(timeEntries));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void btnRequestTimeOff_Click(object sender, RoutedEventArgs e)
        {

            RequestTimeOffWindow requestWindow = new RequestTimeOffWindow(currentEmployeeID);
            if (requestWindow.ShowDialog() == true)
            {

                TimeOffRequests newRequest = requestWindow.NewTimeOffRequest;
                try
                {
                    using (var db = new AISCEntities1())
                    {
                        db.TimeOffRequests.Add(newRequest);
                        db.SaveChanges();
                        MessageBox.Show("Запрос на отпуск отправлен!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка отправки запроса: {ex.Message}");
                }
            }
        }

        private void btnViewSchedules_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AISCEntities1())
                {

                    var schedules = db.Schedules
                        .Where(s => s.EmployeeID == currentEmployeeID)
                        .ToList();


                    frmEmployeeContent.Navigate(new ViewSchedulesPage(schedules));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки графиков: {ex.Message}");
            }
        }

        private void btnViewPayroll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new AISCEntities1())
                {

                    var payrollData = db.Payroll
                        .Where(p => p.EmployeeID == currentEmployeeID)
                        .ToList();


                    frmEmployeeContent.Navigate(new ViewPayrollPage(payrollData));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки зарплаты: {ex.Message}");
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
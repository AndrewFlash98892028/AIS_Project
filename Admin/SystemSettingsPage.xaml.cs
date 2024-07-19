using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AIS.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using OxyPlot;

namespace AIS.Admin
{
    public partial class SystemSettingsPage : Page
    {
        private readonly AISCEntities1 context = new AISCEntities1();
        private readonly string connectionString = @"Data Source=DESKTOP-VT15G9R\SQLEXPRESS01;Initial Catalog=AISC;Integrated Security=True";

        private readonly Dictionary<string, string> russianOutputColumnNames = new Dictionary<string, string>
        {
            {"DepartmentID", "ID отдела"},
            {"StartDate", "Дата начала"},
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
            {"FirstName", "Имя"},
            {"LastName", "Фамилия"},
            {"HireDate", "Дата найма"},
            {"ClockInTime", "Время входа"},
            {"ClockOutTime", "Время выхода"},
            {"TimeEntryID", "ID записи времени"},
            {"Approved", "Одобрено"},
            {"Note", "Примечание"},
            {"Overtime", "Переработка"},
            {"RequestID", "ID запроса"},
            {"Reason", "Причина"},
            {"ApprovedBy", "Одобрено кем"},
            {"AvailableBalance", "Доступный баланс"},
            {"UserID", "ID пользователя"},
            {"Username", "Имя пользователя"},
            {"Password", "Пароль"},
            {"RoleID", "ID роли"},
            {"AttendanceID", "ID посещаемости"},
            {"AttendanceStatus", "Статус посещаемости"},
            {"LogID", "ID журнала"},
            {"Action", "Действие"},
            {"Timestamp", "Время"},
            {"Details", "Подробности"},
            {"ProjectName", "Название проекта"},
            {"ShiftStart", "Начало смены"},
            {"ShiftEnd", "Конец смены"},
            {"DayOfWeek", "День недели"},
            {"ScheduleType", "Тип расписания"},
            {"TaskID", "ID задачи"},
            {"TaskName", "Название задачи"},
            {"GrossPay", "Валовая зарплата"},
            {"NetPay", "Чистая зарплата"},
            {"Deductions", "Вычеты"},
            {"Taxes", "Налоги"},
            {"SettingID", "ID настройки"},
            {"SettingName", "Название настройки"},
            {"SettingValue", "Значение настройки"}
        };

        private readonly Dictionary<string, string> tableTranslations = new Dictionary<string, string>
        {
            { "Departments", "Отделы" },
            { "Employees", "Сотрудники" },
            { "JobTitles", "Должности" },
            { "Payroll", "Зарплата" },
            { "Projects", "Проекты" },
            { "Schedules", "Графики" },
            { "Settings", "Настройки" },
            { "Tasks", "Задачи" },
            { "TimeEntries", "Записи времени" },
            { "TimeOffRequests", "Запросы на отгул" },
            { "Users", "Пользователи" },
            { "AuditLog", "Журнал аудита" },
            { "Attendance", "Посещаемость" },
              { "PasswordHashInfo", "Хэш паролей" },
                { "UserPermissions", "Привелегии пользователй" },
            { "Permissions", "Разрешения" }
        };

        private readonly Dictionary<string, string> columnTranslations = new Dictionary<string, string>
        {
            {"DepartmentID", "ID отдела"},
            {"DepartmentName", "Название отдела"},
            {"StartDate", "Дата начала"},
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
            {"FirstName", "Имя"},
            {"LastName", "Фамилия"},
            {"HireDate", "Дата найма"},
            {"ContactInfo", "Контактные данные"},
            {"TerminationDate", "Дата увольнения"},
            {"HourlyRate", "Почасовая ставка"},
            {"JobTitle", "Должность"},
            {"PayrollID", "Номер"},
            {"ScheduleID", "Номер графика"},
            {"Date", "Дата"},
            {"PermissionID", "ID разрешения"},
            {"PermissionName", "Имя разрешения"},
            {"Description", "Описание"},
            {"IsActive", "Статус"},
            {"ClockInTime", "Время входа"},
            {"ClockOutTime", "Время выхода"},
            {"TimeEntryID", "ID записи времени"},
            {"Approved", "Одобрено"},
            {"Note", "Примечание"},
            {"Overtime", "Переработка"},
            {"RequestID", "ID запроса"},
            {"Reason", "Причина"},
            {"ApprovedBy", "Одобрено кем"},
            {"AvailableBalance", "Доступный баланс"},
            {"UserID", "ID пользователя"},
            {"Username", "Имя пользователя"},
            {"Password", "Пароль"},
            {"RoleID", "ID роли"},
            {"AttendanceID", "ID посещаемости"},
            {"AttendanceStatus", "Статус посещаемости"},
            {"LogID", "ID журнала"},
            {"Action", "Действие"},
            {"Timestamp", "Время"},
            {"Details", "Подробности"},
            {"ProjectName", "Название проекта"},
            {"ShiftStart", "Начало смены"},
            {"ShiftEnd", "Конец смены"},
            {"DayOfWeek", "День недели"},
            {"ScheduleType", "Тип расписания"},
            {"TaskID", "ID задачи"},
            {"TaskName", "Название задачи"},
            {"GrossPay", "Валовая зарплата"},
            {"NetPay", "Чистая зарплата"},
            {"Deductions", "Вычеты"},
            {"Taxes", "Налоги"},
            {"SettingID", "ID настройки"},
            {"SettingName", "Название настройки"},
            {"SettingValue", "Значение настройки"}
        };

        public SystemSettingsPage()
        {
            InitializeComponent();
            LoadSettings();
            PopulatePrinters();
          

            cmbTables.ItemsSource = GetTableNames();
            HighlightCalendarDays();
        }
        private void HighlightCalendarDays()
        {
            HighlightRestorePointDates(true);
            HighlightRestorePointDates(false);
        }

        private void reportCalendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
           
            HighlightCalendarDays();
        }

        private void HighlightRestorePointDates(bool isBackup)
        {
            List<DateTime> restoreDates = GetRestorePointDates(isBackup);

            foreach (DateTime date in restoreDates)
            {
                CalendarDayButton button = FindDayButton(date);
                if (button != null)
                {
                    if (isBackup)
                    {
                        button.Background = System.Windows.Media.Brushes.Blue; 
                    }
                    else
                    {
                        button.Background = System.Windows.Media.Brushes.Red; 
                    }
                }
            }
        }


        private List<DateTime> GetRestorePointDates(bool isBackup)
        {
            string targetDirectory = isBackup
                ? @"C:\Users\Андрей\Desktop\Лебединая песня\Программа\AIS\РK"
                : GetReportsFolderPath();

            if (!Directory.Exists(targetDirectory))
            {
                return new List<DateTime>();
            }

            List<DateTime> restorePointDates = new List<DateTime>();
            string fileExtension = isBackup ? "*.cs" : "*.pdf";
            string[] files = Directory.GetFiles(targetDirectory, fileExtension);

            foreach (string file in files)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                   
                    string datePart = fileName.Split('_')[1];

                    DateTime restorePointDate = DateTime.ParseExact(datePart, "yyyyMMdd", null);
                    restorePointDates.Add(restorePointDate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            return restorePointDates;
        }



        private CalendarDayButton FindDayButton(DateTime date)
        {
            
            return FindDayButtonRecursive(reportCalendar, date);
        }

        private CalendarDayButton FindDayButtonRecursive(DependencyObject parent, DateTime date)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is CalendarDayButton dayButton && dayButton.DataContext is DateTime buttonDate && buttonDate.Date == date.Date)
                {
                    return dayButton;
                }

                var result = FindDayButtonRecursive(child, date);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        private void LoadSettings()
        {
            txtDefaultStartTime.Text = GetSettingValue("DefaultStartTime") ?? "09:00";
            txtDefaultEndTime.Text = GetSettingValue("DefaultEndTime") ?? "18:00";
            txtLunchBreakStartTime.Text = GetSettingValue("LunchBreakStartTime") ?? "12:00";
            txtDefaultLunchBreak.Text = GetSettingValue("DefaultLunchBreak") ?? "60";
            txtTrackingInterval.Text = GetSettingValue("TrackingInterval") ?? "15";
            txtIdleThreshold.Text = GetSettingValue("IdleThreshold") ?? "5";
            cmbNotificationPrinter.SelectedItem = GetSettingValue("NotificationPrinter");
            cmbExportFormat.SelectedIndex = GetSettingValueAsInt("ExportFormat", 0);
            txtDataDelimiter.Text = GetSettingValue("DataDelimiter") ?? ",";
            string fontSizeSetting = GetSettingValue("FontSize");
            if (int.TryParse(fontSizeSetting, out int fontSizeValue) && fontSizeValue >= 0 &&
                fontSizeValue < cmbFontSize.Items.Count)
            {
                cmbFontSize.SelectedIndex = fontSizeValue;
            }
            else
            {
                cmbFontSize.SelectedIndex = 1;
            }

            ChangeFontSize((cmbFontSize.SelectedItem as ComboBoxItem)?.Content.ToString());
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSetting("DefaultStartTime", txtDefaultStartTime.Text);
                SaveSetting("DefaultEndTime", txtDefaultEndTime.Text);
                SaveSetting("LunchBreakStartTime", txtLunchBreakStartTime.Text);
                SaveSetting("DefaultLunchBreak", txtDefaultLunchBreak.Text);
                SaveSetting("TrackingInterval", txtTrackingInterval.Text);
                SaveSetting("IdleThreshold", txtIdleThreshold.Text);
                SaveSetting("NotificationPrinter", cmbNotificationPrinter.SelectedItem as string);
                SaveSetting("ExportFormat", cmbExportFormat.SelectedIndex.ToString());
                SaveSetting("DataDelimiter", txtDataDelimiter.Text);
                SaveSetting("FontSize", cmbFontSize.SelectedIndex.ToString());
                context.SaveChanges();
                MessageBox.Show("Настройки успешно сохранены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении настроек: " + ex.Message);
            }
        }

        private void PopulatePrinters()
        {
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                cmbNotificationPrinter.Items.Add(printerName);
            }
        }

        private void PopulateTables()
        {
            foreach (var table in tableTranslations)
            {
                cmbTables.Items.Add(table.Value);
            }
        }

        private DataTable GetDataFromTable(string tableName)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = $"SELECT * FROM {tableName}";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения данных: {ex.Message}");
            }

            return dataTable;
        }

        public class UIState
        {

            public string TextBoxContent { get; set; }

        }

        private UIState GetCurrentUIState()
        {

            return new UIState
            {
                TextBoxContent = myTextBox.Text
            };

        }
        public void SaveUIState(UIState state)
        {
            string json = JsonConvert.SerializeObject(state);
            File.WriteAllText("uistate.json", json);
        }
        public UIState LoadUIState()
        {
            string json = File.ReadAllText("uistate.json");
            return JsonConvert.DeserializeObject<UIState>(json);
        }
        public void BackupProjectFiles()
        {
            string projectFolderPath = @"C:\Users\Андрей\Desktop\Лебединая песня\Программа\AIS";
            string backupFolderPath = @"C:\Users\Андрей\Desktop\Лебединая песня\Программа\AIS\РK";//НУЖНО ПРИДУМАТЬ КАК ДАВАТЬ ПОЛЬЗОВАТЕЛЮ ВЫБОР КУДА СОЗРАНЯТЬ РК

            foreach (string filePath in Directory.GetFiles(projectFolderPath))
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                string destFile = System.IO.Path.Combine(backupFolderPath, fileName);
                File.Copy(filePath, destFile, true);
            }
        }

        private void RestoreProjectFiles()
        {
            string backupFolderPath = @"C:\Users\Андрей\Desktop\Лебединая песня\Программа\AIS\РK";
            string projectFolderPath = @"C:\Users\Андрей\Desktop\Лебединая песня\Программа\AIS";

            foreach (string filePath in Directory.GetFiles(backupFolderPath))
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                string destFile = System.IO.Path.Combine(projectFolderPath, fileName);
                File.Copy(filePath, destFile, true);



            }
        }

        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {
            UIState currentState = GetCurrentUIState();
            SaveUIState(currentState);
            BackupProjectFiles();
            MessageBox.Show("Точка восстановления создана.");
        }
        private void btnRestoreData_Click(object sender, RoutedEventArgs e)
        {
            UIState savedState = LoadUIState();
            ApplyUIState(savedState);
            RestoreProjectFiles();
            MessageBox.Show("Состояние восстановлено. Перезапустите приложение.");
            RestartApplication();
        }

        private void ApplyUIState(UIState state)
        {
            myTextBox.Text = state.TextBoxContent;
        }

       
        private void myTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private void RestartApplication()
        {

            
           // Application.Current.Shutdown();
        }
        private List<DateTime> GetRestorePointDates()
        {
            string restorePointsDirectory = Path.Combine(GetReportsFolderPath(), "RestorePoints"); 

            if (!Directory.Exists(restorePointsDirectory))
            {
                return new List<DateTime>(); 
            }

            List<DateTime> restorePointDates = new List<DateTime>();
            string[] restorePointFiles = Directory.GetFiles(restorePointsDirectory, "*.bak"); 

            foreach (string file in restorePointFiles)
            {
                try
                {
                   
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string datePart = fileName.Split('_')[1];

                    DateTime restorePointDate = DateTime.Parse(datePart);
                    restorePointDates.Add(restorePointDate);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    
                }
            }

            return restorePointDates;
        }

        private string GetReportsFolderPath()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
           
            string reportsFolder = Path.Combine(appPath, "Отчёты");

            
            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder);
            }

            return reportsFolder;
        }

       

        

        private IEnumerable<object> FindVisualChildren<T>(Calendar reportCalendar)
        {
            throw new NotImplementedException();
        }

        private static void AddCellToHeader(PdfPTable table, string text, Font font = null)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font ?? FontFactory.GetFont("Arial", 12, Font.BOLD)));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);
        }


        private static void AddCellToTable(PdfPTable table, string text, Font font = null)
        {
            table.AddCell(new Phrase(text, font ?? FontFactory.GetFont("Arial", 12, Font.NORMAL)));
        }

        private void ExportDataToPDF()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    Document doc = new Document(iTextSharp.text.PageSize.A4);
                    PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    string tableName = cmbTables.SelectedItem?.ToString();

                    if (!string.IsNullOrEmpty(tableName))
                    {
                        DataTable tableData = GetDataFromTable(tableName);

                        PdfPTable pdfTable = new PdfPTable(tableData.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        foreach (DataColumn column in tableData.Columns)
                        {
                            AddCellToHeader(pdfTable, column.ColumnName);
                        }

                        foreach (DataRow row in tableData.Rows)
                        {
                            foreach (DataColumn column in tableData.Columns)
                            {
                                AddCellToTable(pdfTable, row[column]?.ToString() ?? "");
                            }
                        }

                        doc.Add(pdfTable);
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста выберете таблицу для экспорта.");
                    }

                    doc.Close();
                    MessageBox.Show("Данные были успешно экспортированны в pdf файл.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка экспорта: {ex.Message}");
                }
            }
        }


       

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedFontSize = (cmbFontSize.SelectedItem as ComboBoxItem).Content.ToString();
            ChangeFontSize(selectedFontSize);
            SaveSetting("FontSize", selectedFontSize);
        }

        private void ChangeFontSize(string fontSize)
        {
            double newFontSize = 12;
            switch (fontSize)
            {
                case "Маленький":
                    newFontSize = 10;
                    break;
                case "Средний":
                    newFontSize = 12;
                    break;
                case "Большой":
                    newFontSize = 14;
                    break;
            }

            Application.Current.Resources["ApplicationFontSize"] = newFontSize;
        }


        private string GetSettingValue(string settingName)
        {
            var setting = context.Settings.FirstOrDefault(s => s.SettingName == settingName);
            return setting != null ? setting.SettingValue : null;
        }

        private int GetSettingValueAsInt(string settingName, int defaultValue)
        {
            string settingValue = GetSettingValue(settingName);
            return int.TryParse(settingValue, out int value) ? value : defaultValue;
        }


        private void SaveSetting(string settingName, string settingValue)
        {
            var setting = context.Settings.FirstOrDefault(s => s.SettingName == settingName);
            if (setting == null)
            {
                setting = new Settings { SettingName = settingName, SettingValue = settingValue };
                context.Settings.Add(setting);
            }
            else
            {
                setting.SettingValue = settingValue;
            }
        }

        private void lstColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void cmbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstColumns.Items.Clear();
            string selectedTableRussian = cmbTables.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedTableRussian))
            {
                string selectedTableEnglish =
                    tableTranslations.FirstOrDefault(x => x.Value == selectedTableRussian).Key;

                if (!string.IsNullOrEmpty(selectedTableEnglish))
                {
                    PopulateColumns(selectedTableEnglish);
                }
            }
        }

        private void PopulateColumns(string tableName)
        {
            lstColumns.Items.Clear();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DataTable schemaTable = connection.GetSchema("Columns", new[] { null, null, tableName });

                    foreach (DataRow row in schemaTable.Rows)
                    {
                        string columnName = row["COLUMN_NAME"].ToString();

                        string translatedColumnName = columnTranslations.ContainsKey(columnName)
                            ? columnTranslations[columnName]
                            : columnName;

                        lstColumns.Items.Add(translatedColumnName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения данных: {ex.Message}");
            }
        }

        private void btnGeneratePDFReport_Click(object sender, RoutedEventArgs e)
        {
            string selectedTableName = cmbTables.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedTableName))
            {
                
                switch (selectedTableName)
                {
                    case "Отделы":
                        GenerateDepartmentReport();
                        break;
                    case "Сотрудники":
                        GenerateEmployeeReport();
                        break;
                    case "Должности":
                        GenerateJobTitleReport();
                        break;
                    case "Зарплата":
                        GeneratePayrollReport();
                        break;
                    case "Проекты":
                        GenerateProjectReport();
                        break;
                    case "Графики":
                        GenerateScheduleReport();
                        break;
                    case "Настройки":
                        GenerateSettingsReport();
                        break;
                    case "Задачи":
                        GenerateTaskReport();
                        break;
                    case "Записи времени":
                        GenerateTimeEntryReport();
                        break;
                    case "Запросы на отгул":
                        GenerateTimeOffRequestReport();
                        break;
                    case "Пользователи":
                        GenerateUserReport();
                        break;
                    case "Журнал аудита":
                        GenerateAuditLogReport();
                        break;
                    case "Посещаемость":
                        GenerateAttendanceReport();
                        break;
                    case "Разрешения":
                        GeneratePermissionReport();
                        break;
                    default:
                        MessageBox.Show("Ошибка создания отчета.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйстса выберете таблицу.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
           
            
        }

        private List<string> GetTableNames()
        {
            var tableNames = new List<string>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string tableName = reader["TABLE_NAME"].ToString();
                        if (tableTranslations.ContainsKey(tableName))
                        {
                            tableNames.Add(tableTranslations[tableName]);
                        }
                        else
                        {
                            tableNames.Add(tableName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка таблицы: {ex.Message}");
            }
            return tableNames;
        }
        private string GetEncryptedReportsFolderPath()
        {
            
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            string encryptedReportsFolder = Path.Combine(appPath, "Отчёты");

            return encryptedReportsFolder;
        }

        private void GenerateDepartmentReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string tempFilePath = Path.GetTempFileName();
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return; 
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                       Encoding.UTF8.GetBytes(userPassword), 
                       Encoding.UTF8.GetBytes(userPassword), 
                       PdfWriter.AllowPrinting | PdfWriter.AllowCopy, 
                       PdfWriter.ENCRYPTION_AES_128 
                         );

                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по отделам", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID отдела", font);
                    AddCellToHeader(table, "Название отдела", font);

                    var departments = context.Departments.ToList();
                    foreach (var department in departments)
                    {
                        AddCellToTable(table, department.DepartmentID.ToString(), font);
                        AddCellToTable(table, department.DepartmentName, font);
                    }


                    document.Add(table);
                   

                   
                  

                    document.Close();



                    MessageBox.Show($"Отчет по отделам создан успешно: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по отделам: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private string GetPasswordFromUser()
        {
            string password;
            do
            {
                password = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите пароль для отчета (минимум 6 символов, включая буквы, цифры и спецсимволы /, #, *):",
                    "Защита паролем",
                    "",
                    -1, -1);//координаты - сейчас по центру экрана

                if (string.IsNullOrEmpty(password))
                {
                    return null; 
                }

            } while (!IsValidPassword(password));

            return password;
        }

        private bool IsValidPassword(string password)
        {
            if (password.Length < 6) return false;

            bool hasLetter = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasDigit = true;
                if ("!@#$%^&*()_+=-`~[]\\{}|;':\",./<>?".Contains(c)) hasSpecialChar = true;
            }

            return hasLetter && hasDigit && hasSpecialChar;
        }

        private void GenerateEmployeeReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }

                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по сотрудникам", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID сотрудника", font);
                    AddCellToHeader(table, "Имя", font);
                    AddCellToHeader(table, "Фамилия", font);
                    AddCellToHeader(table, "Дата найма", font);
                    AddCellToHeader(table, "Контактная информация", font);
                    AddCellToHeader(table, "ID должности", font);
                    AddCellToHeader(table, "ID отдела", font);


                    var employees = context.Employees.ToList();
                    foreach (var employee in employees)
                    {
                        AddCellToTable(table, employee.EmployeeID.ToString(), font);
                        AddCellToTable(table, employee.FirstName, font);
                        AddCellToTable(table, employee.LastName, font);
                        AddCellToTable(table, employee.HireDate.ToShortDateString(), font);
                        AddCellToTable(table, employee.ContactInfo, font);
                        AddCellToTable(table, employee.JobTitleID.ToString(), font);
                        AddCellToTable(table, employee.DepartmentID.ToString(), font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет  создан успешно: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по сотрудникам: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void GenerateJobTitleReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по должностям", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID должности", font);
                    AddCellToHeader(table, "Название должности", font);

                    var jobTitles = context.JobTitles.ToList();
                    foreach (var jobTitle in jobTitles)
                    {
                        AddCellToTable(table, jobTitle.JobTitleID.ToString(), font);
                        AddCellToTable(table, jobTitle.JobTitle, font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохраннено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка создания отчета: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по должностям: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GeneratePayrollReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по зарплате", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID", font);
                    AddCellToHeader(table, "ID сотрудника", font);
                    AddCellToHeader(table, "Начало периода", font);
                    AddCellToHeader(table, "Конец периода", font);
                    AddCellToHeader(table, "Валовая зарплата", font);
                    AddCellToHeader(table, "Чистая зарплата", font);
                    AddCellToHeader(table, "Налоги", font);

                    var payrolls = context.Payroll.ToList();
                    foreach (var payroll in payrolls)
                    {
                        AddCellToTable(table, payroll.PayrollID.ToString(), font);
                        AddCellToTable(table, payroll.EmployeeID.ToString(), font);
                        AddCellToTable(table, payroll.PayPeriodStart.ToShortDateString(), font);
                        AddCellToTable(table, payroll.PayPeriodEnd.ToShortDateString(), font);
                        AddCellToTable(table, payroll.GrossPay.ToString(), font);
                        AddCellToTable(table, payroll.NetPay.ToString(), font);
                        AddCellToTable(table, payroll.Taxes.ToString(), font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка создания отчета: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета о зарплате: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateProjectReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по проектам", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID проекта", font);
                    AddCellToHeader(table, "Название проекта", font);
                    AddCellToHeader(table, "Дата начала", font);
                    AddCellToHeader(table, "Дата окончания", font);
                    AddCellToHeader(table, "Статус", font);

                    var projects = context.Projects.ToList();
                    foreach (var project in projects)
                    {
                        AddCellToTable(table, project.ProjectID.ToString(), font);
                        AddCellToTable(table, project.ProjectName, font);
                     
                        AddCellToTable(table, project.EndDate.HasValue ? project.EndDate.Value.ToShortDateString() : "", font);
                      
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка создания отчета: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по проектам: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateScheduleReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по графикам", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(6);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID", font);
                    AddCellToHeader(table, "ID сотрудника", font);
                    AddCellToHeader(table, "Начало смены", font);
                    AddCellToHeader(table, "Конец смены", font);
                    AddCellToHeader(table, "День недели", font);
                    AddCellToHeader(table, "Тип графика", font);

                    var schedules = context.Schedules.ToList();
                    foreach (var schedule in schedules)
                    {
                        AddCellToTable(table, schedule.ScheduleID.ToString(), font);
                        AddCellToTable(table, schedule.EmployeeID.ToString(), font);
                        AddCellToTable(table, schedule.ShiftStart.ToString(), font);
                        AddCellToTable(table, schedule.ShiftEnd.ToString(), font);
                        AddCellToTable(table, schedule.DayOfWeek.ToString(), font);
                        AddCellToTable(table, schedule.ScheduleType, font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateSettingsReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по настройкам", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID", font);
                    AddCellToHeader(table, "Название настройки", font);
                    AddCellToHeader(table, "Значение", font);

                    var settings = context.Settings.ToList();
                    foreach (var setting in settings)
                    {
                        AddCellToTable(table, setting.SettingID.ToString(), font);
                        AddCellToTable(table, setting.SettingName, font);
                        AddCellToTable(table, setting.SettingValue, font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка создания отчета: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по настройкам: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateTaskReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по задачам", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD, BaseColor.BLACK);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new Phrase("ID задачи", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Название задачи", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID проекта", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Статус", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Описание", font));
                    table.AddCell(cell);
                    var tasks = context.Tasks.ToList();
                    foreach (var task in tasks)
                    {
                        cell = new PdfPCell(new Phrase(task.TaskID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(task.TaskName, font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(task.ProjectID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(task.Status, font));
                        table.AddCell(cell);
                      
                    }

                    document.Add(table);
                    document.Close();
                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохраненно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка создания очтета: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по задачам: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateTimeEntryReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    Document document = new Document();
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();
                    iTextSharp.text.Paragraph title =
                        new iTextSharp.text.Paragraph("Отчет по записям рабочего времени", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new Phrase("ID записи", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID сотрудника", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Время входа", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Время выхода", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID проекта", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID задачи", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Одобрено", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Комментарий", font));
                    table.AddCell(cell);
                    var timeEntries = context.TimeEntries.ToList();
                    foreach (var entry in timeEntries)
                    {
                        cell = new PdfPCell(new Phrase(entry.TimeEntryID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.EmployeeID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.ClockInTime.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.ClockOutTime.HasValue
                            ? entry.ClockOutTime.Value.ToString()
                            : "", font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.ProjectID.HasValue
                            ? entry.ProjectID.Value.ToString()
                            : "", font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.TaskID.HasValue ? entry.TaskID.Value.ToString() : "",
                            font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.Approved.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(entry.Note, font));
                        table.AddCell(cell);
                    }

                    document.Add(table);
                    document.Close();
                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по времени: {ex.Message}", "Отчет",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateTimeOffRequestReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();
                    iTextSharp.text.Paragraph title =
                        new iTextSharp.text.Paragraph("Отчет по заявкам на отпуск", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new Phrase("ID заявки", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID сотрудника", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Дата начала", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Дата окончания", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Причина", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Статус", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Одобрено кем", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Комментарий", font));
                    table.AddCell(cell);
                    var requests = context.TimeOffRequests.ToList();
                    foreach (var request in requests)
                    {
                        cell = new PdfPCell(new Phrase(request.RequestID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(request.EmployeeID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(request.StartDate.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(request.EndDate.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(request.Reason, font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(request.Status, font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(request.ApprovedBy.HasValue
                            ? request.ApprovedBy.Value.ToString()
                            : "", font));
                        table.AddCell(cell);
                       
                        table.AddCell(cell);
                    }

                    document.Add(table);
                    document.Close();
                    MessageBox.Show(
                        $"Отчет сохранен: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка сщздания отчета по отпускам: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateUserReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();
                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по пользователям", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new Phrase("ID пользователя", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Имя пользователя", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID роли", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("ID сотрудника", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Права", font));
                    table.AddCell(cell);
                    var users = context.Users.ToList();
                    foreach (var user in users)
                    {
                        cell = new PdfPCell(new Phrase(user.UserID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(user.Username, font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(user.RoleID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(user.EmployeeID.HasValue
                            ? user.EmployeeID.Value.ToString()
                            : "", font));
                        table.AddCell(cell);
                        var permissions = string.Join(", ", user.Permissions.Select(p => p.PermissionName));
                        cell = new PdfPCell(new Phrase(permissions, font));
                        table.AddCell(cell);
                    }

                    document.Add(table);
                    document.Close();
                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета по пользователям: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GenerateAuditLogReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по журналу аудита", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID", font);
                    AddCellToHeader(table, "ID пользователя", font);
                    AddCellToHeader(table, "Действие", font);
                    AddCellToHeader(table, "Дата/время", font);
                    AddCellToHeader(table, "Детали", font);

                    var auditLogs = context.AuditLog.ToList();
                    foreach (var log in auditLogs)
                    {
                        AddCellToTable(table, log.LogID.ToString(), font);
                        AddCellToTable(table, log.UserID.ToString(), font);
                        AddCellToTable(table, log.Action, font);
                        AddCellToTable(table, log.Timestamp.ToString(), font);
                        AddCellToTable(table, log.Details, font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Созранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета аудита: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GenerateAttendanceReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("Отчет по посещаемости", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));

                    PdfPTable table = new PdfPTable(4);
                    table.WidthPercentage = 100;
                    AddCellToHeader(table, "ID", font);
                    AddCellToHeader(table, "ID сотрудника", font);
                    AddCellToHeader(table, "Дата", font);
                    AddCellToHeader(table, "Статус посещаемости", font);

                    var attendances = context.Attendance.ToList();
                    foreach (var attendance in attendances)
                    {
                        AddCellToTable(table, attendance.AttendanceID.ToString(), font);
                        AddCellToTable(table, attendance.EmployeeID.ToString(), font);
                        AddCellToTable(table, attendance.Date.ToShortDateString(), font);
                        AddCellToTable(table, attendance.AttendanceStatus, font);
                    }

                    document.Add(table);
                    document.Close();

                    MessageBox.Show($"Отчет создан: {saveFileDialog.FileName}!",
                        "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета Посещаемости : {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GeneratePermissionReport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            string encryptedReportsFolder = GetEncryptedReportsFolderPath();
            saveFileDialog.InitialDirectory = encryptedReportsFolder;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 10);
                    string userPassword = GetPasswordFromUser();
                    if (string.IsNullOrEmpty(userPassword))
                    {
                        return;
                    }
                    Document document = new Document();
                    PdfWriter writer =
                        PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    writer.SetEncryption(
                      Encoding.UTF8.GetBytes(userPassword),
                      Encoding.UTF8.GetBytes(userPassword),
                      PdfWriter.AllowPrinting | PdfWriter.AllowCopy,
                      PdfWriter.ENCRYPTION_AES_128
                        );
                    document.Open();
                    iTextSharp.text.Paragraph title =
                        new iTextSharp.text.Paragraph("Отчет по правам доступа", font);
                    title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    title.Font = FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLD);
                    document.Add(title);
                    document.Add(new iTextSharp.text.Paragraph(" "));
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    PdfPCell cell = new PdfPCell(new Phrase("ID права", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Название права", font));
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("Описание", font));
                    table.AddCell(cell);
                    var permissions = context.Permissions.ToList();
                    foreach (var permission in permissions)
                    {
                        cell = new PdfPCell(new Phrase(permission.PermissionID.ToString(), font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(permission.PermissionName, font));
                        table.AddCell(cell);
                        cell = new PdfPCell(new Phrase(permission.Description, font));
                        table.AddCell(cell);
                    }

                    document.Add(table);
                    document.Close();
                    MessageBox.Show(
                        $"Отчет создан: {saveFileDialog.FileName}!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    MessageBox.Show($"Ошибка создания отчета привелегий: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void lstColumns_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
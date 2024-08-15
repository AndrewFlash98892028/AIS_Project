using System;

namespace AIS.Employee 
{
    public class EmployeeOvertimeEventArgs : EventArgs
    {
        public int EmployeeId { get; private set; }
        public TimeSpan Overtime { get; private set; }

        public EmployeeOvertimeEventArgs(int employeeId, TimeSpan overtime)
        {
            EmployeeId = employeeId;
            Overtime = overtime;//переработка
        }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AIS.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class TimeEntries
    {
        public int TimeEntryID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime ClockInTime { get; set; }
        public Nullable<System.DateTime> ClockOutTime { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public bool Approved { get; set; }
        public string Note { get; set; }
        public Nullable<decimal> Overtime { get; set; }
    
        public virtual Employees Employees { get; set; }
        public virtual Projects Projects { get; set; }
        public virtual Tasks Tasks { get; set; }
    }
}

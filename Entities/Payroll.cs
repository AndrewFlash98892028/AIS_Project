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
    
    public partial class Payroll
    {
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime PayPeriodStart { get; set; }
        public System.DateTime PayPeriodEnd { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public decimal Deductions { get; set; }
        public decimal Taxes { get; set; }
    
        public virtual Employees Employees { get; set; }
    }
}

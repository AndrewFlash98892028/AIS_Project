using AIS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIS.Admin
{
    public class PayrollService
    {
        private readonly AISCEntities1 dbContext;
        private const decimal PersonalIncomeTaxRate = 0.13m;
        private const decimal SocialInsuranceRate = 0.22m; 
        private const decimal MedicalInsuranceRate = 0.051m; 

        public PayrollService(AISCEntities1 dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<Payroll> GeneratePayroll(DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            var payrollRecords = new List<Payroll>();

           
            var employees = dbContext.Employees.Where(e => e.IsActive).ToList();

            foreach (var employee in employees)
            {
               
                var timeEntries = dbContext.TimeEntries
                    .Where(t => t.EmployeeID == employee.EmployeeID &&
                                t.ClockInTime >= payPeriodStart &&
                                t.ClockInTime <= payPeriodEnd)
                    .ToList();

             
                var payrollRecord = CalculateEmployeePayroll(employee, timeEntries, payPeriodStart, payPeriodEnd);

               
                payrollRecords.Add(payrollRecord);
            }

            return payrollRecords;
        }

        private Payroll CalculateEmployeePayroll(Employees employee, List<TimeEntries> timeEntries,
                                                     DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            decimal hoursWorked = CalculateTotalHoursWorked(timeEntries);
            decimal grossPay = (decimal)(employee.HourlyRate * hoursWorked);

         
            decimal personalIncomeTax = grossPay * PersonalIncomeTaxRate;
            decimal socialInsurance = grossPay * SocialInsuranceRate;
            decimal medicalInsurance = grossPay * MedicalInsuranceRate;

            decimal totalTaxes = personalIncomeTax + socialInsurance + medicalInsurance;

           
            decimal deductions = 0; 

            decimal netPay = grossPay - totalTaxes - deductions;

            return new Payroll
            {
                EmployeeID = employee.EmployeeID,
                PayPeriodStart = payPeriodStart,
                PayPeriodEnd = payPeriodEnd,
                GrossPay = grossPay,
                NetPay = netPay,
                Deductions = deductions,
                Taxes = totalTaxes
            };
        }

        private decimal CalculateTotalHoursWorked(List<TimeEntries> timeEntries)
        {
            decimal totalHours = 0;
            foreach (var entry in timeEntries)
            {
                if (entry.ClockOutTime.HasValue)
                {
                    totalHours += (decimal)(entry.ClockOutTime.Value - entry.ClockInTime).TotalHours;
                }
            }
            return totalHours;
        }
    }
}
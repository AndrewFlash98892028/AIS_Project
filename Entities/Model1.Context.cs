﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AISCEntities1 : DbContext
    {
        public AISCEntities1()
            : base("name=AISCEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Attendance> Attendance { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<JobTitles> JobTitles { get; set; }
        public virtual DbSet<PasswordHashInfo> PasswordHashInfo { get; set; }
        public virtual DbSet<Payroll> Payroll { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Schedules> Schedules { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<TimeEntries> TimeEntries { get; set; }
        public virtual DbSet<TimeOffRequests> TimeOffRequests { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}

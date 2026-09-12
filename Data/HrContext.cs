using Mhrm.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Mhrm.Data
{
    public class HrDbContext : DbContext
    {
        public HrDbContext(DbContextOptions<HrDbContext> options)
            : base(options)
        {
        }

        public DbSet<News> News { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<DepartmentPosition> DepartmentPosition { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  RolePermissions (Many-to-Many)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // DepartmentPosition (Many-to-Many)
            modelBuilder.Entity<DepartmentPosition>()
                .HasOne(dp => dp.Department)
                .WithMany(d => d.DepartmentPosition)
                .HasForeignKey(dp => dp.DepartmentId);

            modelBuilder.Entity<DepartmentPosition>()
                .HasOne(dp => dp.Position)
                .WithMany(p => p.DepartmentPosition)
                .HasForeignKey(dp => dp.PositionId);

            //  User ↔ Employee (1 to 1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            //  UserSession ↔ User (1 to many)
            modelBuilder.Entity<UserSession>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Contract ↔ Employee (many to 1)
            modelBuilder.Entity<Contract>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Document ↔ Employee
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            
            //  Request ↔ Employee (RequestBy)
            modelBuilder.Entity<Request>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(r => r.RequestBy)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Request>()
                .HasOne<Employee>()
                .WithMany()
                .HasForeignKey(r => r.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            

            //  Data 
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "اداری", Description = "رسیدگی به وظایف تعیین شده مدیریت" },
                new Department { Id = 2, Name = "مدیریت", Description = "مدیر و معاونان" },
                new Department { Id = 3, Name = "تولید", Description = "کارگران خط تولید" }
            );

            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Title = "کارشناس", Description = "نظارت بر بخش زیر نظر مدیر" },
                new Position { Id = 2, Title = "مدیر", Description = "نظارت بر بخش" },
                new Position { Id = 3, Title = "اپراتور", Description = "کارگر" }
            );

            modelBuilder.Entity<DepartmentPosition>().HasData(
                new DepartmentPosition { Id = 1, DepartmentId = 1, PositionId = 1 },
                new DepartmentPosition { Id = 2, DepartmentId = 1, PositionId = 2 },
                new DepartmentPosition { Id = 3, DepartmentId = 2, PositionId = 1 },
                new DepartmentPosition { Id = 4, DepartmentId = 2, PositionId = 2 },
                new DepartmentPosition { Id = 5, DepartmentId = 3, PositionId = 1 },
                new DepartmentPosition { Id = 6, DepartmentId = 3, PositionId = 2 },
                new DepartmentPosition { Id = 7, DepartmentId = 3, PositionId = 3 }

                
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin*" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "MHR" },
                new Role { Id = 4, Name = "Employee" },
                new Role { Id = 5, Name = "JustSeeing" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Permission_Key = "Add and delete", Descriptions = "Create / Delete User" },
                new Permission { Id = 2, Permission_Key = "Edit", Descriptions = "Edit User" },
                new Permission { Id = 3, Permission_Key = "Making changes", Descriptions = " Making changes to information by the employee herself" }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 1, PermissionId = 2 },
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 4, PermissionId = 3 }
                   
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    National_id = "1111111111",
                    FirstName = "System",
                    LastName = "Admin",
                    BirthDate = new DateTime(1990, 1, 1),
                    Phone = "09000000000",
                    Email = "admin@hrm.com",
                    HireDate = DateTime.Now,
                    Status = 1,
                    InsuranceNumber = "INS-001",
                    DepartmentPositionId = 1
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    EmployeeId = 1,
                    Username = "admin",
                    Password = "123",
                    RoleId = 1,
                    Bio = "System Administrator"
                }
            );

            modelBuilder.Entity<News>().HasData(
                new News
                {
                    Id = 1,
                    FilePath = "/images/LOGO.jpg",
                    PublishedBy = 1,
                    Title = "خوش امدید",
                    Content = "به سامانه مدیریت منابع انسانی خوش آمدید. این سامانه با هدف تسهیل فرآیندهای اداری، دسترسی سریع به اطلاعات کارکنان، ثبت درخواست‌ها، مشاهده اطلاعیه‌ها و بهبود ارتباطات سازمانی طراحی شده است. امیدواریم این سامانه تجربه‌ای ساده، سریع و کارآمد را برای شما فراهم کند. از همراهی و همکاری شما سپاسگزاریم و برایتان موفقیت و پیشرفت روزافزون آرزو داریم",
                    CreatedAt = new DateTime(1, 1, 1),
                    PublishedAt = new DateTime(1, 1, 1),
                    IsActive = true ,
                    IsImportant = false ,
                    Category = 0 ,
                }
            );
        }
    }
}

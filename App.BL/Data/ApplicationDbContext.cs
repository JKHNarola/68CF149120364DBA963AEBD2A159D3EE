using App.BL.Data.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace App.BL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public virtual DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<ExtendedLog> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<ApplicationUserRole>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            //For Log Table
            // build default model.
            LogModelBuilderHelper.Build(builder.Entity<ExtendedLog>());

            // real relation database can map table:
            builder.Entity<ExtendedLog>().ToTable("Logs");

            builder.Entity<ExtendedLog>().Property(r => r.Id).ValueGeneratedOnAdd();

            builder.Entity<ExtendedLog>().HasIndex(r => r.TimeStamp).HasName("IX_Log_TimeStamp");
            builder.Entity<ExtendedLog>().HasIndex(r => r.EventId).HasName("IX_Log_EventId");
            builder.Entity<ExtendedLog>().HasIndex(r => r.Level).HasName("IX_Log_Level");

            builder.Entity<ExtendedLog>().Property(u => u.Name).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.Browser).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.UserId);
            builder.Entity<ExtendedLog>().Property(u => u.ReqIp).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.ReqPath).HasMaxLength(255);
            builder.Entity<ExtendedLog>().Property(u => u.ReqHeaders).HasMaxLength(5000);
            builder.Entity<ExtendedLog>().Property(u => u.ReqMethod).HasMaxLength(50);
            builder.Entity<ExtendedLog>().Property(u => u.ReqPayload).HasMaxLength(5000);
        }
    }

    public class ContextInitializer
    {
        private ApplicationDbContext _context;

        public ContextInitializer(ApplicationDbContext context)
        {
            _context = context;
        }

        //Seed method
        public async Task InitDb(bool isAutoMigrationOn = true)
        {
            var datetimeFormat = "dd MMM yyyy hh:mm:ss:fff tt";
            var currDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var folderpath = Path.Combine(currDir, "Logs");
            var file = Path.Combine(folderpath, "dbInitLog.txt");
            var logs = new List<string>();

            try { Directory.CreateDirectory(folderpath); } catch { }

            try
            {
                logs.Add("[Info] " + DateTime.Now.ToString(datetimeFormat) + "    " + "Is automigration on? " + isAutoMigrationOn.ToString());
                if (isAutoMigrationOn)
                {
                    logs.Add("[Info] " + DateTime.Now.ToString(datetimeFormat) + "    " + "Applying database migrations");

                    await _context.Database.MigrateAsync();
                }

                logs.Add("[Info] " + DateTime.Now.ToString(datetimeFormat) + "    " + "Verifying ApplicationRoles");
                if (_context.ApplicationRoles.AsNoTracking().Count() == 0)
                {
                    var roles = new List<ApplicationRole>()
                    {
                        new ApplicationRole()
                        {
                            Name = "Admin",
                            NormalizedName = "ADMIN",
                            DisplayName = "Admin",
                            Id = "1"
                        },
                        new ApplicationRole()
                        {
                            Name = "WebUser",
                            NormalizedName = "WEBUSER",
                            DisplayName = "Web User",
                            Id = "2"
                        },
                        new ApplicationRole()
                        {
                            Name = "System",
                            NormalizedName = "SYSTEM",
                            DisplayName = "System",
                            Id = "3"
                        },
                        new ApplicationRole()
                        {
                            Name = "Dev",
                            NormalizedName = "DEV",
                            DisplayName = "Developer",
                            Id = "4"
                        },
                    };
                    _context.ApplicationRoles.AddRange(roles);

                    await _context.SaveChangesAsync();
                }

                logs.Add("[Info] " + DateTime.Now.ToString(datetimeFormat) + "    " + "Verifying System user");
                var sysUser = _context.ApplicationUsers.AsNoTracking().FirstOrDefault(x => x.NormalizedEmail == "SYSTEMUSER@DEMO.EMAIL");
                if (sysUser == null)
                {

                    sysUser = new ApplicationUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = "systemuser@demo.email",
                        EmailConfirmed = true,
                        NormalizedEmail = "SYSTEMUSER@DEMO.EMAIL",
                        FirstName = "System",
                        LastName = "User",
                        UserName = "sysuser",
                        NormalizedUserName = "SYSUSER",
                        SecurityStamp = Guid.NewGuid().ToString(),
                        LockoutEnabled = true
                    };

                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(sysUser, "#Q]hgnP7qYjG9]E_");
                    sysUser.PasswordHash = hashed;

                    _context.ApplicationUsers.Add(sysUser);

                    //TODO: Add user to role

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                logs.Add("[ERROR] " + DateTime.Now.ToString(datetimeFormat) + "   " + ex.ToString());
            }


            logs.Add(Environment.NewLine);

            try { await File.AppendAllLinesAsync(file, logs); } catch { }
        }
    }
}

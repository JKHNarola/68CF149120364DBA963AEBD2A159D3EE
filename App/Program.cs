using App.BAL.Data;
using App.BAL.Data.DTO;
using App.BL;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    var isDbLogging = Convert.ToBoolean(hostingContext.Configuration["IsDbLogging"]);
                    if (isDbLogging)
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddEntityFramework<ApplicationDbContext, ExtendedLog>();
                    }
                    else
                    {
                        logging.AddFile(options =>
                        {
                            options.FileName = AppCommon.AppName + "-logs-";
                            options.LogDirectory = "Logs";
                            options.FileSizeLimit = 20 * 1024 * 1024;
                            options.RetainedFileCountLimit = 20;
                        });
                    }
                })
                .UseStartup<Startup>();
        }

    }
}

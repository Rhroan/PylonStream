using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using PylonStream.Hubs;

namespace PylonStream
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Prevent multiple instances (optional but good practice)
            string processName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 1)
            {
                MessageBox.Show("The application is already running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ApplicationConfiguration.Initialize();

            // 1. Initialize exception captions
            InitialExceptionCaption();

            // 2. Load settings
            Modules.Config = AppConfig.Load();
            Modules.SystemLogger.AddLog("System settings loaded.", "INFO");

            // 3. Initialize Kestrel Web Server in background
            InitialWeb();

            // 4. Run the WinForms UI main loop
            Application.Run(new MainForm());

            // 5. Clean shutdown when WinForms closes
            ShutdownAll();
        }

        private static void InitialWeb()
        {
            var webThread = new Thread(() =>
            {
                try
                {
                    int port = Modules.Config.WebApiPort;
                    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
                    {
                        ContentRootPath = AppContext.BaseDirectory
                    });

                    // Add ASP.NET Core services
                    builder.Services.AddSignalR();
                    builder.Services.AddControllers();

                    var app = builder.Build();
                    Modules.WebApp = app;

                    // Enable serving default files (index.html) and static files from wwwroot
                    app.UseDefaultFiles();
                    app.UseStaticFiles();

                    // Setup HTTP request pipeline
                    app.MapHub<StreamHub>("/streamHub");
                    app.MapControllers();

                    // Save the SignalR HubContext for global log broadcasting
                    Modules.WebHubContext = app.Services.GetRequiredService<IHubContext<StreamHub>>();

                    Modules.SystemLogger.AddLog($"Web Server starting on http://0.0.0.0:{port}");
                    app.Run($"http://0.0.0.0:{port}");
                }
                catch (Exception ex)
                {
                    Modules.SystemLogger.AddLog($"Web Server failed to start: {ex.Message}", "ERROR");
                }
            });
            webThread.IsBackground = true;
            webThread.Start();
        }

        private static void ShutdownAll()
        {
            try
            {
                Modules.SystemLogger.AddLog("Shutting down all services...", "INFO");
                Modules.CameraMgr.StopAll();
                Modules.MediaMTXMgr.Stop();

                // Elegant stop ASP.NET Core Kestrel Web server to release port 5000 immediately
                if (Modules.WebApp != null)
                {
                    Modules.SystemLogger.AddLog("Stopping Web Server...", "INFO");
                    Modules.WebApp.StopAsync().GetAwaiter().GetResult();
                }

                Modules.SystemLogger.AddLog("Shutdown completed.", "INFO");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during shutdown: {ex.Message}");
            }
        }

        private static void InitialExceptionCaption()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                {
                    Modules.SystemLogger?.AddLog($"[UnhandledException] {ex.Message}\r\n{ex.StackTrace}", "ERROR");
                }
            };

            Application.ThreadException += (s, e) =>
            {
                Exception ex = e.Exception;
                Modules.SystemLogger?.AddLog($"[ThreadException] {ex.Message}\r\n{ex.StackTrace}", "ERROR");
            };
        }
    }
}
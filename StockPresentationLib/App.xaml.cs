using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StockLib.Abstraction;
using StockLib.Main.Agents.Analyst;
using StockPersistanceLib.Data;
using StockPersistanceLib.Repositories;
using StockPresentationLib.ViewModel;
using StockPresentationLib.Views;
using StockValuationApp.Entities.Stocks;
using StockValuationApp.Main.Uri;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace StockPresentationLib
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Debug.WriteLine("[App] OnStartup called");

            var services = new ServiceCollection();

            var appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StockValuationApp");
            Directory.CreateDirectory(appDataFolder);

            var dbPath = Path.Combine(appDataFolder, "StockValuation.db");
            var legacyDbPath = Path.Combine(AppContext.BaseDirectory, "StockValuation.db");
            if (!File.Exists(dbPath) && File.Exists(legacyDbPath))
            {
                File.Copy(legacyDbPath, dbPath, overwrite: true);
                Debug.WriteLine($"[App] Migrated existing database from {legacyDbPath} to {dbPath}");
            }

            Debug.WriteLine($"[App] Database path: {dbPath}");

            var connectionString = $"Data Source={dbPath};";
            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseSqlite(connectionString));


            services.AddSingleton<UriFinanceManager>();
            services.AddSingleton<IStockRepository, EfStockRepository>();
            services.AddSingleton<StockManager>();
            services.AddSingleton<StockAnalysisOverviewAgent>();

            services.AddTransient<NavigationVM>();
            services.AddTransient<HomeVM>();
            services.AddTransient<EarningsVM>();
            services.AddTransient<ReturnsVM>();
            services.AddTransient<CriteriasVM>();
            services.AddTransient<AnalysisVM>();
            services.AddTransient<MainWindow>();

            _serviceProvider = services.BuildServiceProvider();

            var dbContextFactory = _serviceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using (var ctx = dbContextFactory.CreateDbContext())
            {
                ctx.Database.Migrate();
                Debug.WriteLine("[App] Database has been reviewed and updated to the last migration.");
            }

            var stockManager = _serviceProvider.GetRequiredService<StockManager>();
            Debug.WriteLine("[App] Loading stocks from database...");
            stockManager.LoadAllAsync().GetAwaiter().GetResult();
            Debug.WriteLine($"[App] Loaded {stockManager.Count()} stocks");

            var main = _serviceProvider.GetRequiredService<MainWindow>();
            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Debug.WriteLine("[App] OnExit called - saving data...");
            if (_serviceProvider is not null)
            {
                var stockManager = _serviceProvider.GetService<StockManager>();
                if (stockManager != null)
                {
                    try
                    {
                        Debug.WriteLine($"[App] Saving {stockManager.Count()} stocks to database...");

                        // Task.Run prevents UI SynchronizationContext deadlocks during shutdown
                        Task.Run(async () => await stockManager.SaveAllAsync())
                            .GetAwaiter()
                            .GetResult();

                        Debug.WriteLine("[App] Save completed");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[App Error] Exception during shutdown save: {ex.Message}");
                    }
                }
            }
            base.OnExit(e);
        }
    }
}

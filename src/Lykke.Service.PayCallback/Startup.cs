using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Common.Log;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Common.Log;
using Lykke.Logs;

// ReSharper disable once RedundantUsingDirective
using Lykke.MonitoringServiceApiCaller;

using Lykke.Service.PayCallback.Core.Services;
using Lykke.Service.PayCallback.Core.Settings;
using Lykke.Service.PayCallback.Modules;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.PayCallback
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }
        private ILog _log;
        private IHealthNotifier _healthNotifier;

        // ReSharper disable once NotAccessedField.Local
        private string _monitoringServiceUrl;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver =
                            new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    });

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration("v1", "PayCallback API");
                });

                var builder = new ContainerBuilder();
                var appSettings = Configuration.LoadSettings<AppSettings>();
                _monitoringServiceUrl = appSettings.CurrentValue.MonitoringServiceClient?.MonitoringServiceUrl;

                services.AddLykkeLogging
                (
                    appSettings.ConnectionString(x => x.PayCallbackService.Db.LogsConnString),
                    "PayCallbackLog",
                    appSettings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                    appSettings.CurrentValue.SlackNotifications.AzureQueue.QueueName
                );

                builder.RegisterModule(new ServiceModule(appSettings.Nested(x => x.PayCallbackService)));

                builder.Populate(services);

                ApplicationContainer = builder.Build();

                _log = ApplicationContainer.Resolve<ILogFactory>().CreateLog(this);
                _healthNotifier = ApplicationContainer.Resolve<IHealthNotifier>();

                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfiles(typeof(Models.AutoMapperProfile));
                    cfg.AddProfiles(typeof(AzureRepositories.AutoMapperProfile));
                    cfg.AddProfiles(typeof(Services.AutoMapperProfile));
                });

                Mapper.AssertConfigurationIsValid();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                _log?.Error(ex);
                throw;
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseLykkeForwardedHeaders();
                app.UseLykkeMiddleware(ex => new { Message = "Technical problem" });

                app.UseMvc();
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
                });
                app.UseSwaggerUI(x =>
                {
                    x.RoutePrefix = "swagger/ui";
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
                app.UseStaticFiles();

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                _log?.Critical(ex);
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                // NOTE: Service not yet recieve and process requests here

                await ApplicationContainer.Resolve<IStartupManager>().StartAsync();

                _healthNotifier.Notify("Started", $"Env: {Program.EnvInfo}");
#if !DEBUG
                if (!string.IsNullOrEmpty(_monitoringServiceUrl))
                    await Configuration.RegisterInMonitoringServiceAsync(_monitoringServiceUrl, _healthNotifier);
#endif
            }
            catch (Exception ex)
            {
                _log.Critical(ex);
                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                // NOTE: Service still can recieve and process requests here, so take care about it if you add logic here.

                await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.Critical(ex);
                }
                throw;
            }
        }

        private Task CleanUp()
        {
            try
            {
                // NOTE: Service can't recieve and process requests here, so you can destroy all resources

                if (_healthNotifier != null)
                {
                    _healthNotifier.Notify("Terminating", $"Env: {Program.EnvInfo}");
                }

                ApplicationContainer.Dispose();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                if (_log != null)
                {
                    _log.Critical(ex);
                    (_log as IDisposable)?.Dispose();
                }
                throw;
            }
        }
    }
}

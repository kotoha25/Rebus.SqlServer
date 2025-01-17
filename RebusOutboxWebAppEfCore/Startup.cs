﻿using System;
using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rebus.Config;
using Rebus.Config.Outbox;
using Rebus.Routing.TypeBased;
using Rebus.SqlServer;
using Rebus.Transport;
using Rebus.Transport.InMem;
using RebusOutboxWebAppEfCore.Entities;
using RebusOutboxWebAppEfCore.Handlers;
using RebusOutboxWebAppEfCore.Messages;
using IDbConnection = Rebus.SqlServer.IDbConnection;

namespace RebusOutboxWebAppEfCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            static IDbConnection GetDbConnection(ITransactionContext transactionContext, IServiceProvider provider)
            {
                //var http = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;

                var scope = provider.CreateScope();
                transactionContext.OnDisposed(_ => scope.Dispose());
                var context = scope.ServiceProvider.GetRequiredService<WebAppDbContext>();
                var connection = context.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    context.Database.OpenConnection();
                }

                var transaction = context.Database.CurrentTransaction?.GetDbTransaction();

                transactionContext.OnDisposed(_ => connection.Dispose());

                return new DbConnectionWrapper((SqlConnection)connection, (SqlTransaction)transaction, managedExternally: false);
            }

            services.AddRebus(
                (configure, provider) => configure
                    .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "outbox-test"))
                    .Outbox(o => o.UseSqlServer(tc => GetDbConnection(tc, provider), "Outbox"))
                    .Routing(r => r.TypeBased().Map<ProcessMessageCommand>("outbox-test"))
            );

            services.AddRebusHandler<ProcessMessageCommandHandler>();

            services.AddDbContext<WebAppDbContext>(options => options.UseSqlServer("server=.; database=rebusoutboxwebapp; trusted_connection=true"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

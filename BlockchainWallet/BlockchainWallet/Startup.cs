﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlockchainWallet.Data;
using BlockchainWallet.Data.Repos;
using BlockchainWallet.Services;

namespace BlockchainWallet
{
    using global::AutoMapper;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BlockchainDbContext>(options =>
                options.UseInMemoryDatabase());
            //options.UseSqlServer(Configuration.GetConnectionString("BlockchainDbConnection")));

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IHttpRequestService, HttpRequestService>();

            services.AddAutoMapper();
            services.AddMemoryCache();
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
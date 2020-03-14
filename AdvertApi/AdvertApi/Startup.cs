using AdvertApi.HealthChecks;
using AdvertApi.Interfaces;
using AdvertApi.Services;
using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;
using Amazon.Util;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvertApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorage>();
            services.AddControllers();
            services.AddHealthChecks()
                .AddCheck<StorageHealthCheck>("Dynamo db storage");
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Web Advert api",
                    Version = "Version 1",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Name",
                        Email = "test@mail.com"
                    }
                });
            });
        }

        public async Task Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseHealthChecks("/health");

            app.UseSwagger();
            app.UseSwaggerUI(action => {
                action.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Advert Api");
            });

            await RegisterToCloudMap();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task RegisterToCloudMap()
        {
            var instanceId = EC2InstanceMetadata.InstanceId;
            string serviceId = Configuration.GetValue<string>("ServiceId");

            if (!string.IsNullOrEmpty(instanceId))
            {
                var ipv4 = EC2InstanceMetadata.PrivateIpAddress;
                var client = new AmazonServiceDiscoveryClient(Amazon.RegionEndpoint.EUWest2);

                await client.RegisterInstanceAsync(new RegisterInstanceRequest
                {
                    ServiceId = serviceId,
                    InstanceId = instanceId,
                    Attributes = new Dictionary<string, string>() { {"AWS_INSTANCE_IPV4", ipv4 }, { "AWS_INSTANCE_PORT", "80" } }
                });
                
            }
        }
    }
}

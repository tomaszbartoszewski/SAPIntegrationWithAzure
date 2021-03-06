﻿using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.WebJobs.Host;

namespace OutboundStudent
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        public static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            ServiceBusConfiguration serviceBusConfiguration = new ServiceBusConfiguration { ConnectionString = SettingsHelper.GetSetting("Microsoft.ServiceBus.ConnectionString") };
            serviceBusConfiguration.MessageOptions.AutoRenewTimeout = TimeSpan.FromMinutes(5);

            config.UseServiceBus(serviceBusConfiguration);

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}

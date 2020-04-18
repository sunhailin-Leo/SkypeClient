﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skype.Client.CefSharp.OffScreen;

namespace Skype.Client.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection(); 
            services.AddLogging(logging =>
            {
                logging.AddDebug();
                logging.AddConsole();
            });

#if DEBUG
            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
#endif

            services.AddSingleton<SkypeCefOffScreenClient>();

            var serviceProvider = services.BuildServiceProvider();

            if (args.Length == 2)
            {
                Console.WriteLine("Creating new instance of client");
                var client = serviceProvider.GetService<SkypeCefOffScreenClient>();

                client.StatusChanged += OnAppOnStatusChanged;
                client.IncomingCall += (sender, eventArgs) => Console.WriteLine(eventArgs);
                client.CallStatusChanged += (sender, eventArgs) => Console.WriteLine(eventArgs);
                client.MessageReceived += (sender, eventArgs) => Console.WriteLine(eventArgs);

                Console.WriteLine("Starting authentication. This might take a few seconds.");

                client.Login(args[0], args[1]);

                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Parameters mismatch");
            }
        }

        private static void OnAppOnStatusChanged(object sender, StatusChangedEventArgs eventArgs)
        {
            if (eventArgs.New == AppStatus.Ready)
            {
                Console.WriteLine("Ready! :). You will see incoming messages and calls on this command line shell. Press any key to exit.");
            }
        }
    }
}

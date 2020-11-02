// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

// Source https://github.com/microsoftgraph/msgraph-training-aspnet-core/blob/master/demo/GraphTutorial/

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EctWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

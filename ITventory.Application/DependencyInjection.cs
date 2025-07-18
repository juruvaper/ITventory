﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ITventory.Shared.Abstractions.Commands;
using ITventory.Shared.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace ITventory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddCommands();

            return services;
        }
    }
}

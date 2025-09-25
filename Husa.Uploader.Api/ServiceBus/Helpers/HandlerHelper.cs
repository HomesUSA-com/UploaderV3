namespace Husa.Uploader.Api.ServiceBus.Helpers
{
    using System;
    using System.Collections.Generic;
    using Husa.Extensions.Authorization;
    using Husa.Extensions.Authorization.Models;
    using Husa.Extensions.ServiceBus.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.AspNetCore.HeaderPropagation;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Primitives;

    public static class HandlerHelper
    {
        public static UserContext GetUploaderUser(UploaderUserSettings userSettings) => new()
        {
            Email = userSettings.Email,
            Name = userSettings.Name,
            Id = userSettings.Id,
            IsMLSAdministrator = userSettings.MLSAdministrator,
            UserRole = userSettings.UserRole,
        };

        public static void ConfigureContext(Message message, UploaderUserSettings userSettings, IServiceScope scope)
        {
            var userProvider = scope.ServiceProvider.GetRequiredService<IUserProvider>();
            userProvider.SetCurrentUser(GetUploaderUser(userSettings));
            var configureTraceId = scope.ServiceProvider.GetRequiredService<IConfigureTraceId>();
            configureTraceId.SetTraceId(message.MessageId);
        }

        public static void ConfigureUserAgent(IServiceScope scope)
        {
            var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
            headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase)
                {
                    { "User-Agent", "background-service" },
                };
        }
    }
}

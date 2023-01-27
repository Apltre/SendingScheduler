using Microsoft.Extensions.Configuration;
using Sending.Core.Example.Consul;
using Winton.Extensions.Configuration.Consul;

namespace Sending.Core.Example.Consul
{
    public static class ConfigurationBuilderExtensions
    {
        internal static IConfigurationBuilder AddConsul(this IConfigurationBuilder builder)
        {
            var configuration = builder.Build();

            var token = configuration.GetValue<string>(ConsulSettings.Token);
            var consulSettings = new ConsulSettings()
            {
                Host = configuration.GetValue<string>(ConsulSettings.ConsulHost),
                Key = configuration.GetValue<string>(ConsulSettings.ConsulKey)
            };

            return builder.AddConsul(consulSettings.Key,
                o =>
                {
                    o.ConsulConfigurationOptions = cco =>
                    {
                        cco.Address = new Uri(consulSettings.Host);
                        cco.Token = token;
                    };
                    o.ReloadOnChange = true;
                });
        }

        internal static IConfiguration ValidateConsulConfig(this IConfiguration configuration)
        {
            var serviceConfiguration = configuration.Get<ConsulConfiguration>();
            var validator = new ConsulServiceConfigValidator();
            var result = validator.Validate(serviceConfiguration);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(f => f.ErrorMessage).ToList();
                throw new Exception(string.Join(' ', errors));
            }
            return configuration;
        }
        public static IConfigurationBuilder AddConsul(this IConfigurationBuilder configBuilder, IConfiguration configuration)
        {
            configBuilder.AddConsul();
            configuration.ValidateConsulConfig();
            return configBuilder;
        }
    }
}
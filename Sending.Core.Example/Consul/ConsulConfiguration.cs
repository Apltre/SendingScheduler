using FluentValidation;

namespace Sending.Core.Example.Consul
{
    public class ConsulConfiguration
    {
        public ConsulDbConfiguration? ConnectionStrings { get; set; }
        public ConsulRabbitConfiguration? Rabbit { get; set; }
        public ConsulServiceConfiguration? Service { get; set; }
    }

    public class ConsulServiceConfigValidator : AbstractValidator<ConsulConfiguration>
    {
        public ConsulServiceConfigValidator()
        {
            RuleFor(r => r.ConnectionStrings)
            .NotNull()
            .SetValidator(new ConsulDbConfigurationValidator())
            .WithMessage(r => $"Поле {nameof(r.ConnectionStrings)} не найдено.");

            RuleFor(r => r.Rabbit)
            .NotNull()
            .SetValidator(new ConsulRabbitConfigurationValidator())
            .WithMessage(r => $"Поле {nameof(r.Rabbit)} не найдено.");

            RuleFor(r => r.Service)
            .NotNull()
            .SetValidator(new ConsulServiceConfigurationValidator())
            .WithMessage(r => $"Поле {nameof(r.Service)} не найдено.");
        }
    }
}

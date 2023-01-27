using FluentValidation;

namespace Sending.Core.Example.Consul
{
    public class ConsulRabbitConfiguration
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int? Port { get; set; }
    }

    public class ConsulRabbitConfigurationValidator : AbstractValidator<ConsulRabbitConfiguration>
    {
        public ConsulRabbitConfigurationValidator()
        {
            RuleFor(r => r.HostName)
            .NotNull()
            .NotEmpty()
            .WithMessage(r => $"Поле {nameof(r.HostName)} не найдено.");

            RuleFor(r => r.UserName)
            .NotNull()
            .NotEmpty()
            .WithMessage(r => $"Поле {nameof(r.UserName)} не найдено.");

            RuleFor(r => r.Password)
            .NotNull()
            .NotEmpty()
            .WithMessage(r => $"Поле {nameof(r.Password)} не найдено.");

            RuleFor(r => r.Port)
            .NotNull()
            .WithMessage(r => $"Поле {nameof(r.Port)} не найдено.");
        }
    }
}

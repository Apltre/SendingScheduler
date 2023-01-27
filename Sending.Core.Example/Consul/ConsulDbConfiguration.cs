using FluentValidation;

namespace Sending.Core.Example.Consul
{
    public class ConsulDbConfiguration
    {
        public string? SendingDB { get; set; }
        public string? RedisMemoryCache { get; set; }
    }

    public class ConsulDbConfigurationValidator : AbstractValidator<ConsulDbConfiguration>
    {
        public ConsulDbConfigurationValidator()
        {
            RuleFor(r => r.SendingDB)
            .NotNull()
            .NotEmpty()
            .WithMessage(r => $"Поле {nameof(r.SendingDB)} не найдено.");

            RuleFor(r => r.RedisMemoryCache)
            .NotNull()
            .NotEmpty()
            .WithMessage(r => $"Поле {nameof(r.RedisMemoryCache)} не найдено.");
        }
    }
}

using FluentValidation;

namespace Sending.Core.Example.Consul
{
    public class ConsulServiceConfiguration
    {
        public int? Id { get; set; }
        public int? QueryBatchSize { get; set; }
        public int? MemoryCacheSlidingInMinutes { get; set; }
        public int? RetriesMaximum { get; set; }
    }

    public class ConsulServiceConfigurationValidator : AbstractValidator<ConsulServiceConfiguration>
    {
        public ConsulServiceConfigurationValidator()
        {
            RuleFor(r => r.Id)
            .NotNull()
            .WithMessage(r => $"Поле {nameof(r.Id)} не найдено.");

            RuleFor(r => r.QueryBatchSize)
            .NotNull()
            .WithMessage(r => $"Поле {nameof(r.QueryBatchSize)} не найдено.");

            RuleFor(r => r.MemoryCacheSlidingInMinutes)
            .NotNull()
            .WithMessage(r => $"Поле {nameof(r.MemoryCacheSlidingInMinutes)} не найдено.");

            RuleFor(r => r.RetriesMaximum)
            .NotNull()
            .WithMessage(r => $"Поле {nameof(r.RetriesMaximum)} не найдено.");
        }
    }
}

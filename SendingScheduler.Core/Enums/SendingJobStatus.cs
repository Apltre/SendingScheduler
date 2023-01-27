namespace SendingScheduler.Core.Enums;

public enum SendingJobStatus
{
    /// <summary>
    /// Успешно отправлен
    /// </summary>
    FinishedSuccessfully = 2,
    /// <summary>
    /// В данный момент обрабатывается
    /// </summary>
    BeingProcessed = 1,
    /// <summary>
    /// Ждёт выполнения
    /// </summary>
    Pending = 0,
    /// <summary>
    /// Закончился ошибкой, которую не возможно классифицировать, в том числе ошибкой в нашем коде
    /// </summary>
    InnerFail = -1,
    /// <summary>
    /// Закончился логической (не переотправляемой) ошибкой от системы, в которую отправляем
    /// и требует обработки
    /// </summary>
    FinishedWithLogicalReceiverError = -3,
    /// <summary>
    /// Закончился логической (не переотправляемой) ошибкой от системы, в которую отправляем
    /// и требует обработки
    /// </summary>
    FinishedWithTemporaryReceiverError = -4
}

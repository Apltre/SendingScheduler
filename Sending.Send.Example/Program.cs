using Sending.Core.Example.Consul;
using SendingScheduler.Send;
using Sending.Core.Example.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureSettingFiles();
builder.Configuration.AddConsul(builder.Configuration);
builder.Services.ConfigureSendingSendService(builder.Configuration);
builder.Logging.ConfigureLogging(builder.Configuration);
var app = builder.Build();

app.Run();

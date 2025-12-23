using Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<GlimpseWorker>();
builder.Services.AddSystemd();

var host = builder.Build();
host.Run();

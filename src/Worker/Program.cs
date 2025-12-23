using Worker;


string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string userConfigPath = Path.Combine(home, ".config", "glimpse", "glimpse-config.json");

var configuration = new ConfigurationBuilder()
    .AddJsonFile(userConfigPath, optional: true)
    .Build();

var glimpseConfig = configuration.Get<GlimpseConfig>()
    ?? throw new InvalidOperationException("Failed to load ~/.config/glimpse/glimpse-config.json. Make sure it exists!");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<GlimpseConfig>(glimpseConfig);
builder.Services.AddHostedService<GlimpseWorker>();
builder.Services.AddSystemd();

var host = builder.Build();
host.Run();

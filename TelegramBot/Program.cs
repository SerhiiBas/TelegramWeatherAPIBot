using Core.Abstractions;
using Core.GeocodingApi.GeocodingApiInterfaces;
using Core.GeocodingApi.GeocodingApiServices;
using Core.Servises;
using Core.WathersAPI.Interface;
using Core.WeathersAPI.WeatherApiService;
using Telegram.Bot;
using TelegramBotAPI.Filters;
using VM.GeocodingApiVM;
using VM.TelegramBot;
using VM.WeatherAPIVM;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);


var botAccessToken = builder.Configuration.GetSection("TelegramBot:BotAccessToken").Get<string>();

string geocodingApiKey = builder.Configuration.GetSection("APIDataKey:Geocoding").Get<string>();

builder.Services.AddHttpClient("tgclient").AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(botAccessToken, client));
builder.Services.AddHttpClient<IGeocodingApiService, GeocodingApiService>(client => client.DefaultRequestHeaders.Add("X-Api-Key", geocodingApiKey));
builder.Services.AddHttpClient<IWeatherAPIService, WeatherAPIService>();

builder.Services.Configure<GeocodingApiLink>(options => options.GeocodingLink= builder.Configuration.GetRequiredSection("APIHost:Geocoding").Get<string>());
builder.Services.Configure<WatherAPILink>(options => options.WatherLink = builder.Configuration.GetSection("APIHost").GetRequiredSection("openweathermap").Get<string>());
builder.Services.Configure<TelegramBotConfig>(options =>
{ options.Domain = builder.Configuration.GetSection("TelegramBot:Domain").Get<string>();
  options.BotAccessToken = botAccessToken;
});

builder.Services.AddTransient<ITelegramBotServise, TelegramBotServise>();

builder.Services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter)));

builder.Services.AddHostedService<InitTelegramServise>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();

app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute("tgwebhook", $"/bot/{botAccessToken}",
        new { controller = "WeatherForecast", action = "Post" });

    endpoints.MapControllers(); 
  });

app.Run();

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using VM.TelegramBot;

namespace Core.Servises
{
    public class InitTelegramServise : IHostedService 
    {
        private ITelegramBotClient _telegramBotClient;
        private readonly TelegramBotConfig _options;

        public InitTelegramServise(ITelegramBotClient telegramBotClient, IOptions<TelegramBotConfig> options)
        {
            _telegramBotClient = telegramBotClient;
            this._options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var domainConfig = _options.Domain;
            var accessToken = _options.BotAccessToken;

            return _telegramBotClient.SetWebhookAsync($"{domainConfig}/bot/{accessToken}", allowedUpdates: Enumerable.Empty<UpdateType>(), cancellationToken: cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _telegramBotClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}

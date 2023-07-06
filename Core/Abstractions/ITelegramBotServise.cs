using Telegram.Bot.Types;

namespace Core.Abstractions
{
    public interface ITelegramBotServise
    {
        Task HandelMassage(Update update, CancellationToken cancellationToken = default);
    }
}

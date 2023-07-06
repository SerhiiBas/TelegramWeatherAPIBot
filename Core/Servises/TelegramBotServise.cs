using Core.Abstractions;
using Core.GeocodingApi.GeocodingApiInterfaces;
using Core.WathersAPI.Interface;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Common.Exceptions;
using VM.GeocodingApiVM;
using System.Text;

namespace Core.Servises
{
    public class TelegramBotServise : ITelegramBotServise
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IWeatherAPIService _watherAPIService;
        private readonly IGeocodingApiService _geocodingApiService;

        public TelegramBotServise(ITelegramBotClient telegramClient, IWeatherAPIService watherAPIService, IGeocodingApiService geocodingApiService)
        {
            this._telegramClient = telegramClient;
            this._watherAPIService = watherAPIService;
            this._geocodingApiService = geocodingApiService;
        }

        public async Task HandelMassage(Update update, CancellationToken cancellationToken = default)
        {
            Contract.NotNull(update, nameof(update));
            if (update.Type == UpdateType.EditedMessage)
                return;
            if (update.Message.Type != MessageType.Text)
            {
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, "Я можу обробляти лише текстові запити", cancellationToken: cancellationToken);
                return;
            }

            if (update.Message.Text.Equals("/start"))
            {
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id
                , "Привіт! Я бот, який допоможе дізнатися погоду в будь-якому куточку світу.\n" +
                "Ви можете використовувати такі Шаблони:\n" +
                "/start - Вивиде на екран доступні опції\n." +
                "/d1 - Дізнайся погоду на наступний день.\n" +
                "/d7 - Дізнайся погоду на наступні сім днів.\n" +
                "/dh - Дізнайся як змінюватиметься погода щогодинно.\n" +
                "Або просто введи назву локації, де Вам, цікаво знати погоду, щоб дізнатися її на сьогодні." +
                "\n Наприклад:\n" +
                "Київ Або: Kiev\n" +
                "d1 Київ\nd7 Kiev\ndh Kiev", cancellationToken: cancellationToken);
                return;
            }

            try
            {
                List<GeocodingLocation> geodata;
                Contract.NotNull(update.Message.Text, nameof(update.Message.Text));

                string[] message = update.Message.Text.Split(' ');

                if (message[0].FirstOrDefault().Equals('/'))
                    geodata = await _geocodingApiService.GetLocationCity(message.Length >= 2 ? String.Join(" ", message[1..]) : "Kiev", cancellationToken: cancellationToken);
                else
                    geodata = await _geocodingApiService.GetLocationCity(String.Join(" ", message), cancellationToken: cancellationToken);

                if (geodata.Count == 0)
                {
                    await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, $"Друже, За такими даними: {update.Message.Text}. Я не зміг знайти локації. Сробуй ще раз!");

                    return;
                }

                if (geodata.FirstOrDefault().Country.Equals("Ru", StringComparison.OrdinalIgnoreCase))
                {
                    geodata.FirstOrDefault().Country = "Країна гній";
                }
                var weather = await _watherAPIService.GetWather(geodata?.FirstOrDefault(), cancellationToken);

                if (weather == null)
                {
                    await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, $"Вибач, нажаль я не зміг дістати погоду. =( ");

                    return;
                }

                switch (message[0])
                {
                    case "/d1":
                        await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id,
                            $"Погода в {geodata[0].Name}, {geodata[0].Country}, {geodata[0].State} станом на {weather.daily.time[1]}.\n" +
                            $"Максимальна температура: {weather?.daily?.temperature_2m_max[1]} {weather?.daily_units?.temperature_2m_min}\n" +
                            $"Мінімальна температура: {weather?.daily?.temperature_2m_min[1]} {weather?.daily_units?.temperature_2m_min}\n" +
                            $"Пориви вітру до {weather?.daily?.windspeed_10m_max[1]} {weather?.daily_units?.windspeed_10m_max}\n" +
                            $"Кількість опадів:\n" +
                            $"Дощ: {weather?.daily?.rain_sum[1]} {weather?.daily_units?.rain_sum}\n" +
                            $"Сніг: {weather?.daily?.snowfall_sum[1]} {weather?.daily_units?.snowfall_sum}");

                        return;
                    case "/d7":
                        string messageToTgd7 = $"Погода в {geodata[0].Name}, {geodata[0].Country}, {geodata[0].State}\n";

                        for (int i = 1; i < 8; i++)
                        {
                            messageToTgd7 += $"{weather.daily.time[i]}: Максимальна температура: {weather?.daily?.temperature_2m_max[i]} {weather?.daily_units?.temperature_2m_min} " +
                                $"Мінімальна температура: {weather?.daily?.temperature_2m_min[i]} {weather?.daily_units?.temperature_2m_min} " +
                                $"Пориви вітру до {weather?.daily?.windspeed_10m_max[i]} {weather?.daily_units?.windspeed_10m_max}\n" +
                                $"Кількість опадів: " +
                                $"Дощ: {weather?.daily?.rain_sum[i]} {weather?.daily_units?.rain_sum} " +
                                $"Сніг: {weather?.daily?.snowfall_sum[i]} {weather?.daily_units?.snowfall_sum}\n";
                        }

                        await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, messageToTgd7);

                        return;

                    case "/dh":
                        string messageToTgdh = $"Погода в {geodata[0].Name}, {geodata[0].Country}, {geodata[0].State}\n";

                        for (int i = 0; i < 24; i++)
                        {
                            messageToTgdh += $"{weather.hourly.time[i]}: Tемпература: {weather?.hourly?.temperature_2m[i]} {weather?.hourly_units?.temperature_2m} " +
                                $"Пориви вітру до {weather?.hourly?.windspeed_10m[i]} {weather?.hourly_units?.windspeed_10m}. " +
                                $"Кількість опадів: " +
                                $"Дощ: {weather?.hourly?.rain[i]} {weather?.hourly_units?.rain} " +
                                $"Сніг: {weather?.hourly?.snowfall[i]} {weather?.hourly_units?.snowfall}\n";
                        }

                        await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, messageToTgdh);

                        return;

                    default:
                        await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id,
                            $"Погода в {geodata[0].Name}, {geodata[0].Country}, {geodata[0].State}, станом на {weather.daily.time.FirstOrDefault()}.\n" +
                            $"Максимальна температура: {weather?.daily?.temperature_2m_max?.FirstOrDefault()} {weather?.daily_units?.temperature_2m_min}\n" +
                            $"Мінімальна температура: {weather?.daily?.temperature_2m_min?.FirstOrDefault()} {weather?.daily_units?.temperature_2m_min}\n" +
                            $"Пориви вітру до {weather?.daily?.windspeed_10m_max?.FirstOrDefault()} {weather?.daily_units?.windspeed_10m_max}\n" +
                            $"Кількість опадів:\n" +
                            $"Дощ: {weather?.daily?.rain_sum?.FirstOrDefault()} {weather?.daily_units?.rain_sum}\n" +
                            $"Сніг: {weather?.daily?.snowfall_sum?.FirstOrDefault()} {weather?.daily_units?.snowfall_sum}");

                        return;
                }
            }
            catch (Exception)
            {
                await _telegramClient.SendTextMessageAsync(update.Message.Chat.Id, $"Вибач, щось пішло не так. =( ");
            }
        }
    }
}

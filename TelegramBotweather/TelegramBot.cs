using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotweather
{
    internal class TelegramBot
    {
        private readonly string toren;
        private readonly TelegramBotClient client;
        /// <summary>
        /// телеграм токен
        /// </summary>
        /// <param name="toren"></param>
        public TelegramBot(string toren)
        {
            this.toren = toren;
            client = new TelegramBotClient(toren);
        }
        public async Task StartAsync()
        {
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await client.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }
        public string GetTelegramId()
        {
            return toren;
        }
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            long chatId = message.Chat.Id;

            if (message.Text == "Погода")
            {
                sentMessage(botClient, chatId, "Введите город", cancellationToken);
            }
            else if(message.Text == "/start")
            {
                sentMessage(botClient, chatId, "Добро пожаловать,введите название города", cancellationToken);
            }
            else
            {
                Weather weather = new();
                WeatherMain weatherInfo = await weather.GetIWeatherAsync(messageText);
                string temp;
                if (weatherInfo == null)
                    temp = "город не найден(";
                else
                    temp = $"Погода в городе {messageText} {Math.Round(weatherInfo.Main.Temp - 273.15f)}c";

                sentMessage(botClient, chatId, temp, cancellationToken);
            }
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        }
        async void sentMessage(ITelegramBotClient botClient, long chatId,string temp, CancellationToken cancellationToken)
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: temp,
                    cancellationToken: cancellationToken);
        }
    }
}

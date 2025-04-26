using My_telegram_bot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.VisualBasic;
using System.Threading;
using GTranslatorAPI;

namespace Telegram_Bot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Бот запущен.");
            CancellationTokenSource cts = new CancellationTokenSource();
            var bot = new TelegramBotClient("7973585300:AAEALQbThNyM5D5NaWmkbqBK4XQfLPHm0J4");
            await bot.DeleteWebhook();
            var Exception = new Exception();
            var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } };
            var update = new Update();

            BotMethods botMethods = new BotMethods();

            var me = await bot.GetMe();
            Console.WriteLine($"ID: {me.Id}, Имя: {me.FirstName}");
            Console.WriteLine("Enter - остановка");

            bot.StartReceiving(
            botMethods.HandleUpdateAsync,     
            botMethods.HandlePollingErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );
            Console.ReadLine();
            cts.Cancel();
            }
        }
    }

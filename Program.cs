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
            var bot = new TelegramBotClient("7973585300:AAGXOuR9Hb7wRj6Nihqpa2PFygPflf_cuYg");
            var Exception = new Exception();
            var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } };
            var update = new Update();

            var botMethods = new BotMethods();

            var me = await bot.GetMe();
            Console.WriteLine($"ID: {me.Id}, Имя: {me.FirstName}");

            bot.StartReceiving(
            updateHandler: botMethods.HandleUpdateAsync,
            botMethods.HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );
            Console.WriteLine("Enter - остановка");
            Console.ReadLine();
            cts.Cancel();
            }
        }
    }

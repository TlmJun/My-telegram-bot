using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_Bot;

namespace My_telegram_bot.Handlers
{
    internal class BotMethods
    {
        private static string BotToken = "7683673191:AAHoIE-NDTggGT4AhzuCdzZCoJtC5JiqasY";
        void OnMessage()
        {
            async Task HandleUpdateAsync(Message msg, UpdateType type)                                     /// Сообщение пользователя и ответ
            {
                using var cts = new CancellationTokenSource();
                var bot = new TelegramBotClient(BotToken, cancellationToken: cts.Token);
                var me = await bot.GetMe();
                Console.WriteLine($"Бот запущен. ID: {me.Id}, Имя: {me.FirstName}");

                Console.WriteLine("Enter - остановаить бота");
                Console.ReadLine();
                cts.Cancel();
                Console.WriteLine($"пользователь {msg.From}. Написал: {msg.Text}. В чате(ID) :{msg.Id}");
                string UserMessage = msg.Text!;
                switch (UserMessage)
                {
                    case "/start":
                        await bot.SendMessage(msg.Chat, $"Привет, {msg.Id}! Этот бот позволяет переводить сообщения",
                        replyMarkup: new InlineKeyboardButton[] { "с англ на рус", "с рус на англ" });
                        break;
                    case "/":
                        break;
                    case "Привет" or "привет":
                        await bot.SendMessage(msg.Id, "Пока:)");
                        break;
                }
            }
        }
        void OnUpdate()
        {
            async Task OnUpdate(Update update)                                                      /// какую кнопку нажал пользователь
            {
                using var cts = new CancellationTokenSource();
                var bot = new TelegramBotClient(BotToken, cancellationToken: cts.Token);
                if (update is { CallbackQuery: { } query })
                {
                    await bot.AnswerCallbackQuery(query.Id, $"You picked {query.Data}");
                    await bot.SendMessage(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
                }
            }
        }
        static void OnError()
        {
            async Task OnError(Exception exception, HandleErrorSource source)                      /// Обработка ошибок
            {
                Console.WriteLine(exception);
            }
        }
    }
    

        
  


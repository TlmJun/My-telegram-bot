using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;
using GTranslatorAPI;
using static System.Net.Mime.MediaTypeNames;
using System.Web;

namespace My_telegram_bot.Handlers
{
    internal class BotMethods
    {
        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)     /// Сообщение пользователя и ответ
        {
            try
            {
                if (update.Message is { } message)
                {
                    await GetUpdate(bot, message, update);
                }
                else if (update.CallbackQuery is { } callbackQuery)
                {
                    ////await HandleCallbackQuery(bot, callbackQuery);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки обновления: {ex.Message}");
            }
            
        }
        public async Task GetUpdate(ITelegramBotClient bot, Message msg, Update update)
        {
            var message = msg.Text!;
            var chatId = msg.Chat.Id;
            Console.WriteLine($"{msg.From} пишет: {message}. ID чата : {chatId}");
            switch (message)
            {
                case "/start":
                    await bot.SendMessage(chatId, $"Привет, {msg.From!.FirstName}! Этот бот позволяет переводить сообщения.");
                    await Task.Delay(100);
                    break;
                case "/author":
                    await bot.SendMessage(chatId, "@Tlmyr");
                    await bot.SendContact(chatId, "+79528637399", "Тимур");
                    await Task.Delay(100);
                    break;
                case "/translate":
                    await Translate(bot, chatId, msg, update);
                    break;
                case "/help":
                    await bot.SendMessage(chatId, "/tranlate - перевод сообщений");
                    await bot.SendMessage(chatId, "/author - автор");
                    break;
                default:
                    await bot.SendMessage(chatId, "я не понял");
                    break;
            }
        }

        public async Task Translate(ITelegramBotClient bot, ChatId chatId, Message msg, Update update)      //Перевод
        {
            HttpClient client = new HttpClient();
            string apiKey = "dict.1.1.20250418T142716Z.238261338557aea8.61b7dda74540dd85e88d1aee02b3e15668e082ac";
            var keyboard = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Английский", "Английский"), InlineKeyboardButton.WithCallbackData("Китайский", "Китайский") } });
            await bot.SendMessage(msg.Chat, "На каком языке хотите перевод?", replyMarkup: keyboard);
            var updates = new List<Update>{new Update { CallbackQuery = new CallbackQuery { Data = "Английский" } }, new Update { CallbackQuery = new CallbackQuery { Data = "Китайский" } } };
            foreach (var update1 in updates)
            {
                if (update.CallbackQuery is { } callbackQuery)
                {
                    string langCode = callbackQuery.Data;
                    switch (langCode)
                    {
                        case " Английский":
                            await bot.AnswerCallbackQuery(callbackQueryId: callbackQuery.Id, "Напишите слово и я его переведу");
                            var wordEn = msg.Text;
                            string Translated1 = $"https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key={apiKey}&lang=ru-en&text={wordEn}";
                            string JSONen = await client.GetStringAsync(apiKey); 
                            await bot.SendMessage(chatId, JSONen);
                            break;
                        case "Китайский":
                            await bot.AnswerCallbackQuery("Напишите слово и я его переведу");
                            var wordZh = msg.Text;
                            string TranslatedZh = $"https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key={apiKey}&lang=ru-zh&text={wordZh}";
                            string JSONzh = await client.GetStringAsync(apiKey);
                            await bot.SendMessage(chatId, JSONzh);
                            break;
                        default:
                            await bot.SendMessage(chatId, "я не понял");
                            break;
                    }
                }
            }
            
            

        }
        public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }
    } 
}
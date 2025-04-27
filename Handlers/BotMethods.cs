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
using System.Security.Policy;
using System.Text.Json;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;

namespace My_telegram_bot.Handlers
{
    internal class BotMethods
    {
        private string ApiKey = "dict.1.1.20250418T142716Z.238261338557aea8.61b7dda74540dd85e88d1aee02b3e15668e082ac";
        private readonly HttpClient client = new HttpClient();
        private Dictionary<long, string> LangChange = new Dictionary<long, string>();

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)     /// Сообщение пользователя и ответ
        {
            try
            {
                
                if (update.Message is { } message)
                {
                    if (message == null) return;
                    await GetUpdate(bot, message, update);
                }
                else { if (update.CallbackQuery is { } messagee)
                    {
                        await ChangeTranslate(bot, messagee, update);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки обновления: {ex.Message}");
            }
            
        }

        ///////         Команды          /////////////////////////////////////////////////        Команды         ////////////////////        Команды         /////////////

        public async Task GetUpdate(ITelegramBotClient bot, Message msg, Update update)
        {
            if (msg.Text == null) { return; }
            var message = msg.Text.ToLower();
            var chatId = msg.Chat.Id;
            Console.WriteLine($"{msg.From} пишет: {message}. ID чата : {chatId}");
            switch (message)
            {
                case "/start":
                    await bot.SendMessage(chatId, $"Привет, {msg.From!.FirstName}! Этот бот позволяет переводить сообщения.");
                    await Task.Delay(100);
                    break;
                case "/author":
                    await bot.SendMessage(chatId, "Меня создал: @Tlmyr");
                    await bot.SendContact(chatId, "+79528637399", "Тимур");
                    await Task.Delay(100);
                    break;
                case "/translate":
                    var keyboard = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Английский", "Английский"), InlineKeyboardButton.WithCallbackData("Китайский", "Китайский") } });
                    await bot.SendMessage(msg.Chat, "На каком языке хотите перевод?", replyMarkup: keyboard);
                    break;
                case "/help":
                    await bot.SendMessage(chatId, $"/translate - перевод сообщений {Environment.NewLine}/author - автор");
                    break;
                case "/stop":
                    await bot.SendMessage(chatId, "Окей, я больше не буду переводить");
                    LangChange.Remove(chatId);
                    break;
                default:
                    if (LangChange.TryGetValue(chatId, out var lang))
                    {
                        switch (lang)
                        {
                            case "en":
                                await Translater(lang, message, bot, chatId);
                                break;
                            case "zh":
                                await Translater(lang, message, bot, chatId);
                                break;
                        }
                    }
                    else
                    {
                        await bot.SendMessage(chatId, "Я не понял. /help - показать список команд");
                    }
                    break;
            }
        }

        ////////      Перевод     //////////////////////////            Перевод         ///////////////////////////////////////////////////    Перевод  ////////////////////////

        public async Task ChangeTranslate(ITelegramBotClient bot, CallbackQuery callback, Update update)                   // обработка кнопок
        {

            var ChatId = callback.Message.Chat.Id;     //Id с привязаной кнопки
            var msg = update.Message;
            var User = msg.From!.FirstName;

            await bot.SendMessage(ChatId, $"Введите слово и я его переведу на {callback.Data} и наоборот, /stop - остановка");

            switch (callback.Data)
            {
                case "Английский":
                    await bot.AnswerCallbackQuery(callback.Id);
                    Console.WriteLine($"{User} выбрал Английский");
                    LangChange[ChatId] = "en";
                    break;
                case "Китайский":
                    await bot.AnswerCallbackQuery(callback.Id);
                    Console.WriteLine($"{User} выбрал Китайский");
                    LangChange[ChatId] = "zh";
                    break;
            }
        }
        public async Task Translater(string lang, string message, ITelegramBotClient bot, ChatId chatId)                  // перевод
        {
            try
            {
                var langDirection = lang == "en" ? "ru-en" : "ru-zh";
                var Translation = await GetTranslation(lang, message, langDirection);
                await bot.SendMessage(chatId, $"Перевод {message}: {Translation}");
            }
            catch(Exception ex)
            {
                try
                {
                    await ReturnLang(lang, message, bot, chatId);
                }
                catch
                {
                    await bot.SendMessage(chatId, "Извините я не нашел перевод");
                }
            }
        }
        private async Task<string> GetTranslation(string lang, string message, string langDirection)                                            // получение JSON списка
        {
            var url = $"https://dictionary.yandex.net/api/v1/dicservice.json/lookup?key={ApiKey}&lang={langDirection}&text={Uri.EscapeDataString(message)}";
            var json = await client.GetStringAsync(url);
            return ParseTranslation(json);
        }
        private string ParseTranslation(string json)                                                                     // расшифровка JSON списка
        {
            using var doc = JsonDocument.Parse(json);
            var translations = doc.RootElement
                .GetProperty("def")[0]
                .GetProperty("tr");

            return translations[0].GetProperty("text").GetString();
        }
        async Task ReturnLang(string lang, string message, ITelegramBotClient bot, ChatId chatId)                       // если не получилось перевести он переводит наоборот
        {
            var langDirection = lang == "en" ? "en-ru" : "zh-en";
            var Translation = await GetTranslation(lang, message, langDirection);
            await bot.SendMessage(chatId, $"{message} на {lang}: {Translation}");
        }


        /////////           ошибки          /////////////////////////        ошибки          ///////////////////////////////////////////////////        ошибки        ////////////////////////////////////
        public Task HandlePollingErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }
    } 
}
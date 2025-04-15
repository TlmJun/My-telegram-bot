using My_telegram_bot.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram_Bot
{
    class Program 
    {
        
        static async Task Main(string[] args)
        {
            BotMethods botMethods;
            botMethods = new BotMethods();
            botMethods.OnMessage();
            botMethods.OnUpdate();
            botMethods.OnError();
        }
    }
}
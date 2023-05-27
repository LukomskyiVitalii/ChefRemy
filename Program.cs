using ChefRemy.Bot;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace ChefRemy
{
    class Program
    {
        public static readonly ChefRemyBot Bote;

        static Program()
        {
            Bote = new ChefRemyBot();
        }
        static async Task Main()
        {
            Bote.Start();
            Console.ReadLine();
        }

       
    }
}

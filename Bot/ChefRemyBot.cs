using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ChefRemy.Logic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using ChefRemy.Delegates;
using System.Text.Json;
using ChefRemy.Models;

namespace ChefRemy.Bot
{
    public class ChefRemyBot
    {
        private ResponseDelegate responseDelegate;
        private const string Token = "5914808391:AAFJkXHoFTNpuQyjQT6dQKb_fqKU3CrI2oc";
        private SearchingLogic searchingLogic;
        private Paginator paginator;
        private CoockingLogic coockingLogic;
        private static ITelegramBotClient Bot;

        public ChefRemyBot()
        {
            Bot = new TelegramBotClient(Token);
            responseDelegate = null;
            coockingLogic = new CoockingLogic();
            searchingLogic = new SearchingLogic();
            paginator = new Paginator();
        }

        public void Start()
        {
            Bot.StartReceiving(Updates, Error);
        }

        private ReplyKeyboardMarkup StartMenu
        {
            get
            {
                var keyboard = new ReplyKeyboardMarkup(new List<KeyboardButton>() { 
                new KeyboardButton(MenuButtons.GetRecipeByName),
                new KeyboardButton(MenuButtons.GetRecipesByIngridients),
                new KeyboardButton(MenuButtons.GetRecipesByCalories),
                });
                keyboard.OneTimeKeyboard = true;
                return keyboard;
            }
        }

        private ReplyKeyboardMarkup Back
        {
            get
            {
                var keyboard = new ReplyKeyboardMarkup(new List<KeyboardButton>() {
                new KeyboardButton(MenuButtons.Back)
                });
                keyboard.OneTimeKeyboard = true;
                return keyboard;
            }
        }

        private async  Task Updates(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            string messageText = String.Empty;
            long chatId = default(long);
            string username = String.Empty;
            int messageId = default(int);
            if(arg2.Type == UpdateType.Message)
            {
                messageText = arg2.Message.Text;
                chatId = arg2.Message.Chat.Id;
                username = arg2.Message.Chat.Username;
                messageId = arg2.Message.MessageId;
                if(!HandleReplyMarkupMessages(messageText, chatId, messageId))
                {
                    try
                    {
                        if (responseDelegate != null)
                        {
                            InvokeResponseDelegate(messageText, chatId);
                        }
                        else
                        {
                            GotoStartMenu(chatId, ErrorMessages.ChooseOption);
                        }
                    }
                    catch
                    {
                        this.SendTextMessage(chatId, "Sorry we can't find any recepies. Verify your input or try another one.");
                    }
                } 
            }
            else if(arg2.Type == UpdateType.CallbackQuery)
            {
                messageText = arg2.CallbackQuery.Data;
                chatId = arg2.CallbackQuery.From.Id;
                username = arg2.CallbackQuery.From.Username;
                messageId = arg2.CallbackQuery.Message.MessageId;
                if (!HandleInlineMarkupMessages(messageText, chatId, messageId))
                {
                    ShowRecipe(messageText, chatId);
                };
            }
            else
            {
                return;
            }
            Console.WriteLine($"Received a '{messageText}' message in chat {chatId} from user {username}.");
        }

        private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

        private async void GetRecipeByName(ChatId chatId)
        {
            Message sentMessage = await Bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: "Please Enter food name u want to coock",
                                        replyMarkup: new ReplyKeyboardRemove()
                                        );
            this.responseDelegate += this.searchingLogic.FindRecipeByName;
        }

        private async void GetRecipeByIngridients(ChatId chatId)
        {
            Message sentMessage = await Bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: "Please Enter ingridients (separate by coma)",
                                        replyMarkup: new ReplyKeyboardRemove()
                                        );
            this.responseDelegate += this.searchingLogic.FindRecipeByIngridients;
        }
        private async void GetRecipeByCalories(ChatId chatId)
        {
            Message sentMessage = await Bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: "Please Enter calories",
                                        replyMarkup: new ReplyKeyboardRemove()
                                        );
            this.responseDelegate += this.searchingLogic.FindRecipeByCalories;
        }

        private async void GotoStartMenu(ChatId chatId, string textToShow)
        {
            this.SendTextMessage(chatId, textToShow, this.StartMenu);
            responseDelegate = null;
            paginator.ClearData();
            coockingLogic.ClearData();
        }

        private async void SendTextMessage(ChatId chatId, string textToShow, IReplyMarkup keyboardMarkup = null)
        {
            if(keyboardMarkup == null)
            {
                keyboardMarkup = new ReplyKeyboardRemove();
            }
            Message sentMessage = await Bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: textToShow,
                                        replyMarkup: keyboardMarkup
                                        );
        }

        private async void StartWork(ChatId chatId)
        {
            var message = "Hello my name is Cheff Remy and i here to help you out in kitchen.\nPlease select option from menu.";
            Message sentMessage = await Bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: message,
                                        replyMarkup: this.StartMenu
                                        );
            responseDelegate = null;
        }

        private async void InvokeResponseDelegate(string messageText,long chatId)
        {
            try
            {
                var response = await responseDelegate(messageText);
                paginator.LoadData(response);
                if (await this.ShowOptions(chatId))
                {
                    this.responseDelegate = null;
                }
            }
            catch(Exception ex)
            {
                SendTextMessage(chatId, ex.Message);
            }
        }

        private async Task<bool> ShowOptions(ChatId chatId, int messageId = default(int))
        {
            if(paginator.hasPages())
            {
                if (messageId == default(int))
                {
                    await Bot.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Getting recepies...",
                        replyMarkup: this.Back
                    );
                    await Bot.SendTextMessageAsync(
                        chatId: chatId,
                        text: paginator.pageName,
                        replyMarkup: paginator.GetPage()
                    );
                }
                else
                {

                    await Bot.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: messageId,
                        text: paginator.pageName);
                    await Bot.EditMessageReplyMarkupAsync(
                        chatId: chatId,
                        messageId: messageId,
                        replyMarkup: paginator.GetPage());
                }
            }
            else
            {
                SendTextMessage(chatId, "Sorry we can't find any recepies. Verify your input or try another one.");
            }
            return paginator.hasPages();
        }
        private bool HandleReplyMarkupMessages(string messageText, ChatId chatId, int messageId)
        {
            bool result = false;
            try
            {
                switch (messageText)
                {
                    case MenuButtons.Start:
                        StartWork(chatId);
                        result = true;
                        break;
                    case MenuButtons.GetRecipeByName:
                        GetRecipeByName(chatId);
                        result = true;
                        break;
                    case MenuButtons.GetRecipesByIngridients:
                        GetRecipeByIngridients(chatId);
                        result = true;
                        break;
                    case MenuButtons.GetRecipesByCalories:
                        GetRecipeByCalories(chatId);
                        result = true;
                        break;
                    case MenuButtons.Back:
                        GotoStartMenu(chatId, ErrorMessages.GoBack);
                        result = true;
                        break;
                }
            }
            catch
            {
                GotoStartMenu(chatId, ErrorMessages.NoRecepiesWereFound);
            }
            return result;
        }

        private bool HandleInlineMarkupMessages(string messageText, ChatId chatId, int messageId)
        {
            bool result = false;
            try
            {
                switch (messageText)
                {
                    case ("Previous page"):
                        paginator.MovePreviousPage();
                        ShowOptions(chatId, messageId);
                        result = true;
                        break;
                    case ("Next page"):
                        paginator.MoveNextPage();
                        ShowOptions(chatId, messageId);
                        result = true;
                        break;
                    case (MenuButtons.NextStep):
                    case (MenuButtons.StartCoocking):
                        this.ShowRecipeStep(chatId);
                        result = true;
                        break;
                    case (MenuButtons.Congrats):
                        this.GotoStartMenu(chatId, "Enjoy your meal!");
                        result = true;
                        break;
                    case (MenuButtons.BackToRecepies):
                        this.GoBackToRecepies(chatId, messageId);
                        result = true;
                        break;
                }
            }
            catch
            {
                GotoStartMenu(chatId, ErrorMessages.NoRecepiesWereFound);
            }
            return result;
        }

        private async void ShowRecipe(string recipeId,ChatId chatId)
        {
            var k = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(MenuButtons.StartCoocking),
                            InlineKeyboardButton.WithCallbackData(MenuButtons.BackToRecepies)
                        }
                    });
            this.coockingLogic.SetRecipe(await this.searchingLogic.GetRecipe(recipeId));
            try
            {
                if (this.coockingLogic.recipePhoto != null)
                {

                    await Bot.SendPhotoAsync(
                                    chatId: chatId,
                                    photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(this.coockingLogic.recipePhoto),
                                    caption: this.coockingLogic.Title,
                                    replyMarkup: k
                                );
                    await Bot.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: this.coockingLogic.getSummary(),
                                    replyMarkup: k
                                );


                }
                else
                {
                    await Bot.SendTextMessageAsync(
                                        chatId: chatId,
                                        text: this.coockingLogic.getSummary(),
                                        replyMarkup: k
                                    );
                }
            }
            catch
            {
                GotoStartMenu(chatId, ErrorMessages.NoRecepieDetailsWereFound);
            }
        }

        private async void ShowRecipeStep(ChatId chatId)
        {
            try
            {
                string step = this.coockingLogic.GetStep();
                await Bot.SendTextMessageAsync(
                    chatId: chatId,
                    text: step,
                    replyMarkup: this.coockingLogic.Step
                );
            }
            catch
            {
                this.GotoStartMenu(chatId, ErrorMessages.NoRecepieDetailsWereFound);
            }
            
        }

        private async void GoBackToRecepies(ChatId chatId,int messageId)
        {
            await Bot.DeleteMessageAsync(chatId, messageId);
            this.coockingLogic.ClearData();

        }
    }

    static class MenuButtons
    {
        public const string Start = "/start";
        public const string GetRecipeByName = "Get recipe by its name";
        public const string GetRecipesByIngridients = "Get recipes by ingridients";
        public const string GetRecipesByCalories = "Get recipes by calories";
        public const string Back = "Back";
        public const string BackToRecepies = "Go back to Recepies";
        public const string StartCoocking = "Start cocking";
        public const string NextStep = "Next Step";
        public const string Congrats = "Congrats";
    }

    static class ErrorMessages
    {
        public const string ChooseOption = "Please choose an menu option";
        public const string GoBack = "Going back";
        public const string NoRecepiesWereFound = "Sorry we didn't find any recepies that matches desccription. Please try to be more speciffic.";
        public const string NoRecepieDetailsWereFound = "Sorry we dont have details for this speciffic recepie.";
        public const string InpuIsIncoretFormat = "Sorry you have to input number.";
    }
}

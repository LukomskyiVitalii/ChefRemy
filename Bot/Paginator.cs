using ChefRemy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChefRemy.Bot
{
    public class Paginator
    {
        private const int ItemsPerPage = 5;
        private int totalPages;
        private int curentPageNumber;
        public string pageName
        {
            get
            {
                return $"Page {curentPageNumber}";
            }
        }
        private Dictionary<int, List<InlineKeyboardButton[]>> pages;

        public Paginator()
        {
            pages = new Dictionary<int, List<InlineKeyboardButton[]>>();
            curentPageNumber = 1;
        }
        private InlineKeyboardButton[] GetControlButtons(int pageNumber)
        {
                string previous = "Previous page";
                string next = "Next page";
                var controls = new List<InlineKeyboardButton>();
                if (pageNumber != 1) controls.Add(InlineKeyboardButton.WithCallbackData(previous, previous));
                if (pageNumber != totalPages) controls.Add(InlineKeyboardButton.WithCallbackData(next, next));
                return controls.ToArray();
        }


        public void LoadData(List<Recipe> options)
        {
            List<InlineKeyboardButton> elements = new List<InlineKeyboardButton>();
            totalPages = (int)Math.Ceiling((double)options.Count / ItemsPerPage);
            foreach (var option in options)
            {
                string buttonText = option.title;
                string callbackData = option.id.ToString();
                InlineKeyboardButton button = InlineKeyboardButton.WithCallbackData(buttonText, callbackData);
                elements.Add(button);
            }
            IEnumerator<InlineKeyboardButton> enumerator = elements.GetEnumerator();

            for (int pageNumber = 1; pageNumber <= totalPages; pageNumber++)
            {
                List<InlineKeyboardButton[]> rows = new List<InlineKeyboardButton[]>();
                for (int item = 1; item <= ItemsPerPage; item++)
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    rows.Add(new[] { enumerator.Current }); 
                }
                rows.Add(GetControlButtons(pageNumber));
                pages.Add(pageNumber, rows);
            }
        }

        public void ClearData()
        {
            pages.Clear();
            curentPageNumber = 1;
        }

        public bool hasPages()
        {
            return this.pages.Count != 0;
        }

        public InlineKeyboardMarkup? GetPage()
        {
            var options = pages.GetValueOrDefault(curentPageNumber);
            return new InlineKeyboardMarkup(options);

        }

        public void MoveNextPage()
        {
            curentPageNumber += 1;

            
        }

        public void MovePreviousPage()
        {
            curentPageNumber -= 1;
        }
    }
}

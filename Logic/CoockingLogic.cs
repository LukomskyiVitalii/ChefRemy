using ChefRemy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;

namespace ChefRemy.Logic
{
    public class CoockingLogic
    {
        public Recipe CurrentRecipe;
        public InlineKeyboardMarkup Step
        {
            private set
            {
                Step = value;
            }
            get
            {
                if (this.CurrentRecipe.analyzedInstructions != null && ((this.CurrentStepIndex + 1 >= this.CurrentRecipe.analyzedInstructions[CurrentSubStepIndex].steps.Count) && this.CurrentSubStepIndex + 1 >= this.CurrentRecipe.analyzedInstructions.Count))
                { 
                    return new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Congrats")
                        }
                    });
                }
                else
                {
                    return new InlineKeyboardMarkup(new[]
  {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("Next Step")
                        }
                    });
                }
            }
        }

        public string recipePhoto { 
            get
            {
                return CurrentRecipe?.image;
            } 
        }

        public string Title { get
            {
                return CurrentRecipe?.title;
            } 
        }

        private int CurrentStepIndex ;
        private int CurrentSubStepIndex;

        public CoockingLogic()
        {
            CurrentRecipe = new Recipe();
            CurrentStepIndex = 0;
            CurrentStepIndex = 0;
        }

        public void SetRecipe(Recipe recipe)
        {
            this.CurrentRecipe = recipe;
            CurrentStepIndex = 0;
            CurrentStepIndex = 0;
        }

        public void ClearData()
        {
            this.CurrentRecipe = null;
            CurrentStepIndex = 0;
            CurrentStepIndex = 0;
        }

        public string getSummary()
        {
            string ingridients = String.Empty;
            foreach(var ingridient in this.CurrentRecipe.extendedIngredients)
            {
                ingridients += ingridient.originalName+" ";
            }
            return this.CurrentRecipe?.title + "\n"+"Time to coock in minutes:" + this.CurrentRecipe?.readyInMinutes +"\n" +"Ingridients: "+ ingridients+ "\n" + Regex.Replace(this.CurrentRecipe?.summary, "<.*?>", string.Empty);
        }

        public string GetStep()
        {
            string result = String.Empty;
            if (CurrentRecipe.analyzedInstructions == null || CurrentRecipe.analyzedInstructions.Count == 0) throw new Exception("This recepie doesn't have steps. We are sorry.");
            var currentStep = this.CurrentRecipe.analyzedInstructions[CurrentSubStepIndex];
            result += $"Step {CurrentSubStepIndex+1}-{currentStep.steps[CurrentStepIndex].number }";
            result += $"\n{currentStep.steps[CurrentStepIndex].step}.";
            this.MoveNext();
            return result;
        }

        public void MoveNext()
        {
            if(this.CurrentStepIndex+1 < this.CurrentRecipe.analyzedInstructions[CurrentSubStepIndex].steps.Count)
            {
                CurrentStepIndex += 1;
            }
            else if(this.CurrentSubStepIndex + 1 < this.CurrentRecipe.analyzedInstructions.Count)
            {
                CurrentSubStepIndex += 1;
                CurrentStepIndex = 0;
            }
            
        }

    }
}

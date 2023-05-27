using ChefRemy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChefRemy.Logic
{
    public class SearchingLogic
    {
        SpoonacularAPI api;

        public SearchingLogic()
        {
            api = new SpoonacularAPI();
        }
        public async Task<List<Recipe>> FindRecipeByName(string name)
        {
            var result = await api.GetAutocompleteRecipeSearch(name);
            return JsonSerializer.Deserialize<List<Recipe>>(result);
        }

        public async Task<List<Recipe>> FindRecipeByIngridients(string ingridients)
        {
            var result = await api.GetRecipesByIngredients(ingridients);
            return JsonSerializer.Deserialize<List<Recipe>>(result);
        }

        public async Task<List<Recipe>> FindRecipeByCalories(string calories)
        {
            try
            {
                var result = await api.GetRecipesByCalories(Convert.ToInt64(calories));
                return JsonSerializer.Deserialize<List<Recipe>>(result);
            }
            catch
            {
                throw new Exception("Please enter a whole number.");
            }
            
        }

        public async Task<Recipe> GetRecipe(string recipeId)
        {
            var recipe = JsonSerializer.Deserialize<Recipe>(await api.GetRecipe(Convert.ToInt32(recipeId)));
            return recipe;
        }
    }
}

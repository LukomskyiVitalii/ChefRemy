using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ChefRemy.Models;
using System.Text.RegularExpressions;

namespace ChefRemy.Logic
{
    class SpoonacularAPI
    {
        private const string baseURl = "https://api.spoonacular.com";
        public const string apiKey = "8144cafe3eb64364a39106ee9b9ec14e";

        public async Task<string> GetAutocompleteRecipeSearch(string recipeName)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURl+ $"/recipes/autocomplete?query={recipeName}&number=20&apiKey=" + apiKey);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody; 
        }

        public async Task<string> GetRecipesByIngredients(string ingridients)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURl + $"/recipes/findByIngredients?ingredients={ingridients}&number=20&limitLicense=true&ranking=1&ignorePantry=false&apiKey=" + apiKey);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody; 
        }

        public async Task<string> GetRecipesByCalories(float calories)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURl + $"/recipes/findByNutrients?minCalories={calories}&maxCalories={calories+50}&apiKey=" + apiKey);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public async Task<string> GetRecipesInstruction(int recipeID)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURl + $"/recipes/{recipeID}/analyzedInstructions?apiKey=" + apiKey);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public async Task<string> GetRecipe(int recipeId)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(baseURl + $"/recipes/{recipeId}/information?apiKey=" + apiKey);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
            
        }
        
}
}

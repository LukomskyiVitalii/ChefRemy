using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChefRemy.Models
{
    public class Recipe
    {
        public string title { get; set; }
        public int id { get; set; }

        public int readyInMinutes { get; set; }

        public List<ExtendedIngredient> extendedIngredients { get; set; }

        public string summary { get; set; }

        public string image { get; set; }

        public string instructions { get; set; }

        public List<AnalyzedInstruction> analyzedInstructions { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChefRemy.Models
{
    public class AnalyzedInstruction
    {
        public string name { get; set; }
        public List<RecipeStep> steps { get; set; }
    }
    public class RecipeStep
    {
        public int number { get; set; }
        public string step { get; set; }
    }
}
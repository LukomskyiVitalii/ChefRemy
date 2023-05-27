using ChefRemy.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChefRemy.Delegates
{
    delegate Task<List<Recipe>> ResponseDelegate(string messageText);
}

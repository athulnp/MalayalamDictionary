using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MalayalamDictionary.Bots
{
    public class WordNotFoundException : Exception
    {
        private string word;
        public WordNotFoundException(string word) : base($"Not found : {word}")
        {
            this.word = word;
        }
        public string Word 
        {
            get { return this.word; }
        }
    }
}

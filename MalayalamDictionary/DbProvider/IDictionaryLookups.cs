using System;
using System.Collections.Generic;

namespace MalayalamDictionary.DbProvider
{
   public interface IDictionaryLookup : IDisposable
    {
        IEnumerable<string> GetMeanings(string word);
    }
}

using Microsoft.Extensions.Logging;
using System;

namespace MalayalamDictionary.Logger
{
    public interface ILoggerProvider : IDisposable
    {
        ILogger CreateLogger(string CategoryName);
    }
}

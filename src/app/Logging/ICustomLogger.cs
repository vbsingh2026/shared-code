using System;

namespace SmartCart.Infrastructure.Logging
{
    public interface ICustomLogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(Exception exception, string message);
    }
}
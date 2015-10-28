using System;

namespace rift.common.logger
{
    public interface ILogger
    {
        void Error(Exception exception);

        void Error(string message);

        void Trace(Exception exception);

        void Trace(string message);
    }
}

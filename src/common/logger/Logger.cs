using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace rift.common.logger
{
    public class Logger : ILogger
    {
        private static string tracePath = "trace.log";
        private static string errorPath = "error.log";

        private readonly BufferBlock<Tuple<LogLevel, Exception>> _buffer = new BufferBlock<Tuple<LogLevel, Exception>>();

        private readonly ActionBlock<Tuple<LogLevel, Exception>> _traceLogger = new ActionBlock<Tuple<LogLevel, Exception>>(e =>
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(tracePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                var buffer = Encoding.UTF8.GetBytes(string.Format("TRACE: [{0}] {1}{2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), e.Item2, Environment.NewLine));
                stream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        });

        private readonly ActionBlock<Tuple<LogLevel, Exception>> _errorLogger = new ActionBlock<Tuple<LogLevel, Exception>>(e =>
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(errorPath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                var buffer = Encoding.UTF8.GetBytes(string.Format("ERROR: [{0}] {1}{2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), e.Item2, Environment.NewLine));
                stream.WriteAsync(buffer, 0, buffer.Length);
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        });

        public Logger()
        {
            _buffer.LinkTo(_traceLogger, e => (e.Item1 & LogLevel.Trace) != LogLevel.None);
            _buffer.LinkTo(_errorLogger, e => (e.Item1 & LogLevel.Error) != LogLevel.None);
        }

        public void Error(Exception exception)
        {
            _buffer.Post(new Tuple<LogLevel, Exception>(LogLevel.Error, exception));
        }

        public void Error(string message)
        {
            _buffer.Post(new Tuple<LogLevel, Exception>(LogLevel.Error, new Exception(message)));
        }

        public void Trace(Exception exception)
        {
            _buffer.Post(new Tuple<LogLevel, Exception>(LogLevel.Trace, exception));
        }

        public void Trace(string message)
        {
            _buffer.Post(new Tuple<LogLevel, Exception>(LogLevel.Trace, new Exception(message)));
        }
    }
}

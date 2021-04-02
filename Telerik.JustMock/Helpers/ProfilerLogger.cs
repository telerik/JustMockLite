/*
 JustMock Lite
 Copyright © 2021 Progress Software Corporation

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

#if DEBUG
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Telerik.JustMock.Helpers
{
    internal static class UnixTimeExtension
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTimestamp(this DateTime date)
        {
            return (date - epoch).Ticks;
        }
    }

    internal enum LogLevel
    {
        TRACE = 0,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        CRITICAL,
        OFF
    }

    internal static class DefaultLogLevel
    {
        static DefaultLogLevel()
        {
            Value = LogLevel.OFF;
        }

        internal static LogLevel Value { get; set; }
    }

    internal static class ProfilerLogger
    {
        static ProfilerLogger()
        {
            CurrentLogLevel = DefaultLogLevel.Value;
        }

        internal static LogLevel CurrentLogLevel { get; private set; }

        internal static void Trace(string formatMessage, params object[] args)
        {
            Log(LogLevel.TRACE, formatMessage, args);
        }

        internal static void Debug(string formatMessage, params object[] args)
        {
            Log(LogLevel.DEBUG, formatMessage, args);
        }

        internal static void Info(string formatMessage, params object[] args)
        {
            Log(LogLevel.INFO, formatMessage, args);
        }

        internal static void Warn(string formatMessage, params object[] args)
        {
            Log(LogLevel.WARN, formatMessage, args);
        }

        internal static void Error(string formatMessage, params object[] args)
        {
            Log(LogLevel.ERROR, formatMessage, args);
        }

        internal static void Critical(string formatMessage, params object[] args)
        {
            Log(LogLevel.CRITICAL, formatMessage, args);
        }

        private static class MessageFormatHelper
        {
            private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            private static readonly string LoggerName = "JustMockProfiler";

            [DllImport("kernel32.dll")]
            public static extern int GetCurrentThreadId();

            public static string FormatMessage(LogLevel level, string formatMessage, params object[] args)
            {
                var builder = new StringBuilder();

                var stackTrace = new StackTrace(true);
                var frame = stackTrace.GetFrame(3);
                builder.AppendFormat(
                    "{0} [{1}] {2} {3} {4}.{5}({6}:{7}) ",
                    LoggerName,
                    level,
                    DateTime.UtcNow.ToUnixTimestamp(),
                    GetCurrentThreadId(),
                    frame.GetMethod().DeclaringType.Name,
                    frame.GetMethod().Name,
                    frame.GetFileName(),
                    frame.GetFileLineNumber());

                builder.AppendFormat(formatMessage, args);

                return builder.ToString();
            }
        }

        private static void Log(LogLevel level, string formatMessage, params object[] args)
        {
            if (level < CurrentLogLevel)
            {
                return;
            }

            var logMessage = MessageFormatHelper.FormatMessage(level, formatMessage, args);

            System.Diagnostics.Debug.AutoFlush = true;
            System.Diagnostics.Debug.WriteLine(logMessage);
        }
    }
}
#endif

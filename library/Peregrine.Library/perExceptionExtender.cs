using System;
using System.Linq;
using System.Text;

namespace Peregrine.Library
{
    /// <summary>
    /// Helper class for <see cref="Exception" />
    /// </summary>
    public static class perExceptionExtender
    {
        /// <summary>
        /// Format a single exception as a text string
        /// </summary>
        /// <param name="exception"></param>
        public static string GetText(this Exception exception)
        {
            var builder = new StringBuilder();
            builder.AppendLine(exception.GetType().ToString());

            if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                builder.AppendLine();
                builder.AppendLine(exception.Message);
            }

            if (!string.IsNullOrWhiteSpace(exception.StackTrace))
            {
                builder.AppendLine();
                builder.AppendLine(exception.StackTrace);
            }

            if (exception.InnerException != null)
            {
                builder.AppendLine();
                builder.AppendLine(new string('-', 50));
                builder.AppendLine();
                builder.Append(exception.InnerException.GetText());
            }

            return builder.ToString();
        }

        /// <summary>
        /// Format all children of an <see cref="AggregateException"/> into a single string
        /// </summary>
        /// <param name="aggregateException"></param>
        public static string GetText(this AggregateException aggregateException)
        {
            return string.Join(
                "\r\n" + new string('=', 80) + "\r\n",
                aggregateException.InnerExceptions.Select(ex => ex.GetText()));
        }
    }
}

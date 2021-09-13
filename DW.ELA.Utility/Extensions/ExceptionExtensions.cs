using System;
using System.Text;

namespace DW.ELA.Utility.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetStackedErrorMessages(this Exception ex, int offset = 0)
        {
            var builder = new StringBuilder();
            builder.Append(new string(' ', offset));
            builder.AppendLine(ex.Message);
            offset += 2;

            if (ex is AggregateException aex)
            {
                aex = aex.Flatten();
                foreach (var ce in aex.InnerExceptions)
                    builder.AppendLine(GetStackedErrorMessages(ce, offset));
            }
            else if (ex.InnerException != null)
            {
                builder.AppendLine(GetStackedErrorMessages(ex.InnerException, offset));
            }

            return builder.ToString();
        }
    }
}

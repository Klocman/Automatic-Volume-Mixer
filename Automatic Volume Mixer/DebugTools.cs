using System;
using System.Diagnostics;

namespace Avm
{
    internal static class DebugTools
    {
        public static void WriteException(Exception ex)
        {
#if DEBUG
            var stackTrace = new StackTrace();
            var methodBase = stackTrace.GetFrame(1).GetMethod();

            Debug.WriteLine(
                $@"Exception in {methodBase.DeclaringType}\{methodBase.Name} -> {ex.Message}" +
                (ex.InnerException == null ? string.Empty : $@"-> {ex.InnerException.Message}"));
#endif
        }
    }
}
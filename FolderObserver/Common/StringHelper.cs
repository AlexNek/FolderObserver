using System;
using System.Globalization;

namespace FolderObserver.Common
{
    internal static class StringHelper
    {
        public static void AssertArgumentHasText(this string argument, string name)
        {
            if (argument.IsNullOrEmpty())
            {
                throw new ArgumentNullException(
                    name,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Argument '{0}' cannot be null or resolve to an empty string : '{1}'.",
                        name,
                        argument));
            }
        }

        public static bool IsNullOrEmpty(this string argument)
        {
            return String.IsNullOrEmpty(argument);
        }
    }
}

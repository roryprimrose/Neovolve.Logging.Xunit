namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;

    internal static class StringShim
    {
        /// <summary>
        ///     Determines whether the specified string contains a specified value using the specified comparison rules.
        /// </summary>
        /// <param name="value">The string to search.</param>
        /// <param name="valueToFind">The value to find within the string.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>
        ///     true if the value parameter occurs within the string parameter, or if value is the empty string ("");
        ///     otherwise, false.
        /// </returns>
        public static bool Contains(this string value, string valueToFind, StringComparison comparison)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return value.IndexOf(valueToFind, comparison) >= 0;
        }
    }
}
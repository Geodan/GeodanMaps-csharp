namespace Geodan.Cloud.Client.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove the last trailing slash from the string
        /// </summary>
        public static string RemoveTrailingSlash(this string value)
        {
            return value.TrimEnd(new[] { '/' });
        }
    }
}

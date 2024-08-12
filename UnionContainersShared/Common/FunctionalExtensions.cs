using System.Text;

namespace UnionContainers.Shared.Common;

public static class FunctionalExtensions
{
    /// <summary>
    /// Performs an action on each item in the source collection
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
    
    /// <summary>
    /// Extension method that calls <see cref="String.IsNullOrWhiteSpace"/>>
    /// </summary>
    /// <param name="source">string to check</param>
    /// <returns>true if the string is null, empty (""), or whitespace (" "). Returns false otherwise.</returns>
    public static bool IsEmpty(this string? source) => string.IsNullOrWhiteSpace(source);
    
    /// <summary>
    /// Extension method that calls <see cref="String.IsNullOrWhiteSpace"/>>
    /// </summary>
    /// <param name="source">string to check</param>
    /// <returns>true if the string is not null, empty, or whitespace</returns>
    public static bool HasValue(this string? source) => !string.IsNullOrWhiteSpace(source);
    
    public static bool IsNull<T>(this T? item) => item is null;
    
    public static bool IsNotNull<T>(this T? item) => item is not null;
    
    public static bool EqualsCaseInsensitive(this string? source, string target) => source.IsNotNull() && source!.Equals(target, StringComparison.OrdinalIgnoreCase); 
    
    public static bool None<T>(this IEnumerable<T> source) => !source.Any();
    
    /// <summary>
    /// Extension method that calls <see cref="File.Exists"/>
    /// </summary>
    /// <param name="path">The path to check as a string</param>
    /// <returns>true if the file was not found, false if the file was found</returns>
    public static bool DoesNotExistAsPath(this string? path) => !File.Exists(path);

    /// <summary>
    /// Tries to convert a byte array to a UTF8 string
    /// If the byte array is null, empty, or cannot be converted, an empty string is returned
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToUTF8String(this byte[]? bytes)
    {
        try
        {
            return bytes is null ? "" : Encoding.UTF8.GetString(bytes);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "";
        }
    }
    
    /// <summary>
    /// Converts a list of items to a comma separated string using
    /// <code>ToString()</code> on each item
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToCommaSeparatedString<T>(this IEnumerable<T> source) => string.Join(", ", source.Select(x => x?.ToString()));

    /// <summary>
    /// Converts a list of items to a command separated string using the provided selector
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string ToCommaSeparatedString<T>(this IEnumerable<T> source, Func<T, string> selector) => string.Join(", ", source.Select(selector));
    
    
}
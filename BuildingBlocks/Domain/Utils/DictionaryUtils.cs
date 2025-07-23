namespace BuildingBlocks.Domain.Utils;

using System;
using System.Collections.Generic;

public static class DictionaryUtils
{
    /// <summary>
    /// Tries to get a value from a dictionary by key and cast it to the specified type.
    /// Returns default(T) if the key doesn't exist or cast fails.
    /// </summary>
    public static T GetValue<T>(this Dictionary<string, object> dict, string key, T defaultValue = default)
    {
        if (dict == null || string.IsNullOrEmpty(key)) return defaultValue;

        if (dict.TryGetValue(key, out var value))
        {
            try
            {
                // If already correct type
                if (value is T tValue)
                    return tValue;

                // Try convert from boxed value
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                // Fail silently and return default
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Tries to get a nested dictionary from a dictionary by key.
    /// </summary>
    public static Dictionary<string, object> GetDictionary(this Dictionary<string, object> dict, string key)
    {
        if (dict == null || string.IsNullOrEmpty(key)) return null;

        if (dict.TryGetValue(key, out var value))
        {
            return value as Dictionary<string, object>;
        }

        return null;
    }

    /// <summary>
    /// Tries to get a list of dictionaries from a dictionary by key.
    /// </summary>
    public static List<Dictionary<string, object>> GetDictionaryList(this Dictionary<string, object> dict, string key)
    {
        if (dict == null || string.IsNullOrEmpty(key)) return null;

        if (dict.TryGetValue(key, out var value))
        {
            return value as List<Dictionary<string, object>>;
        }

        return null;
    }
}

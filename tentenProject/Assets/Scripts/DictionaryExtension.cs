using System.Collections;
using System.Collections.Generic;

public class Dictionary<T, T2, T3> : Dictionary<T, Dictionary<T2, T3>>
{
}

public class Dictionary<T, T2, T3, T4> : Dictionary<T, Dictionary<T2, Dictionary<T3, T4>>>
{
}

public static class DictionaryExtensions
{
    public static void AddOrSet<T, T2>(this Dictionary<T, T2> _dict, T _key, T2 _value)
    {
        if (!_dict.TryAdd(_key, _value))
            _dict[_key] = _value;
    }

    public static Dictionary<T2, T3> GetDict<T, T2, T3>(this Dictionary<T, Dictionary<T2, T3>> _dict, T _key)
    {
        if (!_dict.TryGetValue(_key, out var dict))
        {
            dict = new Dictionary<T2, T3>();
            _dict.Add(_key, dict);
        }

        return dict;
    }

    public static Dictionary<T3, T4> GetDict<T, T2, T3, T4>(this Dictionary<T, Dictionary<T2, Dictionary<T3, T4>>> _dict, T _key, T2 _key2)
    {
        if (!_dict.TryGetValue(_key, out var dict))
        {
            dict = new Dictionary<T2, Dictionary<T3, T4>>();
            _dict.Add(_key, dict);
        }
        
        return dict.GetDict(_key2);
    }
    
    public static void AddOrSet<T, T2, T3>(this Dictionary<T, Dictionary<T2, T3>> _dict, T _key, T2 _key2, T3 _value)
    {
    }
}
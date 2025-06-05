using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> _data = new Dictionary<string, object>();

    public void SetValue(string key, object value) => _data[key] = value;
    public T GetValue<T>(string key) => _data.ContainsKey(key) ? (T) _data[key] : default; 
}
using System.Collections.Generic;

public class Blackboard
{
    private Dictionary<string, object> data = new Dictionary<string, object>();

    public void SetValue(string key, object value) => data[key] = value;
    public T GetValue<T>(string key) => data.ContainsKey(key) ? (T) data[key] : default; 
}
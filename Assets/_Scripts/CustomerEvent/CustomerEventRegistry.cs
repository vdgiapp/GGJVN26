using System.Collections.Generic;
using UnityEngine;

public class CustomerEventRegistry : MonoBehaviour
{
    private Dictionary<string, ICustomerEvent> _events = new();
    
    private void Awake()
    {
        _events["scaryguy1"] = new ScaryGuyCustomerEvent();
    }
    
    public ICustomerEvent GetEvent(string customerEventName)
    {
        if (string.IsNullOrEmpty(customerEventName)) return null;
        _events.TryGetValue(customerEventName, out var e);
        return e;
    }
}
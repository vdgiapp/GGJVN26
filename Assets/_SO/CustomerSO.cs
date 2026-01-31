using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "Scriptable Objects/Customer Data", order = 1)]
public class CustomerSO : ScriptableObject
{
    public string customerEvent; // for some special customer
    public string requiredMaskId;
    
    [TextArea] public List<string> messages;
    [TextArea] public List<string> failedMessages;
    [TextArea] public List<string> successMessages;
}
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public CustomerSO customerData;

    public bool isTalked;
    public bool isServed;
    
    // nếu có con có animation khác thì thêm
    public Animator animator;
    public int currentMessageIndex;
    
    public bool CheckForRequiredMask(string maskId)
    {
        return maskId == customerData.requiredMaskId;
    }
}
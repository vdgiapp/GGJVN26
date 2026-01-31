using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    public CustomerSO customerData;

    public bool isTalked;
    public bool isServed;
    
    public int currentMessageIndex;
    
    public bool CheckForRequiredMask(string maskId)
    {
        return maskId == customerData.requiredMaskId;
    }
}
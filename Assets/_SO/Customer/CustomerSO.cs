using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerSO", menuName = "Scriptable Objects/CustomerSO")]
public class CustomerSO : ScriptableObject
{
    [Header("Type")]
    public CustomerType type;

    [Header("Mask")]
    public List<MaskSO> Masks;

    [Header("Special Rules")]
    public bool IsFirst;// For FollowUp type: is this the first customer in the follow-up sequence
}
public enum CustomerType
{
    Normal,
    VIP,
    FollowUp,
    Duo
}
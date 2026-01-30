using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private CustomerSO customerData;
    private MaskSO WantMask;
    private List<string> Messages;
    int indexMessage = 0;

    // Partner for Duo mechanic
    public Customer Partner { get; set; }
    // Served flag used to know whether this customer was already served
    public bool Served { get; set; }

    public void Initialize(CustomerSO customerData)
    {
        this.customerData = customerData;
        // reset state
        Served = false;
        Partner = null;

        // Select a random mask from the customer data
        int RandomMaskIndex = Random.Range(0, customerData.Masks.Count);
        WantMask = customerData.Masks[RandomMaskIndex];
        int RandomMessageGroup = Random.Range(0, WantMask.MessageGroups.Count);
        Messages = WantMask.MessageGroups[RandomMessageGroup].Messages;
        indexMessage = 0;
    }

    // Overload to force a specific mask (used by FollowUp / Duo mechanics)
    public void Initialize(CustomerSO customerData, MaskSO forcedMask)
    {
        this.customerData = customerData;
        // reset state
        Served = false;
        Partner = null;

        WantMask = forcedMask;
        int RandomMessageGroup = Random.Range(0, WantMask.MessageGroups.Count);
        Messages = WantMask.MessageGroups[RandomMessageGroup].Messages;
        indexMessage = 0;
    }

    public string NextMessage()
    {
        if (indexMessage < Messages.Count)
        {
            string message = Messages[indexMessage];
            indexMessage++;
            return message;
        }
        else
        {
            return null;
        }
    }
    //helper
    public MaskSO GetWantedMask()
    {
        return WantMask;
    }
}

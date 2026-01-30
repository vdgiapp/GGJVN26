using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] private MaskSO WantMask;
    int indexMessage = 0;
    public void Initialize(MaskSO wantedMask)
    {
        WantMask = wantedMask;
        indexMessage = 0;
    }
    public string NextMessage()
    {
        if (indexMessage < WantMask.Messages.Count) indexMessage++;
        return WantMask.Messages[indexMessage - 1];
    }
    //helper
    public MaskSO GetWantedMask()
    {
        return WantMask;
    }
}

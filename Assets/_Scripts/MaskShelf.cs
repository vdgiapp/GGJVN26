using UnityEngine;

public class MaskShelf : MonoBehaviour
{
    public MaskSO maskData;
    public bool CheckForMaskName(string maskId)
    {
        return maskData.maskId == maskId;
    }
}
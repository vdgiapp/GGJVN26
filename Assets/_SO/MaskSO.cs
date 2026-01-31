using UnityEngine;

[CreateAssetMenu(fileName = "MaskData", menuName = "Scriptable Objects/Mask Data", order = 1)]
public class MaskSO : ScriptableObject
{
    public string maskId;
    public string maskTooltip;
    public GameObject maskPrefab;
    public bool isLimited; // lấy thì sẽ mất luôn
}
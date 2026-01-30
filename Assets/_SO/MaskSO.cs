using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaskSO", menuName = "Scriptable Objects/MaskSO")]
public class MaskSO : ScriptableObject
{
    [Header("Info")]
    public string maskName;

    [Header("Clues")]
    [TextArea(2, 4)]
    public List<string> Messages;

}

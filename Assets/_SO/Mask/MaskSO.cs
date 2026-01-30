using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaskSO", menuName = "Scriptable Objects/MaskSO")]
public class MaskSO : ScriptableObject
{
    [Header("Info")]
    public string maskName;

    [Header("Clues (groups of messages)")]
    public List<MessageGroup> MessageGroups;
}

[System.Serializable]
public class MessageGroup
{
    [TextArea(2, 4)]
    public List<string> Messages;
}


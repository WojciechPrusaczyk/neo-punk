using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogLine
{
    public Talkers speakerName;
    public string text;
}
public enum Talkers
{
    CharacterOne,
    CharacterTwo
}

[CreateAssetMenu(fileName = "NewDialogData", menuName = "Dialog/DialogData")]
public class DialogData : ScriptableObject
{
    public string characterOneName;
    public Sprite characterOnePicture;
    public string characterTwoName;
    public Sprite characterTwoPicture;
    public List<DialogLine> lines = new List<DialogLine>();
    public bool repeatable = false;
    public bool hasBeenAlreadySeen = false;
}
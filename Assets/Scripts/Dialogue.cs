using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerImage; 
    public string sentence;     
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines; 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMonologueTrigger : MonoBehaviour
{
    public Dialogue playerMonologue;  

    void Start()
    {
        // Invoke the monologue after 2 seconds
        Invoke("TriggerPlayerMonologue", 2f);  
    }

    public void TriggerPlayerMonologue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(playerMonologue);
        Debug.Log("starting dialog");
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager; // Reference to DialogueManager
    public Dialogue dialogue;
    private bool isPlayerInRange = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in range to talk to NPC.");
            isPlayerInRange = true;  // Set flag to true when player enters the NPC's trigger range
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the NPC range.");
            isPlayerInRange = false;  // Reset flag when player leaves the trigger range
        }
    }

    void Update()
    {
        // Check if player is in range and E key is pressed
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed, starting dialogue.");
            TriggerDialogue();  // Call the method to trigger dialogue
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);  // Ensure DialogueManager is properly set
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueUI; 
    [SerializeField] private GameObject monologueUI; // UI for player monologue
    [SerializeField] public TextMeshProUGUI nameText;  
    [SerializeField] public TextMeshProUGUI monologueText;  // Text to display the monologue
    [SerializeField] private Image speakerImageUI;
    [SerializeField] public TextMeshProUGUI dialogueText; 

    public Animator animator;

    private Queue<DialogueLine> dialogueLines;  
    private bool isDialogueActive = false;  // Flag to track if a dialogue is currently active

    void Start()
    {
        dialogueLines = new Queue<DialogueLine>();

        // Initially hide both the dialogue and monologue UI
        dialogueUI.SetActive(false);
        monologueUI.SetActive(false);  
    }

    public void StartDialogue(Dialogue dialogue, bool isMonologue = false)
    {
        // Prevent starting a new dialogue if one is already active
        if (isDialogueActive) return;  

        // Clear previous lines and enqueue the new dialogue lines
        dialogueLines.Clear();
        foreach (DialogueLine line in dialogue.dialogueLines)
        {
            dialogueLines.Enqueue(line);
        }

        isDialogueActive = true;


        // Check if it's a monologue or an NPC dialogue
        if (isMonologue)
        {
            monologueUI.SetActive(true);  // Show the monologue UI
            DisplayMonologue();  // Display the first monologue line
        }
        else
        {
            dialogueUI.SetActive(true);  // Show NPC dialogue UI
            animator.SetBool("IsOpen", true);
            DisplayNextSentence();  // Display the first sentence
        }
    }

    // Displays the next line for the player monologue
    void DisplayMonologue()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();  // End dialogue when there are no more lines
            return;
        }

        // Get the next line and set it to the monologue text
        DialogueLine line = dialogueLines.Dequeue();
        DisplayNextSentence();
        // monologueText.text = line.sentence;

        // // Start typing the monologue text one letter at a time
        // StopAllCoroutines();
        // StartCoroutine(TypeMonologue(line.sentence));
    }

    // Coroutine for typing out the monologue text letter by letter
    IEnumerator TypeMonologue(string sentence)
    {
        monologueText.text = "";  // Clear the monologue text
        foreach (char letter in sentence.ToCharArray())
        {
            monologueText.text += letter;  // Add one letter at a time
            yield return null;
        }
    }

    // Displays the next sentence for NPC dialogue
    public void DisplayNextSentence()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();  // End dialogue when all lines are displayed
            return;
        }

        // Get the next dialogue line and update the UI elements
        DialogueLine line = dialogueLines.Dequeue();
        nameText.text = line.speakerName;

        // Set the speaker's image if it's provided
        if (line.speakerImage != null)
        {
            speakerImageUI.sprite = line.speakerImage;
        }

        // Start typing the dialogue sentence letter by letter
        StopAllCoroutines();
        StartCoroutine(TypeSentence(line.sentence));
    }

    // Coroutine for typing out the dialogue text letter by letter
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";  // Clear the dialogue text
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;  // Add one letter at a time
            yield return null;
        }
    }

    // Ends the dialogue, hides the UI, and resets the dialogue state
    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        dialogueUI.SetActive(false);  // Hide the dialogue UI
        monologueUI.SetActive(false);  // Hide the monologue UI
        isDialogueActive = false;  // Reset dialogue state to allow new dialogues
        
        FindObjectOfType<Player>().SetMovement(true); // Enable player movement
    }
}

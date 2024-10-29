using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class startDialogue : MonoBehaviour
{
    public GameObject dialogue;
    public TextMeshProUGUI text;
    public string[] lines;
    public float textSpeed;

    private int index;

    public int[] marcusLines;

    public string mainScene;

    public GameObject Marcus;
    public GameObject Livia;

    public AudioClip next;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Marcus.SetActive(true);
        Livia.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        text.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (text.text == lines[index])
            {
                NextLine();
            } else {
                StopAllCoroutines();
                text.text = lines[index];
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player")
        {
            dialogue.SetActive(true);
            collision.gameObject.GetComponent<Movement>().enabled = false;
            StartDialogue();
        }

    }

    void StartDialogue() {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine() {
        foreach (char c in lines[index].ToCharArray())
        {
            Debug.Log(c);
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine() {
        if (index < lines.Length - 1) {
            index++;
            text.text = string.Empty;
            if (marcusLines.Contains(index)) {
                Marcus.SetActive(true);
                Livia.SetActive(false);
            } else {
                Livia.SetActive(true);
                Marcus.SetActive(false);
            }
            PlaySound(next);
            StartCoroutine(TypeLine());
        } else {
            SceneManager.LoadScene(mainScene);
            gameObject.SetActive(false);
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

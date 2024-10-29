using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string[] lines;
    public float textSpeed;

    private int index;

    public GameObject demo;

    public int[] marcusLines;

    public GameObject Marcus;
    public GameObject Livia;
    private AudioSource audioSource;
    public AudioClip next;
    // Start is called before the first frame update
    void Start()
    {
        Marcus.SetActive(false);
        Livia.SetActive(true);
        text.text = string.Empty;
        audioSource = GetComponent<AudioSource>();
        StartDialogue();
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
            gameObject.SetActive(false);
            demo.SetActive(true);
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

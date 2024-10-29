using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // An array to hold the different tutorial pages
    public GameObject[] tutorialPages;

    // Index to track the current page
    private int currentPageIndex = 0;
    public GameObject deathScreenUI;

    // Buttons for navigating through the tutorial
    public Button nextButton;
    public Button finishButton;

    public GameObject player;
    public GameObject minotaur;

    private Vector3 playerStartPosition;
    private Vector3 minotaurStartPosition;

    void Start()
    {
        playerStartPosition = player.transform.position;
        minotaurStartPosition = minotaur.transform.position;
        // Start with the first tutorial page and hide the others
        ShowPage(0);

        // Make sure time is paused at the start of the tutorial
        Time.timeScale = 0;

        // Set up the buttons
        nextButton.onClick.AddListener(NextPage);
        finishButton.onClick.AddListener(FinishTutorial);

        // Only show the next button initially
        nextButton.gameObject.SetActive(true);
        finishButton.gameObject.SetActive(false);
       
    }

    void ShowPage(int pageIndex)
    {
        // Hide all pages
        foreach (GameObject page in tutorialPages)
        {
            page.SetActive(false);
        }

        // Show the current page
        tutorialPages[pageIndex].SetActive(true);
    }

    public void NextPage()
    {
        currentPageIndex++;

        // If we've reached the last page, show the finish button instead of the next button
        if (currentPageIndex >= tutorialPages.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            finishButton.gameObject.SetActive(true);
        }

        // Show the new current page
        ShowPage(currentPageIndex);
    }

    public void FinishTutorial()
    {
        // Hide the tutorial
        foreach (GameObject page in tutorialPages)
        {
            page.SetActive(false);
        }
        finishButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // Resume the game by setting time back to normal
        Time.timeScale = 1;
    }

    public void DeathHandler() {
        deathScreenUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

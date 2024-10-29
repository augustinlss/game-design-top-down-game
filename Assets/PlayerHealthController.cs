using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthController : MonoBehaviour
{
    public float maxHealth = 100f; 
    public float currentHealth;
    public int maxPotions;
    public int currentPotions;

    public Animator potionAnim;
    public TMP_Text potionCountText;

    public Slider healthBar;

    // Audio sources for different actions
    public AudioClip healSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth; 
        currentPotions = maxPotions;
        potionCountText.text = currentPotions.ToString();
        
        // Initialize AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S) && currentPotions > 0 && currentHealth < maxHealth) 
        {
            currentHealth += 25;
            currentHealth = Mathf.Min(currentHealth, maxHealth); // Ensure health doesn't exceed maxHealth
            healthBar.value = currentHealth;

            currentPotions -= 1;
            potionCountText.text = currentPotions.ToString();

            // Play heal sound
            PlaySound(healSound);
        } 
        else if (Input.GetKeyDown(KeyCode.S) && currentPotions == 0) 
        {
            potionAnim.Play("OutOfPotions");
        }
    }

    public void resetHealth() {
        currentHealth = maxHealth;
        healthBar.value = currentHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Player took damage! Current health: " + currentHealth);
        
        // Play damage sound
        PlaySound(damageSound);

        if (currentHealth > 0) 
        {
            healthBar.value = currentHealth;
        } 
        else 
        {
            healthBar.value = 0;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");

        // Play death sound
        PlaySound(deathSound);

        transform.GetComponent<PlayerController>().Die();
    }

    // Method to play any sound
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

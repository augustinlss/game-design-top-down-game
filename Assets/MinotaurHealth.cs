using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinotaurHealth : MonoBehaviour
{
    public float maxHealth = 100f; 
    public float currentHealth;

    public Animator anim;

    public Slider healthBar;
    public GameObject playerObject;

    public GameObject dialogue;

    private void Start()
    {
        dialogue.SetActive(false);
        currentHealth = maxHealth; 
    }

    public void resetHealth() {
        currentHealth = 100;
        healthBar.value = currentHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Minotaur took damage! Current health: " + currentHealth);

        if (currentHealth > 0) {
            healthBar.value = currentHealth;
        } else {
            healthBar.value = 0;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.Play("Minotaur_death");
        transform.GetComponent<MinotaurController>().StopALL();
        playerObject.GetComponent<PlayerController>().StopALL();
        playerObject.GetComponent<PlayerController>().enabled = false;
        playerObject.GetComponent<Animator>().Play("idle");
        transform.GetComponent<MinotaurController>().enabled = false;
        Debug.Log("Minotaur died!");
        dialogue.SetActive(true);
        
    }

}

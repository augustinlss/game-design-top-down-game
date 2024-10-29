using System.Collections;
using UnityEngine;
using Cinemachine;

public class DashStrikeCollider : MonoBehaviour
{
    public GameObject dashStrikeParticleEffect;
    public bool isDashing = false;

    public CinemachineVirtualCamera cam;

    public float dashStrikeDamage;

    public CameraShake cameraShake; // Reference to the camera shake script

    public float shakeDuration = 0.2f; // Duration of the camera shake
    public float shakeMagnitude = 0.3f; // Magnitude of the shake

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Detected");

        // Instantiate the particle effect at the collision point
        if (dashStrikeParticleEffect != null && collision.gameObject.tag == "IgnoreParticles" && isDashing)
        {
            Instantiate(dashStrikeParticleEffect, collision.transform.position, Quaternion.identity);
            cam.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
            collision.gameObject.GetComponent<MinotaurHealth>().TakeDamage(dashStrikeDamage);
            StartCoroutine(PauseTime());
        }

    }

    private IEnumerator PauseTime()
    {
        // Store the original time scale
        float originalTimeScale = Time.timeScale;
        yield return new WaitForSecondsRealtime(0.07f);

        // Set the time scale to 0, effectively pausing the game
        Time.timeScale = 0f;

        // Wait for the specified pause duration
        yield return new WaitForSecondsRealtime(0.5f);

        // Restore the original time scale
        Time.timeScale = originalTimeScale;
    }
}

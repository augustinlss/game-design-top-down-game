using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform player; // Reference to the player (or parent object)
    public float offset = 0f; // Angle offset if needed for adjusting weapon orientation
    public Animator swordAnimator;
    private bool isAttacking = false;
    void Update()
    {
        if (!isAttacking) {
            RotateWeaponTowardsMouse();
        }
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformSwordAttack());
        }
    }

    void RotateWeaponTowardsMouse()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Since we're working in 2D, z is set to 0

        // Get the difference between the weapon/player position and the mouse position
        Vector3 direction = mousePosition - player.position;

        // Calculate the angle between the player and the mouse position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert radians to degrees

        if (player.localScale.x > 0) {
            // Apply the rotation to the weapon (with optional offset if needed)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + offset));
            if (angle > 90 || angle < -90) // When the weapon is pointing left
            {
                transform.localScale = new Vector3(0.12f, -0.1f, 1); // Flip vertically
            }
            else // When the weapon is pointing right
            {
                transform.localScale = new Vector3(0.12f, 0.1f, 1); // Default scale
            }

        } else {
            // Apply the rotation to the weapon (with optional offset if needed)
            transform.rotation = Quaternion.Euler(new Vector3(180, 0, 180 - angle + offset));
            if (angle > 90 || angle < -90) // When the weapon is pointing left
            {
                transform.localScale = new Vector3(0.12f, 0.1f, 1); // Flip vertically
            }
            else // When the weapon is pointing right
            {
                transform.localScale = new Vector3(0.12f, -0.1f, 1); // Default scale
            }
        }

        // Optionally, you can flip the weapon sprite if needed (if your sprite faces one direction only)
        if (angle > 90 || angle < -90) // When the weapon is pointing left
        {
            transform.localScale = new Vector3(0.12f, -0.1f, 1); // Flip vertically
        }
        else // When the weapon is pointing right
        {
            transform.localScale = new Vector3(0.12f, 0.1f, 1); // Default scale
        }
    }

    private IEnumerator PerformSwordAttack()
    {
        isAttacking = true;

        // Trigger the sword attack animation in the Animator
        swordAnimator.SetBool("isAttacking", true);
        // Wait for the duration of the sword animation
        float attackDuration = swordAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(attackDuration);

        swordAnimator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(0.45f);
        isAttacking = false;
    }
}

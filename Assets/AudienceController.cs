using UnityEngine;

public class AudienceController : MonoBehaviour
{
    public Animator audienceAnimator;

    void Start()
    {
        audienceAnimator = GetComponent<Animator>();
    }

    // Call this method to make the audience jump
    public void TriggerJump()
    {
        audienceAnimator.SetBool("isJumping", true);
        Debug.Log("Jump Triggered");
    }

    // call this method to make them stop jumping

    public void StopJump()
    {
        audienceAnimator.SetBool("isJumping", true);
        Debug.Log("Jump Stopped");
    }
}
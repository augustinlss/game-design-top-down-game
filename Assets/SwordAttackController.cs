using UnityEngine;

public class SwordAttackController : MonoBehaviour
{
    public Animator swordAnimator;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            PerformSwordAttack();
        }
    }

    void PerformSwordAttack()
    {
        isAttacking = true;

        swordAnimator.SetBool("isAttacking", true);

        float attackDuration = swordAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Invoke("ResetAttack", attackDuration);
    }

    void ResetAttack()
    {
        isAttacking = false;
        swordAnimator.SetBool("isAttacking", false);
    }
}

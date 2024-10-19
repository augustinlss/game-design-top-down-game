using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;

        private Animator animator;
        private bool canMove = true;  // Flag to control movement

        private void Start()
        {
            animator = GetComponent<Animator>();
        }


        private void Update()
        {
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
            }

            dir.Normalize();
            animator.SetBool("isWalking", dir.magnitude > 0);

            GetComponent<Rigidbody2D>().velocity = speed * dir;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("NPC"))
            {
                // Logic for what happens when colliding with an NPC
                Debug.Log("Collided with NPC!");
                // You can also disable movement or trigger a dialogue here
                // SetMovement(false);
            }
        }
}


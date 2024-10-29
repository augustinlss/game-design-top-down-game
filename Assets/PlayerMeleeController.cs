using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeController : MonoBehaviour
{

    public float meleeDamage;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "IgnoreParticles") {
            collision.gameObject.GetComponent<MinotaurHealth>().TakeDamage(meleeDamage);
        }
    }
}

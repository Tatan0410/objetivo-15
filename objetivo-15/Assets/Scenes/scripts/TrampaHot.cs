using UnityEngine;
using System.Collections;

public class TrampaHot : MonoBehaviour
{
    [Header("TrampaHot timers")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activateTime;

    private Animator anim;
    private SpriteRenderer spriteRend;
    private bool triggered;
    private bool active;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!triggered)
            StartCoroutine(ActivateTrampaHot());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!active) return;

        MuerteJugador muerte = collision.GetComponent<MuerteJugador>();
        if (muerte != null)
            muerte.MorirPorEnemigo();
    }

    private IEnumerator ActivateTrampaHot()
    {
        triggered = true;
        spriteRend.color = Color.red;
        yield return new WaitForSeconds(activationDelay);
        active = true;
        spriteRend.color = Color.white;
        anim.SetBool("activado", true);
        yield return new WaitForSeconds(activateTime);
        active = false;
        triggered = false;
        anim.SetBool("activado", false);
    }
}
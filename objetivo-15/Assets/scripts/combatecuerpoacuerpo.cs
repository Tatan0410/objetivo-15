using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    private const string STRING_ANIMACION_ATAQUE = "Atacar";

    [Header("Referencias")]
    [SerializeField] private Animator animator;

    [Header("Ataque")]
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private int danioAtaque;
    [SerializeField] private float tiempoEntreAtaques;

    private float tiempoUltimoAtaque = -Mathf.Infinity; // permite atacar desde el primer frame

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            IntentarAtacar();
        }
    }

    private void IntentarAtacar()
    {
        if (Time.time < tiempoUltimoAtaque + tiempoEntreAtaques) { return; }
        Atacar();
    }

    private void Atacar()
    {
        if (animator != null)
            animator.SetTrigger(STRING_ANIMACION_ATAQUE);

        tiempoUltimoAtaque = Time.time;

        if (controladorAtaque == null) return;

        Collider2D[] objetosTocados = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D objeto in objetosTocados)
        {
            Enemigo enemigo = objeto.GetComponent<Enemigo>();
            if (enemigo != null)
                enemigo.RecibirDanio(danioAtaque);

            EnemigoVolador volador = objeto.GetComponent<EnemigoVolador>();
            if (volador != null)
                volador.RecibirDanio(danioAtaque);

            rata enemigoRata = objeto.GetComponent<rata>();
            if (enemigoRata != null)
                enemigoRata.RecibirDanio(danioAtaque);
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorAtaque == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
    }
}
using UnityEngine;

public class CombateJugador : MonoBehaviour
{
    private const string STRING_ANIMACION_ATAQUE = "Atacar";

    [Header("Referencias")]
    [SerializeField] private Animator animator;

    [Header("Ataque")]
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private int dañoAtaque;
    [SerializeField] private float tiempoEntreAtaques;

    private float tiempoUltimoAtaque = -Mathf.Infinity; // permite atacar desde el primer frame

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("CombateJugador: se presionó Fire1");
            IntentarAtacar();
        }
    }

    private void IntentarAtacar()
    {
        if (Time.time < tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            Debug.Log("CombateJugador: en cooldown, no ataca todavía");
            return;
        }
        Atacar();
    }

    private void Atacar()
    {
        Debug.Log("CombateJugador: ejecutando Atacar(), animator=" + (animator != null));

        if (animator != null)
            animator.SetTrigger(STRING_ANIMACION_ATAQUE);

        tiempoUltimoAtaque = Time.time;

        if (controladorAtaque == null) return;

        Collider2D[] objetosTocados = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D objeto in objetosTocados)
        {
            Enemigo enemigo = objeto.GetComponentInParent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.RecibirDanio(dañoAtaque);
                continue;
            }

            EnemigoVolador volador = objeto.GetComponentInParent<EnemigoVolador>();
            if (volador != null)
            {
                volador.RecibirDanio(dañoAtaque);
                continue;
            }

            rata enemigoRata = objeto.GetComponentInParent<rata>();
            if (enemigoRata != null)
            {
                enemigoRata.RecibirDanio(dañoAtaque);
                continue;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorAtaque == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
    }
}

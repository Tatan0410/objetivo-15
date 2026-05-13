using UnityEngine;
using System.Collections;

public class CajaSorpresa : MonoBehaviour
{
    [Header("Power-ups posibles")]
    public GameObject[] prefabsPowerUp;

    [Header("Tipo de caja")]
    public bool cajaEnSuelo = false; // false = aérea (golpe cabeza)
                                     // true  = en suelo (pisarla)
    [Header("Apariencia")]
    public Sprite spriteActiva;      // sprite normal (dorado/?)
    public Sprite spriteUsada;       // sprite después de golpear (gris)
    public Color colorActiva = new Color(1f, 0.85f, 0f); // dorado
    public Color colorUsada = new Color(0.4f, 0.4f, 0.4f); // gris

    [Header("Animación de golpe")]
    public float alturaRebote = 0.3f;
    public float velocidadRebote = 8f;

    private bool usada = false;
    private SpriteRenderer sr;
    private Vector3 posOriginal;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        posOriginal = transform.position;

        if (sr)
        {
            if (spriteActiva != null) sr.sprite = spriteActiva;
            sr.color = colorActiva;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (usada) return;
        if (!col.collider.CompareTag("Player")) return;

        // Detecta dirección del golpe por la normal del contacto
        foreach (ContactPoint2D contacto in col.contacts)
        {
            if (!cajaEnSuelo)
            {
                // Caja AÉREA: el jugador golpea desde ABAJO
                // normal.y < -0.5 significa que el golpe viene de abajo
                if (contacto.normal.y < -0.5f)
                {
                    Activar();
                    break;
                }
            }
            else
            {
                // Caja en SUELO: el jugador la pisa desde ARRIBA
                // normal.y > 0.5 significa que el golpe viene de arriba
                if (contacto.normal.y > 0.5f)
                {
                    Activar();
                    break;
                }
            }
        }
    }

    void Activar()
    {
        usada = true;
        SoltarPowerUp();
        StartCoroutine(AnimacionGolpe());
        CambiarApariencia();
    }

    void SoltarPowerUp()
    {
        if (prefabsPowerUp.Length == 0)
        {
            Debug.LogWarning("CajaSorpresa: no hay power-ups asignados");
            return;
        }

        int idx = Random.Range(0, prefabsPowerUp.Length);

        // Suelta el power-up arriba de la caja
        Vector3 posSpawn = transform.position + Vector3.up * 1.2f;
        Instantiate(prefabsPowerUp[idx], posSpawn, Quaternion.identity);
    }

    void CambiarApariencia()
    {
        if (!sr) return;
        if (spriteUsada != null) sr.sprite = spriteUsada;
        sr.color = colorUsada;
    }

    IEnumerator AnimacionGolpe()
    {
        // Sube
        float tiempo = 0f;
        while (tiempo < 0.1f)
        {
            transform.position = posOriginal +
                Vector3.up * Mathf.Sin(tiempo * velocidadRebote) * alturaRebote;
            tiempo += Time.deltaTime;
            yield return null;
        }
        // Baja de vuelta
        transform.position = posOriginal;
    }
}
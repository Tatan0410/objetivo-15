using UnityEngine;

public class NPCSaltoFondo : MonoBehaviour
{
    [Header("Salto")]
    public float alturaSalto = 0.8f;
    public float duracionSalto = 0.8f;
    public float pausaEntreSaltos = 2f;

    [Header("Sprites")]
    public Sprite spriteIdle;
    public Sprite spriteSalto;

    private Vector3 posicionBase;
    private float temporizador;
    private bool saltando = false;
    private float tiempoSalto;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            sr = gameObject.AddComponent<SpriteRenderer>();

        if (spriteIdle != null)
            sr.sprite = spriteIdle;

        posicionBase = transform.position;
        temporizador = Random.Range(0f, pausaEntreSaltos);
    }

    void Update()
    {
        if (!saltando)
        {
            temporizador -= Time.deltaTime;
            if (temporizador <= 0f)
            {
                saltando = true;
                tiempoSalto = 0f;
                if (spriteSalto != null)
                    sr.sprite = spriteSalto;
            }
        }
        else
        {
            tiempoSalto += Time.deltaTime;
            float t = tiempoSalto / duracionSalto;

            if (t >= 1f)
            {
                saltando = false;
                transform.position = posicionBase;
                if (spriteIdle != null)
                    sr.sprite = spriteIdle;
                temporizador = pausaEntreSaltos + Random.Range(0f, 0.5f);
            }
            else
            {
                float y = 4f * alturaSalto * t * (1f - t);
                transform.position = posicionBase + new Vector3(0, y, 0);
            }
        }
    }
}

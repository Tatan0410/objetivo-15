using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogoNPC
{
    public bool esJugador;
    [TextArea(2, 5)]
    public string texto;
}

public class NPCDialogoJRPG : MonoBehaviour
{
    [Header("Dialogo")]
    public DialogoNPC[] dialogos;

    [Header("Apariencia")]
    public Sprite retratoNPC;
    public TMP_FontAsset fontAsset;

    [Header("Deteccion")]
    public float radioDeteccion = 3f;
    public float cooldownReaparicion = 2f;

    [Header("Burbuja de dialogo")]
    public Sprite spriteNubeDialogo;
    public float alturaBurbuja = 1.8f;
    public Vector2 tamanoBurbuja = new Vector2(3f, 0.8f);

    [Header("Zoom de camara")]
    [Range(0.1f, 1f)]
    public float factorZoomDialogo = 0.8f;

    [Header("Configuracion")]
    public float velocidadTexto = 0.03f;
    public float tamanioFuente = 28f;

    private Transform player;
    private PlayerController playerController;
    private CamaraSeguidora camara;

    private GameObject burbuja;
    private Image burbujaFondo;
    private TMP_Text burbujaTexto;
    private Transform duenoBurbuja;

    private int indiceActual = 0;

    private Coroutine animTexto;
    private bool dialogando = false;
    private bool escribiendo = false;
    private bool cooldownActivo = false;
    private bool dialogoCompletado = false;
    private float cooldownRestante = 0f;

    void Start()
    {
        if (!Application.isPlaying) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerController = p.GetComponent<PlayerController>();
        }

        if (retratoNPC == null)
            retratoNPC = GetComponent<SpriteRenderer>()?.sprite;

        if (Camera.main != null)
            camara = Camera.main.GetComponent<CamaraSeguidora>();

        CrearBurbuja();
    }

    void OnDestroy()
    {
        if (burbuja != null)
            Destroy(burbuja);
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        if (player == null) return;

        if (dialogando)
        {
            if (!escribiendo && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
                AvanzarDialogo();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dialogoCompletado && dist > radioDeteccion * 1.2f)
            dialogoCompletado = false;

        if (cooldownActivo)
        {
            cooldownRestante -= Time.deltaTime;
            if (cooldownRestante <= 0f)
                cooldownActivo = false;
            return;
        }

        if (!dialogoCompletado && dist < radioDeteccion)
            IniciarDialogo();
    }

    void CrearBurbuja()
    {
        burbuja = new GameObject("BurbujaDialogo_" + gameObject.name);
        burbuja.transform.SetParent(null);
        burbuja.transform.localScale = Vector3.one * 0.005f;
        burbuja.SetActive(false);

        Canvas canvas = burbuja.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        GameObject fondo = new GameObject("Fondo");
        fondo.transform.SetParent(burbuja.transform, false);
        burbujaFondo = fondo.AddComponent<Image>();
        if (spriteNubeDialogo != null)
            burbujaFondo.sprite = spriteNubeDialogo;
        burbujaFondo.type = Image.Type.Sliced;
        burbujaFondo.color = new Color(1, 1, 1, 0.92f);
        burbujaFondo.raycastTarget = false;
        RectTransform rtFondo = burbujaFondo.GetComponent<RectTransform>();
        rtFondo.sizeDelta = tamanoBurbuja * 100f;
        rtFondo.anchoredPosition = Vector2.zero;

        GameObject txt = new GameObject("Texto");
        txt.transform.SetParent(burbuja.transform, false);
        burbujaTexto = txt.AddComponent<TextMeshProUGUI>();
        burbujaTexto.font = fontAsset;
        burbujaTexto.fontSize = tamanioFuente;
        burbujaTexto.color = Color.black;
        burbujaTexto.alignment = TextAlignmentOptions.Center;
        burbujaTexto.enableWordWrapping = true;
        burbujaTexto.overflowMode = TextOverflowModes.Overflow;
        burbujaTexto.raycastTarget = false;
        RectTransform rtTexto = burbujaTexto.GetComponent<RectTransform>();
        rtTexto.sizeDelta = new Vector2(tamanoBurbuja.x * 90f, tamanoBurbuja.y * 70f);
        rtTexto.anchoredPosition = Vector2.zero;
    }

    void IniciarDialogo()
    {
        if (dialogos == null || dialogos.Length == 0) return;

        dialogando = true;
        indiceActual = 0;

        if (playerController != null)
            playerController.DesactivarControl();

        if (camara != null)
        {
            Vector3 midpoint = (player.position + transform.position) * 0.5f;
            camara.ForzarPosicion(midpoint);
            camara.ForzarZoom(factorZoomDialogo);
        }

        MostrarDialogo(indiceActual);
    }

    void MostrarDialogo(int indice)
    {
        if (indice >= dialogos.Length) { CerrarDialogo(); return; }

        Transform target = dialogos[indice].esJugador ? player : transform;
        PosicionarBurbuja(target);

        if (animTexto != null)
            StopCoroutine(animTexto);
        animTexto = StartCoroutine(EscribirTexto(dialogos[indice].texto));
    }

    void PosicionarBurbuja(Transform target)
    {
        duenoBurbuja = target;
        burbuja.transform.position = target.position + Vector3.up * alturaBurbuja;
        burbuja.SetActive(true);
    }

    void AvanzarDialogo()
    {
        if (escribiendo)
        {
            if (animTexto != null)
                StopCoroutine(animTexto);
            burbujaTexto.text = dialogos[indiceActual].texto;
            escribiendo = false;
            return;
        }

        indiceActual++;
        MostrarDialogo(indiceActual);
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;
        burbujaTexto.text = "";

        foreach (char letra in texto)
        {
            burbujaTexto.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
        animTexto = null;
    }

    void CerrarDialogo()
    {
        dialogando = false;
        dialogoCompletado = true;

        if (animTexto != null)
        {
            StopCoroutine(animTexto);
            animTexto = null;
        }

        burbuja.SetActive(false);

        camara?.RestaurarPosicion();
        camara?.RestaurarZoom();

        if (playerController != null)
            playerController.ActivarControl();

        cooldownActivo = true;
        cooldownRestante = cooldownReaparicion;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}

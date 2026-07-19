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

    [Header("Barras de cine")]
    public float altoBarra = 200f;
    public float altoAreaDialogo = 300f;

    [Header("Retratos")]
    public float tamanoRetrato = 150f;
    public float margenRetratoLateral = 40f;
    public float margenRetratoAbajo = 30f;

    [Header("Texto")]
    public float margenTextoJugador = 60f;
    public float margenTextoNPC = 60f;
    public float posYTexto = 170f;
    public float anchoTextoJugador = 850f;
    public float anchoTextoNPC = 850f;
    public float altoTexto = 200f;

    [Header("Configuracion")]
    public float velocidadTexto = 0.03f;
    public float tamanioFuente = 25f;

    [Header("Previsualizacion (solo Editor)")]
    public bool previsualizar = false;

    private Transform player;
    private PlayerController playerController;
    private Canvas canvas;

    private Image retratoJugadorImg;
    private Image retratoNPCImg;
    private RectTransform barraSuperior;
    private RectTransform barraInferior;
    private TMP_Text textoDialogo;
    private GameObject indicadorAvance;
    private int indiceActual = 0;

    private Coroutine animTexto;
    private bool dialogando = false;
    private bool escribiendo = false;
    private bool cooldownActivo = false;
    private bool dialogoCompletado = false;
    private float cooldownRestante = 0f;

    void OnValidate()
    {
        if (Application.isPlaying) return;
        if (!gameObject.activeInHierarchy) return;

        if (previsualizar && canvas == null)
            CrearCanvas(true);
        else if (!previsualizar && canvas != null)
            DestruirCanvas();
    }

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

        CrearCanvas(true);
    }

    void OnDestroy()
    {
        if (canvas != null)
            DestruirCanvas();
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

    void IniciarDialogo()
    {
        if (dialogos == null || dialogos.Length == 0) return;

        dialogando = true;
        indiceActual = 0;

        if (playerController != null)
            playerController.DesactivarControl();

        canvas.gameObject.SetActive(true);

        if (retratoJugadorImg != null && player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
                retratoJugadorImg.sprite = sr.sprite;
        }

        if (retratoNPCImg != null && retratoNPC != null)
            retratoNPCImg.sprite = retratoNPC;

        MostrarDialogo(indiceActual);
    }

    void MostrarDialogo(int indice)
    {
        if (indice >= dialogos.Length) { CerrarDialogo(); return; }

        PosicionarTexto(dialogos[indice].esJugador);

        if (animTexto != null)
            StopCoroutine(animTexto);
        animTexto = StartCoroutine(EscribirTexto(dialogos[indice].texto));
    }

    void PosicionarTexto(bool esJugador)
    {
        if (textoDialogo == null) return;

        textoDialogo.alignment = esJugador ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        RectTransform rt = textoDialogo.GetComponent<RectTransform>();

        if (esJugador)
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0.5f);
            rt.anchoredPosition = new Vector2(margenTextoJugador, posYTexto);
            rt.sizeDelta = new Vector2(anchoTextoJugador, altoTexto);
        }
        else
        {
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0.5f);
            rt.anchoredPosition = new Vector2(-margenTextoNPC, posYTexto);
            rt.sizeDelta = new Vector2(anchoTextoNPC, altoTexto);
        }
    }

    void AvanzarDialogo()
    {
        if (escribiendo)
        {
            if (animTexto != null)
                StopCoroutine(animTexto);
            textoDialogo.text = dialogos[indiceActual].texto;
            escribiendo = false;
            MostrarIndicadorAvance(true);
            return;
        }

        indiceActual++;
        MostrarDialogo(indiceActual);
    }

    IEnumerator EscribirTexto(string texto)
    {
        escribiendo = true;
        MostrarIndicadorAvance(false);
        textoDialogo.text = "";

        foreach (char letra in texto)
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadTexto);
        }

        escribiendo = false;
        MostrarIndicadorAvance(true);
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

        canvas.gameObject.SetActive(false);

        if (playerController != null)
            playerController.ActivarControl();

        cooldownActivo = true;
        cooldownRestante = cooldownReaparicion;
    }

    void MostrarIndicadorAvance(bool visible)
    {
        if (indicadorAvance != null)
            indicadorAvance.SetActive(visible);
    }

    void DestruirCanvas()
    {
        if (canvas == null) return;
        if (Application.isPlaying)
            Destroy(canvas.gameObject);
        else
            DestroyImmediate(canvas.gameObject);
        canvas = null;
        retratoJugadorImg = null;
        retratoNPCImg = null;
        barraSuperior = null;
        barraInferior = null;
        textoDialogo = null;
        indicadorAvance = null;
    }

    #region Creacion de UI

    void CrearCanvas(bool conTexto)
    {
        GameObject go = new GameObject("Canvas_DialogoJRPG_" + gameObject.name);
        go.transform.SetParent(transform);
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();

        if (!Application.isPlaying)
        {
            go.hideFlags = HideFlags.DontSave;
            go.SetActive(true);
        }
        else
        {
            go.hideFlags = HideFlags.None;
            go.SetActive(false);
            AsignarFont();
        }

        CrearBarras(go.transform);
        CrearRetratos(go.transform);

        if (conTexto)
            CrearTexto(go.transform);
    }

    void AsignarFont()
    {
        if (fontAsset == null)
        {
            TMP_FontAsset[] fonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            if (fonts.Length > 0) fontAsset = fonts[0];
        }
    }

    GameObject CrearImagen(Transform parent, string nombre, Color color)
    {
        GameObject img = new GameObject(nombre);
        img.transform.SetParent(parent, false);
        Image i = img.AddComponent<Image>();
        i.color = color;
        i.raycastTarget = false;
        return img;
    }

    void CrearBarras(Transform parent)
    {
        GameObject sup = CrearImagen(parent, "BarraSuperior", Color.black);
        barraSuperior = sup.GetComponent<RectTransform>();
        barraSuperior.anchorMin = new Vector2(0, 1);
        barraSuperior.anchorMax = new Vector2(1, 1);
        barraSuperior.pivot = new Vector2(0.5f, 1);
        barraSuperior.sizeDelta = new Vector2(0, altoBarra);

        GameObject inf = CrearImagen(parent, "BarraInferior", Color.black);
        barraInferior = inf.GetComponent<RectTransform>();
        barraInferior.anchorMin = new Vector2(0, 0);
        barraInferior.anchorMax = new Vector2(1, 0);
        barraInferior.pivot = new Vector2(0.5f, 0);
        barraInferior.sizeDelta = new Vector2(0, altoAreaDialogo);
    }

    void CrearRetratos(Transform parent)
    {
        GameObject jug = CrearImagen(parent, "RetratoJugador", Color.white);
        retratoJugadorImg = jug.GetComponent<Image>();
        RectTransform rtJ = jug.GetComponent<RectTransform>();
        rtJ.anchorMin = new Vector2(0, 0);
        rtJ.anchorMax = new Vector2(0, 0);
        rtJ.pivot = new Vector2(0, 0);
        rtJ.anchoredPosition = new Vector2(margenRetratoLateral, margenRetratoAbajo);
        rtJ.sizeDelta = new Vector2(tamanoRetrato, tamanoRetrato);

        GameObject npc = CrearImagen(parent, "RetratoNPC", Color.white);
        retratoNPCImg = npc.GetComponent<Image>();
        RectTransform rtN = npc.GetComponent<RectTransform>();
        rtN.anchorMin = new Vector2(1, 0);
        rtN.anchorMax = new Vector2(1, 0);
        rtN.pivot = new Vector2(1, 0);
        rtN.anchoredPosition = new Vector2(-margenRetratoLateral, margenRetratoAbajo);
        rtN.sizeDelta = new Vector2(tamanoRetrato, tamanoRetrato);
    }

    void CrearTexto(Transform parent)
    {
        GameObject txt = new GameObject("TextoDialogo");
        txt.transform.SetParent(parent, false);
        textoDialogo = txt.AddComponent<TextMeshProUGUI>();
        textoDialogo.font = fontAsset;
        textoDialogo.fontSize = tamanioFuente;
        textoDialogo.enableAutoSizing = false;
        textoDialogo.enableWordWrapping = true;
        textoDialogo.overflowMode = TextOverflowModes.Truncate;
        textoDialogo.color = Color.white;
        textoDialogo.raycastTarget = false;

        RectTransform rt = textoDialogo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0.5f);
        rt.anchoredPosition = new Vector2(margenTextoJugador, posYTexto);
        rt.sizeDelta = new Vector2(anchoTextoJugador, altoTexto);

        GameObject indicador = new GameObject("IndicadorAvance");
        indicador.transform.SetParent(parent, false);
        indicadorAvance = indicador;
        TextMeshProUGUI tmp = indicador.AddComponent<TextMeshProUGUI>();
        tmp.font = fontAsset;
        tmp.text = "Presiona ESPACIO  \u25B6";
        tmp.fontSize = 16;
        tmp.color = new Color(1, 1, 1, 0.7f);
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.raycastTarget = false;
        RectTransform rtI = indicador.GetComponent<RectTransform>();
        rtI.anchorMin = new Vector2(0.5f, 0);
        rtI.anchorMax = new Vector2(0.5f, 0);
        rtI.pivot = new Vector2(0.5f, 0.5f);
        rtI.anchoredPosition = new Vector2(700, 50f);
        rtI.sizeDelta = new Vector2(250, 30);
        indicador.SetActive(false);
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}

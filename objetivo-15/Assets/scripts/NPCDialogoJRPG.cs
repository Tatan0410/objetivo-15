using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPCDialogoJRPG : MonoBehaviour
{
    [Header("Dialogo")]
    public string textoNPC = "!Hola!";
    public string textoJugador = "";

    [Header("Apariencia")]
    public Sprite retratoNPC;
    public Sprite spritePanelDialogo;
    public TMP_FontAsset fontAsset;

    [Header("Deteccion")]
    public float radioDeteccion = 3f;
    public float cooldownReaparicion = 2f;

    [Header("Configuracion")]
    public float velocidadTexto = 0.03f;
    public float altoBarra = 200f;
    public float altoAreaDialogo = 300f;
    public float tamanioFuente = 25f;
    public float anchoPanelDialogo = 700f;
    public float altoPanelDialogo = 120f;

    private Transform player;
    private PlayerController playerController;
    private Canvas canvas;

    private RectTransform panelDialogo;
    private GameObject fondoBurbuja;
    private Image retratoJugadorImg;
    private Image retratoNPCImg;
    private TMP_Text textoDialogo;
    private GameObject indicadorAvance;

    private Coroutine animTexto;
    private bool dialogando = false;
    private bool escribiendo = false;
    private bool cooldownActivo = false;
    private bool dialogoCompletado = false;
    private float cooldownRestante = 0f;

    private enum Paso { PlayerText, NPCText }
    private Paso pasoActual;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerController = p.GetComponent<PlayerController>();
        }

        if (retratoNPC == null)
            retratoNPC = GetComponent<SpriteRenderer>()?.sprite;

        CrearCanvas();
    }

    void Update()
    {
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
        dialogando = true;
        pasoActual = string.IsNullOrEmpty(textoJugador) ? Paso.NPCText : Paso.PlayerText;

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

        string textoInicial = pasoActual == Paso.PlayerText ? textoJugador : textoNPC;
        MostrarTexto(textoInicial);
    }

    void AvanzarDialogo()
    {
        if (escribiendo)
        {
            if (animTexto != null)
                StopCoroutine(animTexto);
            textoDialogo.text = pasoActual == Paso.PlayerText ? textoJugador : textoNPC;
            escribiendo = false;
            MostrarIndicadorAvance(true);
            return;
        }

        switch (pasoActual)
        {
            case Paso.PlayerText:
                pasoActual = Paso.NPCText;
                MostrarTexto(textoNPC);
                break;

            case Paso.NPCText:
                CerrarDialogo();
                break;
        }
    }

    void MostrarTexto(string texto)
    {
        if (animTexto != null)
            StopCoroutine(animTexto);
        animTexto = StartCoroutine(EscribirTexto(texto));

        bool esNPC = pasoActual == Paso.NPCText;

        if (panelDialogo != null)
        {
            float offsetX = esNPC ? 350f : -350f;
            panelDialogo.anchoredPosition = new Vector2(offsetX, 170f);
        }

        if (fondoBurbuja != null)
            fondoBurbuja.transform.localScale = esNPC ? new Vector3(-1, 1, 1) : Vector3.one;
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

    #region Creacion de UI

    void CrearCanvas()
    {
        GameObject go = new GameObject("Canvas_DialogoJRPG_" + gameObject.name);
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        go.SetActive(false);

        AsignarFont();

        CrearBarras(go.transform);
        CrearRetratos(go.transform);
        CrearPanelDialogo(go.transform);
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
        RectTransform rtSup = sup.GetComponent<RectTransform>();
        rtSup.anchorMin = new Vector2(0, 1);
        rtSup.anchorMax = new Vector2(1, 1);
        rtSup.pivot = new Vector2(0.5f, 1);
        rtSup.sizeDelta = new Vector2(0, altoBarra);

        GameObject inf = CrearImagen(parent, "BarraInferior", Color.black);
        RectTransform rtInf = inf.GetComponent<RectTransform>();
        rtInf.anchorMin = new Vector2(0, 0);
        rtInf.anchorMax = new Vector2(1, 0);
        rtInf.pivot = new Vector2(0.5f, 0);
        rtInf.sizeDelta = new Vector2(0, altoAreaDialogo);
    }

    void CrearRetratos(Transform parent)
    {
        float tamano = 150f;
        float margen = 40f;
        float margenAbajo = 30f;

        GameObject jug = CrearImagen(parent, "RetratoJugador", Color.white);
        retratoJugadorImg = jug.GetComponent<Image>();
        RectTransform rtJ = jug.GetComponent<RectTransform>();
        rtJ.anchorMin = new Vector2(0, 0);
        rtJ.anchorMax = new Vector2(0, 0);
        rtJ.pivot = new Vector2(0, 0);
        rtJ.anchoredPosition = new Vector2(margen, margenAbajo);
        rtJ.sizeDelta = new Vector2(tamano, tamano);

        GameObject npc = CrearImagen(parent, "RetratoNPC", Color.white);
        retratoNPCImg = npc.GetComponent<Image>();
        RectTransform rtN = npc.GetComponent<RectTransform>();
        rtN.anchorMin = new Vector2(1, 0);
        rtN.anchorMax = new Vector2(1, 0);
        rtN.pivot = new Vector2(1, 0);
        rtN.anchoredPosition = new Vector2(-margen, margenAbajo);
        rtN.sizeDelta = new Vector2(tamano, tamano);
    }

    void CrearPanelDialogo(Transform parent)
    {
        panelDialogo = new GameObject("PanelDialogo").AddComponent<RectTransform>();
        panelDialogo.transform.SetParent(parent, false);
        panelDialogo.anchorMin = new Vector2(0.5f, 0);
        panelDialogo.anchorMax = new Vector2(0.5f, 0);
        panelDialogo.pivot = new Vector2(0.5f, 0.5f);
        panelDialogo.anchoredPosition = new Vector2(0, 170f);
        panelDialogo.sizeDelta = new Vector2(anchoPanelDialogo, altoPanelDialogo);

        fondoBurbuja = new GameObject("FondoBurbuja");
        fondoBurbuja.transform.SetParent(panelDialogo, false);
        Image img = fondoBurbuja.AddComponent<Image>();
        img.color = Color.white;
        if (spritePanelDialogo != null)
        {
            img.sprite = spritePanelDialogo;
            img.type = Image.Type.Sliced;
        }
        else
        {
            Texture2D tex = new Texture2D(4, 4);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                    tex.SetPixel(x, y, Color.white);
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            img.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100);
            img.type = Image.Type.Sliced;
            Debug.LogWarning("[NPCDialogoJRPG] spritePanelDialogo no asignado. Usando placeholder blanco.");
        }
        img.raycastTarget = false;
        RectTransform rtF = fondoBurbuja.GetComponent<RectTransform>();
        rtF.anchorMin = Vector2.zero;
        rtF.anchorMax = Vector2.one;
        rtF.offsetMin = Vector2.zero;
        rtF.offsetMax = Vector2.zero;

        GameObject contenido = new GameObject("ContenidoDialogo");
        contenido.transform.SetParent(panelDialogo, false);
        RectTransform rtC = contenido.AddComponent<RectTransform>();
        rtC.anchorMin = Vector2.zero;
        rtC.anchorMax = Vector2.one;
        rtC.offsetMin = new Vector2(25, 15);
        rtC.offsetMax = new Vector2(-25, -15);

        GameObject txt = new GameObject("TextoDialogo");
        txt.transform.SetParent(contenido.transform, false);
        textoDialogo = txt.AddComponent<TextMeshProUGUI>();
        textoDialogo.font = fontAsset;
        textoDialogo.fontSize = tamanioFuente;
        textoDialogo.enableAutoSizing = false;
        textoDialogo.enableWordWrapping = true;
        textoDialogo.overflowMode = TextOverflowModes.Overflow;
        textoDialogo.color = Color.white;
        textoDialogo.alignment = TextAlignmentOptions.Left;
        textoDialogo.raycastTarget = false;
        RectTransform rtT = txt.GetComponent<RectTransform>();
        rtT.anchorMin = Vector2.zero;
        rtT.anchorMax = Vector2.one;
        rtT.offsetMin = Vector2.zero;
        rtT.offsetMax = Vector2.zero;

        indicadorAvance = new GameObject("IndicadorAvance");
        indicadorAvance.transform.SetParent(contenido.transform, false);
        TextMeshProUGUI tmp = indicadorAvance.AddComponent<TextMeshProUGUI>();
        tmp.font = fontAsset;
        tmp.text = "Presiona ESPACIO  \u25B6";
        tmp.fontSize = 16;
        tmp.color = new Color(1, 1, 1, 0.7f);
        tmp.alignment = TextAlignmentOptions.BottomRight;
        tmp.raycastTarget = false;
        RectTransform rtI = indicadorAvance.GetComponent<RectTransform>();
        rtI.anchorMin = Vector2.zero;
        rtI.anchorMax = Vector2.one;
        rtI.offsetMin = Vector2.zero;
        rtI.offsetMax = Vector2.zero;
        indicadorAvance.SetActive(false);
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
    }
}

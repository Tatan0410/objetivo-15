using UnityEngine;
using UnityEngine.UI;

public class ColeccionablesManager : MonoBehaviour
{
    // Singleton para acceder desde cualquier script
    public static ColeccionablesManager instancia;

    [Header("Conteo de plásticos")]
    public int plasticosRecogidos = 0;
    public int plasticosTotales = 0;

    [Header("UI - arrastra el Text aquí en el Inspector")]
    public Text textoPlasticos;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Cuenta automáticamente cuántos plásticos hay en la escena
        plasticosTotales = GameObject.FindGameObjectsWithTag("Plastico").Length;
        ActualizarUI();
    }

    public void AgregarPlastico()
    {
        plasticosRecogidos++;
        ActualizarUI();

        // Si recogió todos, muestra mensaje
        if (plasticosRecogidos >= plasticosTotales)
        {
            Debug.Log("¡Nivel limpio! Recogiste todos los plásticos");
        }
    }

    void ActualizarUI()
    {
        if (textoPlasticos != null)
            textoPlasticos.text = "♻️ " + plasticosRecogidos + "/" + plasticosTotales;
    }
}
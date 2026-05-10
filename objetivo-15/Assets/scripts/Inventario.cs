using UnityEngine;
using TMPro;

public class Inventario : MonoBehaviour
{
    public static Inventario instancia;

    [Header("Cantidades")]
    public int botellasPET = 0;
    public int bolsasPlasticas = 0;
    public int tarros = 0;
    public int tubosPVC = 0;

    [Header("UI")]
    public TMP_Text textoBotella;
    public TMP_Text textoBolsa;
    public TMP_Text textoTarro;
    public TMP_Text textoTubo;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    public void AgregarPlastico(TipoPlastico tipo)
    {
        switch (tipo)
        {
            case TipoPlastico.BotellaPET:
                botellasPET++;
                break;
            case TipoPlastico.BolsaPlastica:
                bolsasPlasticas++;
                break;
            case TipoPlastico.Tarro:
                tarros++;
                break;
            case TipoPlastico.TuboPVC:
                tubosPVC++;
                break;
        }

        ActualizarUI();
        Debug.Log("Recogiste: " + tipo.ToString());
    }

    public bool TieneIngredientes(int pet, int bolsas, int t, int tubos)
    {
        return botellasPET >= pet &&
               bolsasPlasticas >= bolsas &&
               tarros >= t &&
               tubosPVC >= tubos;
    }

    public void GastarIngredientes(int pet, int bolsas, int t, int tubos)
    {
        botellasPET -= pet;
        bolsasPlasticas -= bolsas;
        tarros -= t;
        tubosPVC -= tubos;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (textoBotella) textoBotella.text = "PET: " + botellasPET;
        if (textoBolsa) textoBolsa.text = "Bolsa: " + bolsasPlasticas;
        if (textoTarro) textoTarro.text = "Tarro: " + tarros;
        if (textoTubo) textoTubo.text = "Tubo: " + tubosPVC;
    }
}
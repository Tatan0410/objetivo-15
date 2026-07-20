using UnityEngine;

public class Inventario : MonoBehaviour
{
    public static Inventario instancia;

    [Header("Cantidades")]
    public int botellasPET = 0;
    public int bolsasPlasticas = 0;
    public int icopor = 0;

    [Header("UI - Contadores")]
    public ContadorHUD contadorPET;
    public ContadorHUD contadorBolsa;
    public ContadorHUD contadorIcopor;

    void Awake()
    {
        if (instancia == null)
            instancia = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ActualizarUI();
    }

    public void AgregarPlastico(TipoPlastico tipo)
    {
        Debug.Log("AGREGANDO: " + tipo);
        switch (tipo)
        {
            case TipoPlastico.BotellaPET:
                botellasPET++;
                break;
            case TipoPlastico.BolsaPlastica:
                bolsasPlasticas++;
                break;
            case TipoPlastico.Icopor:
                icopor++;
                break;
        }

        ActualizarUI();
        Debug.Log("Recogiste: " + tipo.ToString());
    }

    public bool TieneIngredientes(int pet, int bolsas, int ico)
    {
        return botellasPET >= pet &&
               bolsasPlasticas >= bolsas &&
               icopor >= ico;
    }

    public void GastarIngredientes(int pet, int bolsas, int ico)
    {
        botellasPET -= pet;
        bolsasPlasticas -= bolsas;
        icopor -= ico;
        ActualizarUI();
    }

    void ActualizarUI()
    {
        if (contadorPET != null) contadorPET.Actualizar(botellasPET);
        if (contadorBolsa != null) contadorBolsa.Actualizar(bolsasPlasticas);
        if (contadorIcopor != null) contadorIcopor.Actualizar(icopor);
    }
}

using UnityEngine;

public class ConectarMunicionUI : MonoBehaviour
{
    void Start()
    {
        ContadorHUD contador = GetComponent<ContadorHUD>();
        if (contador != null && MunicionManager.instancia != null)
            MunicionManager.instancia.AsignarContador(contador);
    }
}

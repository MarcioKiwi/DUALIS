using UnityEngine;

public class ControleMenu : MonoBehaviour
{
    [Header("Arraste o Painel_Inventario aqui")]
    public GameObject painelInventario;

    void Update()
    {
        // Se o jogador apertar a tecla TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Inverte o estado atual (se tá ligado, desliga. Se tá desligado, liga)
            painelInventario.SetActive(!painelInventario.activeSelf);
        }
    }
}

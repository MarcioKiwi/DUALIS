using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void IniciarNovoJogo()
    {
        // 1. Formata o Memory Card (Apaga todas as memórias antigas de morte e vitórias)
        PlayerPrefs.DeleteAll();

        // 2. Carrega a primeira fase
        SceneManager.LoadScene("Zona Industrial");
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit();
    }
}

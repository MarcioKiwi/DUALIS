using UnityEngine;

public class Inimigo_Mapa : MonoBehaviour
{
    [Header("Identidade do Inimigo")]
    [Tooltip("Dê um nome único para cada inimigo do mapa (Ex: ZumbiTutorial, ZumbiElite1, etc)")]
    public string chaveDeMorte = "ZumbiTutorialMorto";

    void Start()
    {
        // Agora ele checa a sua própria chave exclusiva na memória!
        if (PlayerPrefs.GetInt(chaveDeMorte, 0) == 1)
        {
            Debug.Log(gameObject.name + " já foi derrotado. Removendo do mapa definitivamente...");
            Destroy(gameObject);
        }
    }
}
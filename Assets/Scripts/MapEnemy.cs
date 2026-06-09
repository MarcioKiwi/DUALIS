using UnityEngine;

public class InimigoMapa : MonoBehaviour
{
    void Start()
    {
        // Se o arquivo no Memory Card diz que ele morreu (é igual a 1), ele se destrói!
        if (PlayerPrefs.GetInt("ZumbiTutorialMorto", 0) == 1)
        {
            Destroy(gameObject);
        }
    }
}

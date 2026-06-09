using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro; 

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configurações")]
    public GameObject painelSettings;
    public AudioMixer meuMixer;

    [Header("Textos dos Sliders")]
    public TextMeshProUGUI textoVolMusica;  
    public TextMeshProUGUI textoVolEfeitos; 

    public void IniciarJogo()
    {
        SceneManager.LoadScene("Zona Industrial");
    }

    public void SairDoJogo()
    {
        Debug.Log("O jogo foi fechado!");
        Application.Quit();
    }

    public void AbrirSettings()
    {
        if (painelSettings != null)
            painelSettings.SetActive(true);
    }

    public void FecharSettings()
    {
        if (painelSettings != null)
            painelSettings.SetActive(false);
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("FaseSalva"))
        {
            string cenaSalva = PlayerPrefs.GetString("FaseSalva");
            SceneManager.LoadScene(cenaSalva);
        }
        else
        {
            Debug.Log("Nenhum save encontrado! Comece clicando em Play.");
        }
    }

    public void MudarVolumeMusica(float valor)
    {
        meuMixer.SetFloat("VolMusica", Mathf.Log10(valor) * 20);

        
        if (textoVolMusica != null)
        {
            int porcentagem = Mathf.RoundToInt(valor * 100);
            textoVolMusica.text = porcentagem.ToString() + "%";
        }
    }

    public void MudarVolumeEfeitos(float valor)
    {
        meuMixer.SetFloat("VolSFX", Mathf.Log10(valor) * 20);

        
        if (textoVolEfeitos != null)
        {
            int porcentagem = Mathf.RoundToInt(valor * 100);
            textoVolEfeitos.text = porcentagem.ToString() + "%";
        }
    }
}
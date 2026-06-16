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
        // 1. Limpa os dados de saves antigos (Moedas, Atributos, etc) para começar do zero
        PlayerPrefs.DeleteAll();

        // 2. Avisa o jogo que a intro AINDA NÃO foi vista nesse novo save
        PlayerPrefs.SetInt("IntroConcluida", 0);

        // 3. Manda para a Cena de Introdução (A tela preta com os textos)
        SceneManager.LoadScene("Cena_Intro");
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

    // 🟢 MUDANÇA AQUI: Botão de Carregar Jogo (Load)
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("FaseSalva"))
        {
            // Garante como medida de segurança que a caminhada automática não vai rodar de novo
            PlayerPrefs.SetInt("IntroConcluida", 1);

            // Carrega o mapa direto de onde o jogador salvou, pulando a cutscene
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
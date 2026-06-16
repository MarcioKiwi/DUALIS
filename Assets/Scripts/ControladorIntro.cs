using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ControladorIntro : MonoBehaviour
{
    [Header("História")]
    [TextArea(3, 10)]
    public string[] paragrafos;
    public float tempoPorFrase = 4f;
    public float velocidadeFade = 0.8f;

    [Header("UI")]
    public TextMeshProUGUI campoTexto;

    void Start()
    {
        campoTexto.text = "";
        StartCoroutine(ExecutarIntro());
    }

    IEnumerator ExecutarIntro()
    {
        foreach (string frase in paragrafos)
        {
            campoTexto.text = frase;

            // Fade In do Texto
            yield return StartCoroutine(FadeTexto(0, 1));

            yield return new WaitForSeconds(tempoPorFrase);

            // Fade Out do Texto
            yield return StartCoroutine(FadeTexto(1, 0));

            yield return new WaitForSeconds(1f);
        }

        // Fim da história, vai para o mapa principal
        SceneManager.LoadScene("Zona Industrial");
    }

    IEnumerator FadeTexto(float inicio, float fim)
    {
        float tempo = 0;
        while (tempo < 1)
        {
            tempo += Time.deltaTime * velocidadeFade;
            campoTexto.alpha = Mathf.Lerp(inicio, fim, tempo);
            yield return null;
        }
    }
}
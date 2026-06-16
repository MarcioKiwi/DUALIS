using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("Conexões da Interface (UI)")]
    public GameObject caixaDeDialogo; 
    public TMP_Text textoNome;
    public TMP_Text textoFala;

    private Queue<string> filaDeFalas;

    void Awake()
    {
        filaDeFalas = new Queue<string>();
        caixaDeDialogo.SetActive(false);
    }

    public void MostrarFala(string nomeDoPersonagem, string[] frasesDoNPC)
    {
        caixaDeDialogo.SetActive(true);
        textoNome.text = nomeDoPersonagem;

        filaDeFalas.Clear();

        foreach (string frase in frasesDoNPC)
        {
            filaDeFalas.Enqueue(frase);
        }

        ProximaFala();
    }

    public void ProximaFala()
    {
        if (filaDeFalas.Count == 0)
        {
            EsconderDialogo();
            return;
        }

        string fraseAtual = filaDeFalas.Dequeue();
        textoFala.text = fraseAtual;
    }

    public void EsconderDialogo()
    {
        caixaDeDialogo.SetActive(false);
        Debug.Log("Fim do diálogo.");
    }
}
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Interface Principal")]
    public GameObject caixaDeDialogo;
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoFala;

    [Header("Botões de Escolha")]
    public GameObject botao1;
    public GameObject botao2;
    public TextMeshProUGUI textoBotao1;
    public TextMeshProUGUI textoBotao2;

    // --- NOVAS VARIÁVEIS PARA O SISTEMA DE LISTA ---
    private string[] falasAtuais;
    private int indiceAtual;
    private bool dialogando = false;

    void Start()
    {
        EsconderDialogo();
    }

    void Update()
    {
        // Se a caixa está ligada, não tem botões de escolha na tela, e o jogador clicou o mouse (ou apertou Espaço/E)
        if (dialogando && caixaDeDialogo.activeSelf && !botao1.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                ProximaFala();
            }
        }
    }

    // Agora recebemos um Array (lista) de falas []
    public void MostrarFala(string nome, string[] falas)
    {
        caixaDeDialogo.SetActive(true);
        textoNome.text = nome;

        botao1.SetActive(false);
        botao2.SetActive(false);

        // Guarda a lista e zera o contador
        falasAtuais = falas;
        indiceAtual = 0;
        dialogando = true;

        // Mostra a primeira frase
        textoFala.text = falasAtuais[indiceAtual];
    }

    private void ProximaFala()
    {
        indiceAtual++; // Pula para o próximo número

        // Verifica se ainda tem frases na lista
        if (indiceAtual < falasAtuais.Length)
        {
            textoFala.text = falasAtuais[indiceAtual];
        }
        else
        {
            // Acabaram as frases! Fecha a caixa.
            EsconderDialogo();
        }
    }

    public void MostrarEscolha(string nome, string fala, string opcao1, string opcao2)
    {
        caixaDeDialogo.SetActive(true);
        textoNome.text = nome;
        textoFala.text = fala;

        botao1.SetActive(true);
        botao2.SetActive(true);
        textoBotao1.text = opcao1;
        textoBotao2.text = opcao2;

        dialogando = false; // Pausa o clique na tela para forçar o jogador a clicar num botão
    }

    public void EsconderDialogo()
    {
        caixaDeDialogo.SetActive(false);
        dialogando = false;
    }
}
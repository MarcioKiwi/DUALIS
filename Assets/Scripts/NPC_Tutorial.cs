using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC_Tutorial : MonoBehaviour
{
    [Header("Configurações do NPC")]
    public string nomeNPC = "Estátua Arcana";

    [Header("Interface")]
    public GameObject indicadorDeBotao;
    public DialogueManager gerenciadorDeDialogo; // 🟢 Nova variável para puxar o cérebro da UI!

    private bool playerPerto = false;

    void Start()
    {
        if (indicadorDeBotao != null)
        {
            indicadorDeBotao.SetActive(false);
        }
    }

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E))
        {
            IniciarInteracao();
        }
    }

    void IniciarInteracao()
    {
        if (indicadorDeBotao != null)
        {
            indicadorDeBotao.SetActive(false);
        }

        if (gerenciadorDeDialogo != null)
        {
            // Criando uma lista com várias falas separadas!
            string[] falasDaEstatua = new string[]
            {
                "...",
                "...",
                "Parece que seu psicológico foi afetado pela explosão. Não é atoa que está tentando conversar com uma estátua.",
                "Agora que descobriu que não sou uma estátua qualquer, vou lhe ensinar a usar sua nova força.",
                "enfrente o inimigo a frente e verei se é digno de meu treinamento."

            };

            // Mandando a lista para o Manager
            gerenciadorDeDialogo.MostrarFala(nomeNPC, falasDaEstatua);
        }
        else
        {
            Debug.LogWarning("Faltou arrastar o DialogueManager para a Estátua!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = true;
            if (indicadorDeBotao != null) indicadorDeBotao.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerPerto = false;
            if (indicadorDeBotao != null) indicadorDeBotao.SetActive(false);

            // Opcional: Esconder a caixa de texto se o jogador sair correndo de perto
            if (gerenciadorDeDialogo != null) gerenciadorDeDialogo.EsconderDialogo();
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Biblioteca que permite mudar de cena!

public class IA_Inimigo : MonoBehaviour
{
    [Header("Configurações do Alvo")]
    public Transform alvo;
    public float distanciaDeVisao = 5f;

    [Header("Configurações de Patrulha")]
    public Transform[] pontosDePatrulha;
    public float velocidadePatrulha = 1.5f;
    public float velocidadePerseguicao = 3f;

    [Header("Animação")]
    public Animator anim;

    [Header("Transição de Batalha")]
    [Tooltip("O nome exato da cena que vai carregar ao encostar no jogador")]
    public string nomeDaCenaDeBatalha = "SampleScene";

    private int indicePontoAtual = 0;

    void Update()
    {
        float distanciaProJogador = Vector2.Distance(transform.position, alvo.position);

        if (distanciaProJogador < distanciaDeVisao)
        {
            PerseguirJogador();

            if (anim != null) anim.SetBool("Perseguindo", true);
        }
        else
        {
            Patrulhar();

            if (anim != null) anim.SetBool("Perseguindo", false);
        }
    }

    void Patrulhar()
    {
        if (pontosDePatrulha.Length == 0) return;
        Transform pontoDestino = pontosDePatrulha[indicePontoAtual];
        transform.position = Vector2.MoveTowards(transform.position, pontoDestino.position, velocidadePatrulha * Time.deltaTime);

        if (Vector2.Distance(transform.position, pontoDestino.position) < 0.1f)
        {
            indicePontoAtual++;
            if (indicePontoAtual >= pontosDePatrulha.Length) indicePontoAtual = 0;
        }
    }

    void PerseguirJogador()
    {
        transform.position = Vector2.MoveTowards(transform.position, alvo.position, velocidadePerseguicao * Time.deltaTime);
    }

    // 🔥 A MÁGICA DA TRANSIÇÃO ACONTECE AQUI
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se o objeto que o zumbi encostou tem a etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Zumbi pegou o Advogado! Mudando de cena...");
            // Carrega a cena com o nome escrito na variável
            SceneManager.LoadScene(nomeDaCenaDeBatalha);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeVisao);
    }
}
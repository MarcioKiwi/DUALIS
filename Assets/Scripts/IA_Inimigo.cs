using UnityEngine;
using UnityEngine.SceneManagement; 

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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Zumbi pegou o Advogado! Mudando de cena...");

            PlayerPrefs.SetFloat("PosX", collision.transform.position.x);
            PlayerPrefs.SetFloat("PosY", collision.transform.position.y);
            Inimigo_Mapa configMapa = GetComponent<Inimigo_Mapa>();
            if (configMapa != null)
            {
                PlayerPrefs.SetString("InimigoAtualNoMapa", configMapa.chaveDeMorte);
            }
            SceneManager.LoadScene(nomeDaCenaDeBatalha);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeVisao);
    }
}
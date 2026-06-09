using UnityEngine;

public class GerenciadorDoMapa : MonoBehaviour
{
    public GameObject jogador;
    public Transform pontoDeRespawn;

    void Start()
    {
        // Verifica se o jogador sofreu Game Over na última batalha
        if (PlayerPrefs.GetInt("JogadorMorreu", 0) == 1)
        {
            // Teleporta o jogador para o ponto seguro
            jogador.transform.position = pontoDeRespawn.position;

            // Apaga o aviso de morte para não ficar teleportando toda vez!
            PlayerPrefs.SetInt("JogadorMorreu", 0);
        }
    }
}
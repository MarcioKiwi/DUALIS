using System.Collections;
using UnityEngine;

public class AutoCaminhadaInicial : MonoBehaviour
{
    [Header("Configuração do Jogador")]
    public GameObject jogador;
    public float tempoAndando = 2.5f;
    public float velocidadedeCaminhada = 2f;

    [Header("Configuração do Animator")]
    public string parametroAndar = "Andando"; 
    public string parametroX = "MoveX";  
    public string parametroY = "MoveY";    

    private PlayerMovement scriptDeControleDoJogador;

    void Start()
    {
        if (jogador != null)
        {
            scriptDeControleDoJogador = jogador.GetComponent<PlayerMovement>();
        }

        // Só executa se for a primeira vez que entramos no mapa (New Game)
        if (PlayerPrefs.GetInt("IntroConcluida", 0) == 0)
        {
            StartCoroutine(SequenciaDeEntrada());
        }
        else
        {
            // Se já jogou antes, garante que o controle comece ativo
            if (scriptDeControleDoJogador != null) scriptDeControleDoJogador.enabled = true;
        }
    }

    IEnumerator SequenciaDeEntrada()
    {
        // 1. Trava o controle do jogador
        if (scriptDeControleDoJogador != null) scriptDeControleDoJogador.enabled = false;

        
        Animator anim = jogador.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetBool(parametroAndar, true);

            
            anim.SetFloat(parametroX, 1f);
            anim.SetFloat(parametroY, 0f);
        }

        
        float contador = 0;
        while (contador < tempoAndando)
        {
            jogador.transform.Translate(Vector3.right * velocidadedeCaminhada * Time.deltaTime);
            contador += Time.deltaTime;
            yield return null;
        }

        
        if (anim != null)
        {
            anim.SetBool(parametroAndar, false);
            
            anim.SetFloat(parametroX, 0f);
            anim.SetFloat(parametroY, 0f);
        }

        // Devolve o controle
        if (scriptDeControleDoJogador != null) scriptDeControleDoJogador.enabled = true;

        PlayerPrefs.SetInt("IntroConcluida", 1);
    }
}
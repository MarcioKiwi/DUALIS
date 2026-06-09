using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
    [Header("Configurações de História")]
    public bool lutaDoTutorial = true; // 🟢 O interruptor do milagre!

    [Header("Estado Atual")]
    public BattleState state;

    [Header("Grupos (Party)")]
    public List<CharacterBattle> playerParty;
    public List<CharacterBattle> enemyParty;

    [Header("Interface(HUD)")]
    public List<BattleHUD> playerHUDs;
    public List<BattleHUD> enemyHUDs;
    public int currentPlayerIndex = 0;
    public int currentTargetIndex = 0;

    public DialogueManager gerenciadorDeDialogo; 

    [Header("Telas de Fim de Jogo")]
    public GameObject endGamePanel;
    public TextMeshProUGUI resultText;

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        Debug.Log("A Batalha de Grupos começou");

        for (int i = 0; i < playerParty.Count; i++)
        {
            if (playerParty[i] == null) continue;
            if (playerParty[i].baseData == null) continue;
            if (i >= playerHUDs.Count || playerHUDs[i] == null) continue;

            playerHUDs[i].SetupHUD(playerParty[i].baseData, playerParty[i].level);
        }

        for (int i = 0; i < enemyParty.Count; i++)
        {
            if (enemyParty[i] == null || enemyParty[i].baseData == null || i >= enemyHUDs.Count || enemyHUDs[i] == null) continue;
            enemyHUDs[i].SetupHUD(enemyParty[i].baseData, enemyParty[i].level);
        }

        state = BattleState.PLAYERTURN;
    }

    public void PlayerAttack()
    {
        if (state != BattleState.PLAYERTURN) return;

        CharacterBattle player = playerParty[currentPlayerIndex];
        CharacterBattle enemy = enemyParty[currentTargetIndex];

        Debug.Log($"{player.baseData.characterName} ataca com sua espada!");

        Animator anim = player.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("Atacar");

        int damage = player.baseData.baseStrength;
        bool isDead = enemy.TakeDamage(damage);
        player.GainPsychological(20);

        playerHUDs[currentPlayerIndex].UpdatePsychological(player.currentPsychological);
        enemyHUDs[currentTargetIndex].UpdateHP(enemy.currentHP, enemy.baseData.maxHP);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            NextPlayerTurn();
        }
    }

    public void PlayerMagic()
    {
        if (state != BattleState.PLAYERTURN) return;

        CharacterBattle player = playerParty[currentPlayerIndex];
        CharacterBattle enemy = enemyParty[currentTargetIndex];

        int magicCost = 10;
        int magicDamage = player.baseData.baseStrength * 2;

        bool hasEnoughMP = player.SpendMP(magicCost);

        if (hasEnoughMP)
        {
            Debug.Log($"{player.baseData.characterName} lançou uma Habilidade Poderosa!");
            playerHUDs[currentPlayerIndex].UpdateMP(player.currentMP, player.baseData.maxMP);

            bool isDead = enemy.TakeDamage(magicDamage);
            enemyHUDs[currentTargetIndex].UpdateHP(enemy.currentHP, enemy.baseData.maxHP);

            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                NextPlayerTurn();
            }
        }
        else
        {
            Debug.Log("MP Insuficiente! Escolha outra ação.");
        }
    }

    public void PlayerHeal()
    {
        if (state != BattleState.PLAYERTURN) return;
        CharacterBattle player = playerParty[currentPlayerIndex];

        int healCost = 5;
        int healAmount = 20;

        bool hasEnoughMP = player.SpendMP(healCost);

        if (hasEnoughMP)
        {
            Debug.Log($"{player.baseData.characterName} usou uma Magia de Cura!");
            player.Heal(healAmount);

            playerHUDs[currentPlayerIndex].UpdateHP(player.currentHP, player.baseData.maxHP);
            playerHUDs[currentPlayerIndex].UpdateMP(player.currentMP, player.baseData.maxMP);

            NextPlayerTurn();
        }
        else
        {
            Debug.Log("MP Insuficiente para se curar!");
        }
    }

    void EnemyTurn()
    {
        CharacterBattle enemy = enemyParty[currentTargetIndex];
        Debug.Log($"Turno do inimigo! O {enemy.baseData.characterName} está se preparando...");
        StartCoroutine(EnemyAttackRoutine());
    }

    System.Collections.IEnumerator EnemyAttackRoutine()
    {
        CharacterBattle enemy = enemyParty[currentTargetIndex];

        yield return new WaitForSeconds(1.5f);
        Debug.Log($"O {enemy.baseData.characterName} ataca!");

        Animator anim = enemy.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("Atacar");

        int randomTargetIndex = Random.Range(0, playerParty.Count);
        CharacterBattle targetHero = playerParty[randomTargetIndex];

        int damage = enemy.baseData.baseStrength;
        bool isDead = targetHero.TakeDamage(damage);

        int stressDamage = 5;
        targetHero.TakeStress(stressDamage);

        playerHUDs[randomTargetIndex].UpdateHP(targetHero.currentHP, targetHero.baseData.maxHP);
        playerHUDs[randomTargetIndex].UpdatePsychological(targetHero.currentPsychological);

        if (isDead)
        {
            // 🟢 Toca a animação de morte!
            Animator animHero = targetHero.GetComponentInChildren<Animator>();
            if (animHero != null) animHero.SetTrigger("Morte");

            if (lutaDoTutorial)
            {
                // 🟢 Inicia a rotina secreta da Estátua
                StartCoroutine(RotinaDoMilagre(targetHero, randomTargetIndex));
            }
            else
            {
                state = BattleState.LOST;
                EndBattle();
            }
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            state = BattleState.PLAYERTURN;
            NextPlayerTurn();
        }
    }


    // 🟢 A Mágica Acontece Aqui (Agora com Caixa de Texto!)
    System.Collections.IEnumerator RotinaDoMilagre(CharacterBattle hero, int hudIndex)
    {
        // 1. Espera 2 segundos para o jogador ver a animação de morte
        yield return new WaitForSeconds(2.0f);

        // 2. Chama a caixa de diálogo na tela (se ela estiver conectada)
        if (gerenciadorDeDialogo != null)
        {
            string[] falasDoMilagre = new string[]
            {
                "O tempo congela...",
                "É só isso do que és capaz?",
                "Última chance, mostre-me que és capaz de possuir tal poder."
            };

            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falasDoMilagre);

            // 🛑 MAGIA NEGRA DA PROGRAMAÇÃO: O código PAUSA aqui até a caixa de texto sumir da tela!
            yield return new WaitUntil(() => !gerenciadorDeDialogo.caixaDeDialogo.activeSelf);
        }
        else
        {
            // Se esquecer de arrastar no Inspector, ele ainda avisa no Console para não travar o jogo
            Debug.Log("O tempo congela... A voz da Estátua ecoa: 'Ainda não é sua hora, Advogado.'");
        }

        // 3. Enche a vida
        hero.Heal(hero.baseData.maxHP);
        playerHUDs[hudIndex].UpdateHP(hero.currentHP, hero.baseData.maxHP);

        // 4. Toca a animação de voltar à vida
        Animator animHero = hero.GetComponentInChildren<Animator>();
        if (animHero != null) animHero.SetTrigger("Reviver");

        // 5. Desliga o milagre para ele não ser imortal
        lutaDoTutorial = false;

        // 6. Dá um tempinho extra e devolve o turno pro jogador
        yield return new WaitForSeconds(1.5f);
        state = BattleState.PLAYERTURN;
        NextPlayerTurn();
    }

    public void NextPlayerTurn()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex >= playerParty.Count)
        {
            currentPlayerIndex = -1;
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
        else
        {
            playerParty[currentPlayerIndex].OnRoundEnd();
            playerHUDs[currentPlayerIndex].UpdatePsychological(playerParty[currentPlayerIndex].currentPsychological);

            if (playerParty[currentPlayerIndex].inactiveTurnsLeft > 0)
            {
                Debug.Log($"--- {playerParty[currentPlayerIndex].baseData.characterName} ESTÁ EM PÂNICO E PERDEU A VEZ! ---");
                playerParty[currentPlayerIndex].inactiveTurnsLeft--;
                NextPlayerTurn();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                Debug.Log($"É a vez de {playerParty[currentPlayerIndex].baseData.characterName}!");
            }
        }
    }

    void EndBattle()
    {
        endGamePanel.SetActive(true);
        CharacterBattle player = playerParty[currentPlayerIndex];
        CharacterBattle enemy = enemyParty[currentTargetIndex];

        if (state == BattleState.WON)
        {
            Debug.Log("VITÓRIA!");

            for (int i = 0; i < playerParty.Count; i++)
            {
                playerParty[i].AddXP(enemy.baseData.xpReward);
                playerHUDs[i].UpdateHP(playerParty[i].currentHP, playerParty[i].baseData.maxHP);
                playerHUDs[i].UpdateLevel(playerParty[i].level);
            }
            resultText.text = "VITÓRIA!";
            resultText.color = Color.green;

            // 🟢 MEMORY CARD: Avisa que o zumbi morreu!
            PlayerPrefs.SetInt("ZumbiTutorialMorto", 1);

        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("DERROTA...");
            resultText.text = "GAME OVER";
            resultText.color = Color.red;

            // 🟢 MEMORY CARD: Avisa que o jogador morreu (só acontece se não for o milagre)!
            PlayerPrefs.SetInt("JogadorMorreu", 1);
        }
    }

    // 🟢 Função nova para voltar ao mapa! (Lembre-se de colocar ela no seu botão de continuar)
    public void VoltarParaMapa()
    {
        // Certifique-se de que o nome da cena está EXATAMENTE igual ao seu arquivo
        SceneManager.LoadScene("Zona Industrial");
    }
}
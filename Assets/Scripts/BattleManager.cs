using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
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

    [Header ("Telas de Fim de Jogo")]
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
            if (playerParty[i] == null)
            {
                Debug.LogError($"[ERRO FATAL] O espaço {i} da lista Player Party está VAZIO no BattleManager!");
                continue; 
            }
            if (playerParty[i].baseData == null)
            {
                Debug.LogError($"[ERRO FATAL] O Herói {playerParty[i].gameObject.name} está sem a Ficha (Base Data) no Inspetor dele!");
                continue;
            }
            if (i >= playerHUDs.Count || playerHUDs[i] == null)
            {
                Debug.LogError($"[ERRO FATAL] O Menu (HUD) do Herói {playerParty[i].gameObject.name} não foi arrastado para o BattleManager!");
                continue;
            }

            playerHUDs[i].SetupHUD(playerParty[i].baseData, playerParty[i].level);
        }

        for (int i = 0; i < enemyParty.Count; i++)
        {
            if (enemyParty[i] == null || enemyParty[i].baseData == null || i >= enemyHUDs.Count || enemyHUDs[i] == null)
            {
                Debug.LogError($"[ERRO FATAL] Falta arrastar algo do Inimigo no Inspetor!");
                continue;
            }
            enemyHUDs[i].SetupHUD(enemyParty[i].baseData, enemyParty[i].level);
        }

        state = BattleState.PLAYERTURN;
    }


    public void PlayerAttack()
    {
        
        if (state != BattleState.PLAYERTURN)
            return;

        CharacterBattle player = playerParty[currentPlayerIndex];
        CharacterBattle enemy = enemyParty[currentTargetIndex];

        Debug.Log($"{player.baseData.characterName} ataca com sua espada!");

        int damage = player.baseData.baseStrength;
        bool isDead = enemy.TakeDamage(damage);
        player.GainPsychological(10);

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
        if (state != BattleState.PLAYERTURN)
            return;

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
        if (state != BattleState.PLAYERTURN)
            return;
        CharacterBattle player = playerParty[currentPlayerIndex];
        CharacterBattle enemy = enemyParty[currentTargetIndex];


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

        int randomTargetIndex = Random.Range(0, playerParty.Count);

        CharacterBattle targetHero = playerParty[randomTargetIndex];

        int damage = enemy.baseData.baseStrength;
        bool isDead = targetHero.TakeDamage(damage);

        playerHUDs[randomTargetIndex].UpdateHP(targetHero.currentHP, targetHero.baseData.maxHP);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            yield return new WaitForSeconds(1f);
            state = BattleState.PLAYERTURN;
        }
    }

    public void NextPlayerTurn()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex >= playerParty.Count)
        {
            currentPlayerIndex = 0;
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            Debug.Log($"É a vez de {playerParty[currentPlayerIndex].baseData.characterName}!");
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

        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("DERROTA...");
            resultText.text = "GAME OVER";
            resultText.color = Color.red;
        }
    }

    public void RestartBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
    [Header("Configurações de História")]
    public bool lutaDoTutorial = true;

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

    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelUpText;
    public TextMeshProUGUI dropsText;

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        for (int i = 0; i < playerParty.Count; i++)
        {
            if (playerParty[i] == null || playerParty[i].baseData == null || i >= playerHUDs.Count || playerHUDs[i] == null) continue;
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
            player.Heal(healAmount);
            playerHUDs[currentPlayerIndex].UpdateHP(player.currentHP, player.baseData.maxHP);
            playerHUDs[currentPlayerIndex].UpdateMP(player.currentMP, player.baseData.maxMP);
            NextPlayerTurn();
        }
    }

    void EnemyTurn()
    {
        CharacterBattle enemy = enemyParty[currentTargetIndex];
        StartCoroutine(EnemyAttackRoutine());
    }

    System.Collections.IEnumerator EnemyAttackRoutine()
    {
        CharacterBattle enemy = enemyParty[currentTargetIndex];
        yield return new WaitForSeconds(1.5f);

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
            Animator animHero = targetHero.GetComponentInChildren<Animator>();
            if (animHero != null) animHero.SetTrigger("Morte");

            if (lutaDoTutorial)
            {
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

    System.Collections.IEnumerator RotinaDoMilagre(CharacterBattle hero, int hudIndex)
    {
        yield return new WaitForSeconds(2.0f);

        if (gerenciadorDeDialogo != null)
        {
            string[] falasDoMilagre = new string[]
            {
                "O tempo congela...",
                "É só isso do que é capaz?",
                "Última chance, mostre-me que é capaz de possuir tal poder."
            };

            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falasDoMilagre);
            yield return new WaitUntil(() => !gerenciadorDeDialogo.caixaDeDialogo.activeSelf);
        }

        hero.Heal(hero.baseData.maxHP);
        playerHUDs[hudIndex].UpdateHP(hero.currentHP, hero.baseData.maxHP);

        Animator animHero = hero.GetComponentInChildren<Animator>();
        if (animHero != null) animHero.SetTrigger("Reviver");

        lutaDoTutorial = false;

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
                playerParty[currentPlayerIndex].inactiveTurnsLeft--;
                NextPlayerTurn();
            }
            else
            {
                state = BattleState.PLAYERTURN;
            }
        }
    }

    void EndBattle()
    {
        endGamePanel.SetActive(true);
        CharacterBattle enemy = enemyParty[currentTargetIndex];

        if (xpText != null) xpText.text = "";
        if (levelUpText != null) levelUpText.text = "";
        if (dropsText != null) dropsText.text = "";

        if (state == BattleState.WON)
        {
            resultText.text = "VITÓRIA!";
            resultText.color = Color.green;

            PlayerPrefs.SetInt("JogadorMorreu", 0);

            int xpGanho = enemy.baseData.xpReward;
            int moedaGanha = enemy.baseData.dropPsicologico;

            // Salva as moedas
            int moedaAtual = PlayerPrefs.GetInt("PontosPsicologico", 0);
            PlayerPrefs.SetInt("PontosPsicologico", moedaAtual + moedaGanha);

            int niveisGanhosTotais = 0;

            for (int i = 0; i < playerParty.Count; i++)
            {
                int levelAntes = playerParty[i].level;

                playerParty[i].AddXP(xpGanho); // Distribui o XP e calcula o Level Up

                int levelDepois = playerParty[i].level;
                niveisGanhosTotais += (levelDepois - levelAntes);

                // GRAVA O PROGRESSO DEFINITIVO DE LEVEL E XP DE CADA HERÓI
                PlayerPrefs.SetInt("LevelAdvogado", playerParty[i].level);
                PlayerPrefs.SetInt("XPAdvogado", playerParty[i].currentXP);

                playerHUDs[i].UpdateHP(playerParty[i].currentHP, playerParty[i].baseData.maxHP);
                playerHUDs[i].UpdateLevel(playerParty[i].level);
            }

            if (xpText != null) xpText.text = $"+{xpGanho} XP";

            if (levelUpText != null)
            {
                if (niveisGanhosTotais > 0)
                    levelUpText.text = $"LEVEL UP! (+{niveisGanhosTotais} Níveis)";
                else
                    levelUpText.gameObject.SetActive(false);
            }

            if (dropsText != null) dropsText.text = $"Extraído: {moedaGanha} Pontos Psicológicos";

            // 🟢 A MÁGICA ACONTECE AQUI: Lê qual foi o monstro que iniciou a luta e salva a morte DELE.
            string chaveDoInimigoDerrotado = PlayerPrefs.GetString("InimigoAtualNoMapa", "ZumbiTutorialMorto");
            PlayerPrefs.SetInt(chaveDoInimigoDerrotado, 1);
        }
        else if (state == BattleState.LOST)
        {
            resultText.text = "GAME OVER";
            resultText.color = Color.red;
            PlayerPrefs.SetInt("JogadorMorreu", 1);
        }
    }

    public void VoltarParaMapa()
    {
        SceneManager.LoadScene("Zona Industrial");
    }
}
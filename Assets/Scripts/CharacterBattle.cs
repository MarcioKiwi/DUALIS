using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    [Header("A Ficha do Personagem")]
    public CharacterData baseData;
    private CharacterData originalAdvogadoData;

    [Header("Transformação")]
    public CharacterData transformedData;
    public bool isTransformed = false;

    [Header("Status Atual da Batalha")]
    public int currentHP;
    public int currentMP;
    public int currentPsychological;
    public int inactiveTurnsLeft = 0;

    [Header("Progressão")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    void Awake()
    {
        baseData = Instantiate(baseData);
        originalAdvogadoData = baseData;

        // 🧠 1. LENDO O LEVEL E O XP DO MEMORY CARD
        int levelSalvo = PlayerPrefs.GetInt("LevelAdvogado", 1);
        currentXP = PlayerPrefs.GetInt("XPAdvogado", 0); // 🟢 NOVO: Puxa o XP acumulado!

        if (levelSalvo > 1)
        {
            level = levelSalvo;
            int bonusLevels = levelSalvo - 1;
            baseData.maxHP += (10 * bonusLevels);
            baseData.baseStrength += (2 * bonusLevels);

            // 🟢 NOVO: Ajusta dinamicamente a meta de XP baseada no seu nível atual
            xpToNextLevel = 100 + (50 * bonusLevels);

            Debug.Log($"[Dualis] Advogado entrou no Nível {level} com {currentXP}/{xpToNextLevel} de XP!");
        }

        // 🧠 2. PUXANDO OS UPGRADES DA LOJA CLANDESTINA
        int bonusVida = PlayerPrefs.GetInt("BonusVida", 0);
        int bonusMana = PlayerPrefs.GetInt("BonusMana", 0);
        int bonusForca = PlayerPrefs.GetInt("BonusForca", 0);
        int bonusPsico = PlayerPrefs.GetInt("BonusPsicologico", 0);

        baseData.maxHP += bonusVida;
        baseData.maxMP += bonusMana;
        baseData.baseStrength += bonusForca;
        originalAdvogadoData.maxPshychological += bonusPsico;

        // 3. ENCHENDO AS BARRAS PARA A LUTA COMEÇAR
        currentHP = baseData.maxHP;
        currentMP = baseData.maxMP;
        currentPsychological = 0;
    }

    public bool TakeDamage(int damageAmount)
    {
        int danoReal = damageAmount - baseData.baseDefense;
        if (danoReal < 0) danoReal = 0;
        currentHP -= danoReal;

        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }
        return false;
    }

    public void TakeStress(int stressAmount)
    {
        currentPsychological -= stressAmount;
        if (currentPsychological < 0) currentPsychological = 0;

        if (currentHP == 0)
        {
            inactiveTurnsLeft = 1;
        }
    }

    public bool SpendMP(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            return true;
        }
        return false;
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > baseData.maxHP) currentHP = baseData.maxHP;
    }

    // 🟢 ATUALIZADO: Modificado para atualizar a meta de XP corretamente no Level Up
    public void AddXP(int xpAmount)
    {
        currentXP += xpAmount;
        Debug.Log($"{baseData.characterName} ganhou {xpAmount} de XP! (Total: {currentXP}/{xpToNextLevel})");

        if (currentXP >= xpToNextLevel)
        {
            level++;
            currentXP -= xpToNextLevel;
            xpToNextLevel += 50; // Aumenta a dificuldade do próximo nível
            baseData.maxHP += 10;
            baseData.baseStrength += 2;
            currentHP = baseData.maxHP;

            Debug.Log($"LEVEL UP!!! {baseData.characterName} alcançou o nível {level}!");
        }
    }

    public void GainPsychological(int amount)
    {
        if (isTransformed) return;
        currentPsychological += amount;

        if (currentPsychological > originalAdvogadoData.maxPshychological)
        {
            currentPsychological = originalAdvogadoData.maxPshychological;
        }
    }

    public void Transform()
    {
        if (isTransformed) return;

        if (currentPsychological >= originalAdvogadoData.maxPshychological)
        {
            baseData = Instantiate(transformedData);
            isTransformed = true;
        }
    }

    public void RevertTransform()
    {
        if (isTransformed)
        {
            baseData = originalAdvogadoData;
            isTransformed = false;
            currentPsychological = 0;
        }
    }

    public void OnRoundEnd()
    {
        if (isTransformed)
        {
            int dreno = Mathf.CeilToInt(originalAdvogadoData.maxPshychological * 0.20f);
            currentPsychological -= dreno;

            if (currentPsychological <= 0)
            {
                currentPsychological = 0;
                RevertTransform();
                inactiveTurnsLeft = 2;
            }
        }
    }
}
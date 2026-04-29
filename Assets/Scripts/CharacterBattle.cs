using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    [Header("A Ficha do Personagem")]
    public CharacterData baseData;

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

        currentHP = baseData.maxHP;
        currentMP = baseData.maxMP;
        currentPsychological = 0;

        Debug.Log($"O personagem {baseData.characterName} entrou na batalha com {currentHP} de vida!");
    }

    
    public bool TakeDamage(int damageAmount)
    {
        int danoReal = damageAmount - baseData.baseDefense;

        if( danoReal < 0)
        {
            danoReal = 0;
        }

        currentHP -= danoReal;
        
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log($"{baseData.characterName} tomou {damageAmount} de dano e FOI DERROTADO!");
            return true; // Retorna VERDADEIRO: Sim, ele morreu.
        }

        Debug.Log($"{baseData.characterName} tomou {damageAmount} de dano! HP restante: {currentHP}");
        return false; // Retorna FALSO: Não, ele ainda está vivo.
    }

    public void TakeStress(int stressAmount)
    {
        currentPsychological -= stressAmount;

        if (currentPsychological < 0)
        {
            currentPsychological = 0;
        }

        Debug.Log($"{baseData.characterName} sofreu estresse! Mente atual: {currentPsychological}");

        if (currentHP == 0)
        {
            Debug.Log($"[Crise] {baseData.characterName} ENTROU EM PÂNICO E PERDEU O CONTROLE!");
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
        else
        {
            return false;
        }
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > baseData.maxHP)
        {
            currentHP = baseData.maxHP;
        } 
    }
    public void AddXP(int xpAmount)
    {
        currentXP += xpAmount;
        Debug.Log($"{baseData.characterName} ganhou {xpAmount} de XP! (Total: {currentXP}/{xpToNextLevel})");

        if (currentXP >= xpToNextLevel)
        {
            level++;
            currentXP -= xpToNextLevel;
            xpToNextLevel += 50;
            baseData.maxHP += 10;
            baseData.baseStrength += 2;
            currentHP = baseData.maxHP;

            Debug.Log($"LEVEL UP!!! {baseData.characterName} alcançou o nível {level}!");
            Debug.Log($"Novos Status -> HP Máximo: {baseData.maxHP} | Força: {baseData.baseStrength}");
        }
    }

    public void GainPsychological(int amount)
    {
        currentPsychological += amount;

        if (currentPsychological > baseData.maxPshychological)
        {
            currentPsychological = baseData.maxPshychological;
        }
        Debug.Log($"{baseData.characterName} bateu e ganhou {amount} de Psicológico! (Atual: {currentPsychological})");
    }

    public void Transform()
    {
        if (!isTransformed && currentPsychological > 0)
        {
            baseData = Instantiate(transformedData);
            isTransformed = true;
            Debug.Log($"{gameObject.name} se transformou em {baseData.characterName}!");
        }
    }
}


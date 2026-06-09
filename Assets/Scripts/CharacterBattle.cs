using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    [Header("A Ficha do Personagem")]
    public CharacterData baseData;
    private CharacterData originalAdvogadoData; // 🧠 Memória para poder voltar a ser Advogado!

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
        originalAdvogadoData = baseData; // Guarda o Advogado em segurança antes de qualquer coisa

        currentHP = baseData.maxHP;
        currentMP = baseData.maxMP;
        currentPsychological = 0;

        Debug.Log($"O personagem {baseData.characterName} entrou na batalha com {currentHP} de vida!");
    }

    public bool TakeDamage(int damageAmount)
    {
        int danoReal = damageAmount - baseData.baseDefense;

        if (danoReal < 0)
        {
            danoReal = 0;
        }

        currentHP -= danoReal;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log($"{baseData.characterName} tomou {damageAmount} de dano e FOI DERROTADO!");
            return true;
        }

        Debug.Log($"{baseData.characterName} tomou {damageAmount} de dano! HP restante: {currentHP}");
        return false;
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
        }
    }

    public void GainPsychological(int amount)
    {
        // Se já estiver transformado, ele não ganha mais psicológico (já está no topo drenando)
        if (isTransformed) return;

        currentPsychological += amount;

        // Usei a grafia 'maxPshychological' do seu ScriptableObject original
        if (currentPsychological > originalAdvogadoData.maxPshychological)
        {
            currentPsychological = originalAdvogadoData.maxPshychological;
        }
        Debug.Log($"{baseData.characterName} ganhou {amount} de Psicológico! (Atual: {currentPsychological}/{originalAdvogadoData.maxPshychological})");
    }

    // 🔥 MUDANÇA: Agora o jogador ESCOLHE clicar aqui através de um botão
    public void Transform()
    {
        if (isTransformed)
        {
            Debug.LogWarning("Já está transformado!");
            return;
        }

        // Só permite se a barra estiver em 100% (igual ou maior ao máximo)
        if (currentPsychological >= originalAdvogadoData.maxPshychological)
        {
            baseData = Instantiate(transformedData);
            isTransformed = true;
            Debug.Log($"🔥 OBJEÇÃO ARTIFACT! Se transformou em {baseData.characterName}!");

            // TODO: Aqui você avisa o BattleHUD para atualizar os textos/barras se os status mudarem
        }
        else
        {
            Debug.Log("Psicológico ainda não está em 100% para transformar!");
        }
    }

    // ⚖️ NOVO MÉTODO: Traz o Advogado de volta
    public void RevertTransform()
    {
        if (isTransformed)
        {
            baseData = originalAdvogadoData; // Puxa o Advogado de volta da memória
            isTransformed = false;
            currentPsychological = 0; // Zera a barra para recomeçar o ciclo
            Debug.Log($"⚖️ O efeito acabou. Voltou a ser {baseData.characterName}.");

            // TODO: Avisar o BattleHUD para resetar visualmente
        }
    }

    // ⏳ Drena 20% da barra a cada rodada
    // Deves chamar esta função no fim de cada ronda
    public void OnRoundEnd()
    {
        if (isTransformed)
        {
            // Calcula quanto é 20% do valor máximo da barra
            int dreno = Mathf.CeilToInt(originalAdvogadoData.maxPshychological * 0.20f);
            currentPsychological -= dreno;

            Debug.Log($"[Turno] Juiz Arcano a gastar energia. Drenado: {dreno} de Psicológico. Restante: {currentPsychological}");

            // Se a energia acabar, destransforma-se automaticamente e fica atordoado
            if (currentPsychological <= 0)
            {
                currentPsychological = 0;
                RevertTransform(); // Volta a ser Advogado

                // 🔥 NOVO: Fica atordoado por 2 rondas inteiras!
                inactiveTurnsLeft = 2;
                Debug.Log($"[Exaustão] A mente do Advogado colapsou! Está atordoado por {inactiveTurnsLeft} rondas.");
            }
        }
    }
}

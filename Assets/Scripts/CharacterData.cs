using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "RPG/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Informações Básicas")]
    public string characterName;
    public Sprite portrait;

    [Header("Atributos de Combate")]
    public int maxHP;
    public int maxMP;
    public int baseStrength;
    public int baseDefense;
    public int baseSpeed;

    [Header("Atributos do Dualis")]
    public int maxPshychological;

    [Header("Recompensas")]
    public int xpReward;
    public int dropPsicologico; // 🟢 NOVO: A Moeda do Jogo!
}

using UnityEngine;


[CreateAssetMenu(fileName = "New Character Data", menuName = "RPG/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Informações Básicas")]
    public string characterName;
    public Sprite portrait; // A foto do rosto dele no menu

    [Header("Atributos de Combate")]
    public int maxHP;
    public int maxMP;
    public int baseStrength; // Força para ataques físicos
    public int baseDefense; // Defesa contra ataques
    public int baseSpeed; // Velocidade (quem tem mais, ataca primeiro no turno)

    [Header("Atributos do Dualis")]
    public int maxPshychological;

    [Header("Recompensas")]
    public int xpReward;
}


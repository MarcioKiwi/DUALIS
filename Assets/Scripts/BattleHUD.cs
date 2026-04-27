using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BattleHUD : MonoBehaviour
{
    [Header("Elementos da tela")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI mpText;

    [Header("Barras Visuais")]
    public Slider hpSlider;
    public Slider mpSlider;
    public Slider psychologycalSlider;

    
    public void SetupHUD(CharacterData character, int currentLevel)
    {
        nameText.text = character.characterName;
        levelText.text = $"Lvl: {currentLevel}";
        hpText.text = $"HP: {character.maxHP}/{character.maxHP}";
        hpSlider.maxValue = character.maxHP;
        hpSlider.value = character.maxHP;

        if (mpText != null && mpSlider != null)
        {
            mpText.text = $"MP: {character.maxMP}/{character.maxMP}";
            mpSlider.maxValue = character.maxMP;
            mpSlider.value = character.maxMP;
        }

        if (psychologycalSlider != null)
        {
            psychologycalSlider.maxValue = character.maxPshychological;
            psychologycalSlider.value = character.maxPshychological;
        }
    }

    
    public void UpdateHP(int currentHP, int maxHP)
    {
        hpText.text = $"HP: {currentHP}/{maxHP}";
        hpSlider.value = currentHP;
    }

    public void UpdateMP(int currentMP, int maxMP)
    {
        if (mpText != null && mpSlider != null)
        {
            mpText.text = $"MP: {currentMP}/{maxMP}";
            mpSlider.value = currentMP;
        }
        
    }

    public void UpdatePsychological(int currentPsy)
    {
        if (psychologycalSlider != null)
        {
            psychologycalSlider.value = currentPsy;
        }
    }

    public void UpdateLevel(int currentLevel)
    {
        levelText.text = $"Lvl: {currentLevel}";
    }
}

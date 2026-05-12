using UnityEngine;

[CreateAssetMenu(fileName = "NovoItem", menuName = "Inventario/Novo Item")]
public class ItemData : ScriptableObject
{
    [Header("Informações Básicas")]
    public string nomeItem;
    [TextArea(3, 5)]
    public string descricaoItem;
    public Sprite iconeItem;

    [Header("Configurações do GDD")]
    public TipoDeItem tipo;

    [Header("Efeitos (Para Consumíveis)")]
    public int valorCuraHP;
    public int valorCuraSanidade;
}

public enum TipoDeItem
{
    Consumivel,
    Equipavel,
    Importante
}
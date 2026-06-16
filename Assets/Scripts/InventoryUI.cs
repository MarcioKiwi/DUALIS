using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Conexão com a Mochila")]
    public InventoryManager inventoryManager;
    public CharacterData fichaDoAdvogado;

    [Header("Paineis Principais")]
    public GameObject painelInventarioInteiro; 
    public GameObject areaDosItens;
    public GameObject painelStatus;

    [Header("Onde os itens vão aparecer")]
    public Transform areaGridItens;
    public GameObject slotPrefab;

    [Header("Painel de Descrição / Status")]
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoEfeito;
    public Button botaoUsar;

    private TipoDeItem abaAtual = TipoDeItem.Consumivel;
    private ItemData itemSelecionado;

    void Start()
    {
        LimparDescricao();
        painelStatus.SetActive(false);
        areaDosItens.SetActive(true);
        AtualizarInterface();
    }

    
    void OnEnable()
    {
        abaAtual = TipoDeItem.Consumivel;
        LimparDescricao();

        if (painelStatus != null) painelStatus.SetActive(false);
        if (areaDosItens != null) areaDosItens.SetActive(true);

        AtualizarInterface();
    }

    public void FecharInventario()
    {
        // Se o painel de status estiver ativo, o botão de voltar APENAS volta para os itens
        if (painelStatus != null && painelStatus.activeSelf)
        {
            painelStatus.SetActive(false);
            areaDosItens.SetActive(true);
            LimparDescricao();
            AtualizarInterface();
        }
        // Se o painel de status já estiver fechado (ou seja, o jogador já está na tela de itens)
        else
        {
            // Aí sim, o botão fecha o inventário inteiro!
            if (painelInventarioInteiro != null)
            {
                painelInventarioInteiro.SetActive(false);
            }
        }
    }

    // --- ABAS DE ITENS ---
    public void ClicarAbaConsumiveis() { MudarAba(TipoDeItem.Consumivel); }
    public void ClicarAbaEquipaveis() { MudarAba(TipoDeItem.Equipavel); }
    public void ClicarAbaImportantes() { MudarAba(TipoDeItem.Importante); }

    private void MudarAba(TipoDeItem novaAba)
    {
        painelStatus.SetActive(false);
        areaDosItens.SetActive(true);
        abaAtual = novaAba;
        LimparDescricao();
        AtualizarInterface();
    }

    public void ClicarAbaStatus()
    {
        areaDosItens.SetActive(false);
        painelStatus.SetActive(true);
        LimparDescricao();

        // 1. Resgata o nível e o dinheiro
        int levelAtual = PlayerPrefs.GetInt("LevelAdvogado", 1);
        int pontosPsicologicos = PlayerPrefs.GetInt("PontosPsicologico", 0);
        int xpAtual = PlayerPrefs.GetInt("XPAdvogado", 0);

        // 2. 🟢 Resgata todos os bônus comprados na Estátua Arcana
        int bonusVida = PlayerPrefs.GetInt("BonusVida", 0);
        int bonusMana = PlayerPrefs.GetInt("BonusMana", 0);
        int bonusForca = PlayerPrefs.GetInt("BonusForca", 0);
        int bonusMental = PlayerPrefs.GetInt("BonusPsicologico", 0);

        // 3. 🟢 Calcula o valor REAL (Base + Level + Bônus da Estátua)
        int hpReal = fichaDoAdvogado.maxHP + (10 * (levelAtual - 1)) + bonusVida;
        int mpReal = fichaDoAdvogado.maxMP + bonusMana;
        int forcaReal = fichaDoAdvogado.baseStrength + bonusForca;

        // 4. Escreve na tela mostrando o total e o quanto veio da loja
        textoNome.text = "Status de " + fichaDoAdvogado.characterName;

        textoEfeito.text = $"Nível Atual: {levelAtual}\n" +
                           $"Pontos Psicológicos (Moeda): {pontosPsicologicos}\n\n" +
                           $"Vida Máxima (HP): {hpReal} (Bônus: +{bonusVida})\n" +
                           $"Mana Máxima (MP): {mpReal} (Bônus: +{bonusMana})\n" +
                           $"Força de Ataque: {forcaReal} (Bônus: +{bonusForca})\n" +
                           $"Sanidade: {bonusMental}";
    }

    public void AtualizarInterface()
    {
        foreach (Transform filho in areaGridItens) Destroy(filho.gameObject);

        foreach (ItemData item in inventoryManager.itenNoInventario)
        {
            if (item.tipo == abaAtual)
            {
                GameObject novoSlot = Instantiate(slotPrefab, areaGridItens);
                novoSlot.GetComponent<Image>().sprite = item.iconeItem;

                
                novoSlot.GetComponent<Button>().onClick.AddListener(() => MostrarDescricao(item));
            }
        }
    }

    public void MostrarDescricao(ItemData item)
    {
        itemSelecionado = item;
        botaoUsar.gameObject.SetActive(true);
        textoNome.text = item.nomeItem;
        textoEfeito.text = item.descricaoItem;
    }

    public void LimparDescricao()
    {
        itemSelecionado = null;
        textoNome.text = "";
        textoEfeito.text = "";
        botaoUsar.gameObject.SetActive(false);
    }

    public void UsarItemSelecionado()
    {
        if (itemSelecionado != null)
        {
            Debug.Log("Você usou: " + itemSelecionado.nomeItem);
            inventoryManager.itenNoInventario.Remove(itemSelecionado);
            LimparDescricao();
            AtualizarInterface();
        }
    }
}
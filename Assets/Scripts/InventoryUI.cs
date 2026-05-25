using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Conexão com a Mochila")]
    public InventoryManager inventoryManager;

    [Header("Onde os itens vão aparecer")]
    public Transform areaGridItens;
    public GameObject slotPrefab;

    [Header("Painel de Descrição")]
    public TextMeshProUGUI textoNome;
    public TextMeshProUGUI textoEfeito;
    public Button botaoUsar;

    private TipoDeItem abaAtual = TipoDeItem.Consumivel;

    // NOVIDADE: A "memória" de qual item estamos olhando agora
    private ItemData itemSelecionado;

    void Start()
    {
        LimparDescricao();

        foreach (Transform filho in areaGridItens)
        {
            Destroy(filho.gameObject);
        }
    }

    public void ClicarAbaConsumiveis()
    {
        abaAtual = TipoDeItem.Consumivel;
        LimparDescricao();
        AtualizarInterface();
    }

    public void ClicarAbaEquipaveis()
    {
        abaAtual = TipoDeItem.Equipavel;
        LimparDescricao();
        AtualizarInterface();
    }

    public void ClicarAbaImportantes()
    {
        abaAtual = TipoDeItem.Importante;
        LimparDescricao();
        AtualizarInterface();
    }

    public void AtualizarInterface()
    {
        foreach (Transform filho in areaGridItens)
        {
            Destroy(filho.gameObject);
        }

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
        itemSelecionado = item; // Guarda o item na memória!

        botaoUsar.gameObject.SetActive(true);
        textoNome.text = item.nomeItem;
        textoEfeito.text = item.descricaoItem;
    }

    public void LimparDescricao()
    {
        itemSelecionado = null; // Esquece o item
        textoNome.text = "";
        textoEfeito.text = "";
        botaoUsar.gameObject.SetActive(false);
    }

    // NOVIDADE: A função que o botão USAR vai chamar!
    public void UsarItemSelecionado()
    {
        if (itemSelecionado != null)
        {
            // 1. O Efeito: Manda uma mensagem pro Console avisando que curou
            // (No futuro, é aqui que você vai mandar o código do Jogador aumentar a vida dele)
            Debug.Log("Você usou: " + itemSelecionado.nomeItem + "! Curou " + itemSelecionado.valorCuraHP + " de HP.");

            // 2. Tira o item da mochila
            inventoryManager.itenNoInventario.Remove(itemSelecionado);

            // 3. Limpa a tela da direita (o texto e o próprio botão somem)
            LimparDescricao();

            // 4. Atualiza a tela da esquerda (para a poção sumir do grid de vez)
            AtualizarInterface();
        }
    }
}
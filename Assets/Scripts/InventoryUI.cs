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

    void Start()
    {
        LimparDescricao(); // Garante que a direita comece limpa

        // Garante que a esquerda (o grid) comece vazia
        foreach (Transform filho in areaGridItens)
        {
            Destroy(filho.gameObject);
        }
    }

    // --- FUNÇÕES DAS ABAS ---
    public void ClicarAbaConsumiveis()
    {
        Debug.Log("O botão Consumíveis foi clicado com sucesso!"); // O nosso alarme
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
    // -------------------------

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

    // Essa função agora também faz o botão USAR aparecer
    public void MostrarDescricao(ItemData item)
    {
        botaoUsar.gameObject.SetActive(true); // Revela o botão
        textoNome.text = item.nomeItem;
        textoEfeito.text = item.descricaoItem + "\n\nCura HP: " + item.valorCuraHP;
    }

    // A MÁGICA DO POLIMENTO: Esconde tudo!
    public void LimparDescricao()
    {
        textoNome.text = ""; // Deixa o nome vazio
        textoEfeito.text = ""; // Deixa o efeito vazio
        botaoUsar.gameObject.SetActive(false); // Deixa o botão invisível
    }
}
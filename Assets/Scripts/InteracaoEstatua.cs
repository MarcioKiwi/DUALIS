using UnityEngine;

public class InteracaoEstatua : MonoBehaviour
{
    [Header("Conexões de UI (Tutorial)")]
    public DialogueManager gerenciadorDeDialogo;
    public GameObject painelDeEscolhas;
    public GameObject botaoContinuar;

    [Header("A Loja Clandestina (Painéis)")]
    public GameObject painelLoja_Pai;
    public GameObject painelLoja_Principal; // Onde ficam os botões: Comprar, Upar, Sair
    public GameObject painelLoja_Comprar;   // Sub-menu de Itens
    public GameObject painelLoja_Upar;      // Sub-menu de Atributos

    [Header("Itens da Loja")]
    public ItemData pocaoParaVender;

    [Header("Indicador de Interação")]
    public GameObject indicadorE; 

    private bool jogadorPerto = false;
    private bool estaConversando = false; 

    void Start()
    {
        
        if (indicadorE != null)
        {
            indicadorE.SetActive(false);
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E) && !estaConversando)
        {
            
            if (indicadorE != null) indicadorE.SetActive(false);
            estaConversando = true;

            ConversarComEstatua();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPerto = true;

           
            if (indicadorE != null && !estaConversando)
            {
                indicadorE.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPerto = false;

            
            if (indicadorE != null)
            {
                indicadorE.SetActive(false);
            }

            
            if (estaConversando)
            {
                BotaoSair();
            }
        }
    }

    public void ConversarComEstatua()
    {
        // 1. TUTORIAL: Primeira vez que conversa (Ainda não decidiu sobre a poção)
        if (PlayerPrefs.GetInt("ZumbiTutorialMorto", 0) == 1 && PlayerPrefs.GetInt("PocaoDecidida", 0) == 0)
        {
            string[] falas = { "Até que não é tão fraco. Se quer ficar mais forte, esta poção lhe ajudará." };
            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);

            painelDeEscolhas.SetActive(true);
            botaoContinuar.SetActive(false);
        }
        // 2. LOJA CLANDESTINA: Já passou do tutorial
        else if (PlayerPrefs.GetInt("PocaoDecidida", 0) == 1)
        {
            string[] falas = { "Sinto o cheiro de sanidade em você... Deseja trocar fragmentos de sanidade por poder?" };
            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);

            painelLoja_Pai.SetActive(true);

            painelLoja_Principal.SetActive(true);
            painelLoja_Comprar.SetActive(false);
            painelLoja_Upar.SetActive(false);

            botaoContinuar.SetActive(false);
        }
        // 3. ANTES DO TUTORIAL (Ainda não matou o zumbi)
        else
        {
            string[] falas = { "Há uma criatura adiante. Mostre-me do que é capaz." };
            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);
        }
    }

    // ==========================================
    // BOTÕES DO TUTORIAL ORIGINAL
    // ==========================================
    public void EscolherBeber()
    {
        painelDeEscolhas.SetActive(false);
        botaoContinuar.SetActive(true);

        PlayerPrefs.SetInt("PocaoDecidida", 1);
        PlayerPrefs.SetInt("LevelAdvogado", 2);

        string[] falas = { "O líquido queima sua garganta... Mas parece apenas um suco normal." };
        gerenciadorDeDialogo.MostrarFala("Voz Misteriosa", falas);
    }

    public void EscolherRecusar()
    {
        painelDeEscolhas.SetActive(false);
        botaoContinuar.SetActive(true);

        PlayerPrefs.SetInt("PocaoDecidida", 1);

        string[] falas = { "Interessante... Ser cauteloso é bom. Escolha com cuidado em que confiar." };
        gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);
    }

    // ==========================================
    // NAVEGAÇÃO DA LOJA CLANDESTINA
    // ==========================================
    public void BotaoComprar()
    {
        painelLoja_Principal.SetActive(false);
        painelLoja_Comprar.SetActive(true); // Abre a vitrine de itens
    }

    public void BotaoUpar()
    {
        painelLoja_Principal.SetActive(false);
        painelLoja_Upar.SetActive(true); // Abre o menu de atributos
    }

    // Função bônus para voltar do sub-menu para o menu principal da loja
    public void BotaoVoltar()
    {
        painelLoja_Comprar.SetActive(false);
        painelLoja_Upar.SetActive(false);
        painelLoja_Principal.SetActive(true);
    }

    public void BotaoSair()
    {
        painelLoja_Pai.SetActive(false);
        painelLoja_Principal.SetActive(false);
        painelLoja_Comprar.SetActive(false);
        painelLoja_Upar.SetActive(false);
        botaoContinuar.SetActive(true);
        gerenciadorDeDialogo.EsconderDialogo();

        // 🟢 ADICIONADO: Libera para mostrar o 'E' novamente e marca fim da conversa
        estaConversando = false;
        if (jogadorPerto && indicadorE != null)
        {
            indicadorE.SetActive(true);
        }
    }

    // ==========================================
    // LÓGICA DE COMPRA (ITENS)
    // ==========================================
    public void ComprarPocao()
    {
        int moedaAtual = PlayerPrefs.GetInt("PontosPsicologico", 0);
        int precoPocao = 20; // Preço fixo da poção

        if (moedaAtual >= precoPocao)
        {
            PlayerPrefs.SetInt("PontosPsicologico", moedaAtual - precoPocao);
            FindFirstObjectByType<InventoryManager>().AdicionarItem(pocaoParaVender);

            string[] falas = { "Uma troca justa. A poção está na sua mochila." };
            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);
        }
        else
        {
            string[] falas = { "Sanidade insuficiente para este item." };
            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);
        }

        BotaoVoltar(); // Volta para a loja principal
    }

    // ==========================================
    // LÓGICA DE UPAR (ATRIBUTOS)
    // ==========================================
    private int precoAtributo = 50; // Preço para qualquer upgrade de atributo

    public void UparVida()
    {
        TentarUparAtributo("BonusVida", 10, "Sua vitalidade se expande.");
    }

    public void UparMana()
    {
        TentarUparAtributo("BonusMana", 5, "Sua energia mística flui mais forte.");
    }

    public void UparForca()
    {
        TentarUparAtributo("BonusForca", 2, "Seus músculos se contorcem com novo poder.");
    }

    public void UparPsicologico()
    {
        TentarUparAtributo("BonusPsicologico", 10, "Sua mente se blinda contra o abismo.");
    }

    private void TentarUparAtributo(string nomeDoSave, int quantidadeDeBonus, string falaDeSucesso)
    {
        int moedaAtual = PlayerPrefs.GetInt("PontosPsicologico", 0);

        if (moedaAtual >= precoAtributo)
        {
            PlayerPrefs.SetInt("PontosPsicologico", moedaAtual - precoAtributo);

            int bonusAtual = PlayerPrefs.GetInt(nomeDoSave, 0);
            PlayerPrefs.SetInt(nomeDoSave, bonusAtual + quantidadeDeBonus);

            string[] falas = { falaDeSucesso };
            gerenciadorDeDialogo.MostrarFala("Voz Misteriosa", falas);
        }
        else
        {
            string[] falas = { "Sanidade insuficiente. Você precisa de " + precoAtributo + " pontos." };
            gerenciadorDeDialogo.MostrarFala("Estátua Arcana", falas);
        }

        BotaoVoltar(); // Volta para a loja principal
    }
}
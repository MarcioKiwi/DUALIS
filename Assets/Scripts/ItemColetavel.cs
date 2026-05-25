using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    [Header("Qual item é este?")]
    public ItemData dadoDoItem; // Aqui você vai arrastar o arquivo da Poção!

    // Essa função roda sozinha quando alguém entra no "Trigger" (área) da poção
    private void OnTriggerEnter2D(Collider2D outro)
    {
        // Verifica se quem pisou na poção foi o Jogador (lembra da Tag?)
        if (outro.CompareTag("Player"))
        {
            // Procura o cérebro do inventário na cena
            InventoryManager mochila = FindObjectOfType<InventoryManager>();

            if (mochila != null)
            {
                // Adiciona a poção na lista do inventário!
                mochila.itenNoInventario.Add(dadoDoItem);

                Debug.Log("Você pegou o item: " + dadoDoItem.nomeItem);

                // Destrói a poção do chão para ela sumir da rua
                Destroy(gameObject);
            }
        }
    }
}

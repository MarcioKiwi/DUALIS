using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public List<ItemData> itenNoInventario = new List<ItemData>();

    public void AdicionarItem(ItemData novoItem)
    {
        itenNoInventario.Add(novoItem);
        Debug.Log("Você pegou: " + novoItem.nomeItem);
    }
}

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Quem a câmera deve seguir?")]
    public Transform alvo;

    [Header("Configurações")]
    public float suavidade = 5f; // Quanto maior o número, mais rápida/dura é a câmera

    // Usamos LateUpdate em vez de Update para a câmera. 
    // Assim ela só se move DEPOIS que o personagem já andou, evitando "engasgos" na imagem.
    void LateUpdate()
    {
        if (alvo != null)
        {
            // Pega a posição (X e Y) do personagem, mas mantém o Z da câmera (que descobrimos ser vital!)
            Vector3 posicaoDesejada = new Vector3(alvo.position.x, alvo.position.y, transform.position.z);

            // Move a câmera suavemente da posição atual para a posição do personagem
            transform.position = Vector3.Lerp(transform.position, posicaoDesejada, suavidade * Time.deltaTime);
        }
    }
}

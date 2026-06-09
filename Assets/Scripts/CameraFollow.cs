using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configurações Básicas")]
    public Transform alvo; 
    public float velocidade = 5f;

    [Header("Limites do Mapa")]
    public Vector2 limiteMinimo; // Canto inferior esquerdo
    public Vector2 limiteMaximo; // Canto superior direito

    void LateUpdate()
    {
        if (alvo != null)
        {
            float alvoX = alvo.position.x;
            float alvoY = alvo.position.y;

            // Aqui a câmera é proibida de passar dos limites
            float xLimitado = Mathf.Clamp(alvoX, limiteMinimo.x, limiteMaximo.x);
            float yLimitado = Mathf.Clamp(alvoY, limiteMinimo.y, limiteMaximo.y);

            Vector3 posicaoDesejada = new Vector3(xLimitado, yLimitado, -10f);
            transform.position = Vector3.Lerp(transform.position, posicaoDesejada, velocidade * Time.deltaTime);
        }
    }
}

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 5f;

    [Header("Componentes")]
    public Rigidbody2D rb;
    public Animator anim;

    private Vector2 movement;

    void Start()
    {
        // 1. Checa se o jogador veio de uma vitória e já andou pelo mapa antes
        if (PlayerPrefs.GetInt("JogadorMorreu", 0) == 0 && PlayerPrefs.HasKey("PosX"))
        {
            // 2. Se ganhou, puxa a memória de onde ele estava pisando
            float xSalvo = PlayerPrefs.GetFloat("PosX");
            float ySalvo = PlayerPrefs.GetFloat("PosY");

            // 3. Teletransporta o herói de volta para a cara do monstro derrotado
            transform.position = new Vector3(xSalvo, ySalvo, transform.position.z);
        }
        else
        {
            // Se o JogadorMorreu for 1 (perdeu), o código ignora a posição salva 
            // e deixa ele nascer no Spawn Point normal do Editor
        }
    }
    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        anim.SetFloat("Velocidade", movement.sqrMagnitude);

 
        if (movement != Vector2.zero)
        {
            anim.SetFloat("MoveX", movement.x);
            anim.SetFloat("MoveY", movement.y);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}

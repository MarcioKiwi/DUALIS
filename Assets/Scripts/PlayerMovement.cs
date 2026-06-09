using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 5f;

    [Header("Componentes")]
    public Rigidbody2D rb;
    public Animator anim;

    private Vector2 movement;

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

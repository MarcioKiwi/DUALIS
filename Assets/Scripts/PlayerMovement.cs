using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 5f;

    [Header("Componentes")]
    public Rigidbody2D rb;

    private Vector2 movement;

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}

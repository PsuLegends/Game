using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 2.4f;
    private float jumpPower = 6.2f;
    private bool isFacingRight = true;
    private float jumpTime = 0f;
    private float maxJumpTime = 0.5f; // Максимальное время нажатия для максимальной высоты

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    private float acceleration = 0.1f; // Ускорение для инерции
    private float deceleration = 0.2f; // Замедление для инерции

    private bool isJumping = false;

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        // Работа с прыжком
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            jumpTime = 0f; // Начинаем отсчет времени прыжка
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }

        if (Input.GetButton("Jump") && isGrounded())
        {
            jumpTime += Time.deltaTime;
            float dynamicJumpPower = Mathf.Lerp(jumpPower, jumpPower * 2f, jumpTime / maxJumpTime);
            if (rb.velocity.y > 0f) // Если персонаж еще не достиг апогея прыжка
            {
                rb.velocity = new Vector2(rb.velocity.x, dynamicJumpPower);
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); // Уменьшаем высоту прыжка при отпускании кнопки
        }

        Flip();
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayer);
    }

    private void FixedUpdate()
    {
        // Инерция для движения по горизонтали
        if (horizontal != 0f) // Инерция применяется только при движении
        {
            float targetSpeed = horizontal * speed;
            float velocityX = rb.velocity.x;

            if (Mathf.Abs(targetSpeed) > Mathf.Abs(velocityX))
            {
                velocityX += acceleration * Mathf.Sign(targetSpeed);
            }
            else
            {
                velocityX -= deceleration * Mathf.Sign(velocityX);
            }

            // Ограничиваем максимальную скорость
            velocityX = Mathf.Clamp(velocityX, -speed, speed);

            rb.velocity = new Vector2(velocityX, rb.velocity.y);
        }
        else
        {
            // Если нет ввода, плавно снижаем скорость
            float velocityX = rb.velocity.x;

            if (Mathf.Abs(velocityX) > 0.1f) // Если скорость не слишком мала
            {
                velocityX -= deceleration * Mathf.Sign(velocityX); // Плавное замедление
            }
            else
            {
                velocityX = 0f; // Останавливаем движение, когда скорость становится достаточно малой
            }

            rb.velocity = new Vector2(velocityX, rb.velocity.y);
        }

        // Инерция при прыжке
        if (isJumping && horizontal != 0f)
        {
            float velocityX = rb.velocity.x;

            // Добавляем инерцию к горизонтальной скорости при прыжке
            velocityX += acceleration * Mathf.Sign(horizontal);

            // Ограничиваем максимальную скорость, чтобы она не выходила за пределы
            velocityX = Mathf.Clamp(velocityX, -speed, speed);

            rb.velocity = new Vector2(velocityX, rb.velocity.y);
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}

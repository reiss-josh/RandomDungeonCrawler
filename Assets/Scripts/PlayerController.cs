using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float move, vMove;
    private bool keyJump, keyShoot;
    public float speed;
    private Rigidbody2D rb2d;

    // Start is called before the first frame update
    void Start()
    {
        speed = 6.5f;
        rb2d = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");
        vMove = Input.GetAxisRaw("Vertical");
        keyJump = Input.GetButtonDown("Jump");
        keyShoot = Input.GetButtonDown("Fire1");

        rb2d.velocity = new Vector2(move * speed, vMove * speed);
    }
}

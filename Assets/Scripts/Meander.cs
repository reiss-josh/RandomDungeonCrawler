using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meander : MonoBehaviour
{
    private Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float rand1 = Random.Range(-5.0f, 5.0f);
        float rand2 = Random.Range(-5.0f, 5.0f);
        rb2d.AddForce(new Vector2(rand1, rand2));
    }
}

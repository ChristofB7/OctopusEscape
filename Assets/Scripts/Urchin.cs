using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Urchin : MonoBehaviour
{
    public int moveRange = 10;
    private int current = 0;
    private bool up = true;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float v_smoothing = .05f;
    private Rigidbody2D body;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (current >= moveRange)
        {
            up = false;
        }
        else if (current <= -moveRange)
        {
            up = true;
        }
        if (up)
        {
            current += 1;
            Vector3 targetVelocity = new Vector2(0, 1 * speed);
            body.velocity = Vector3.SmoothDamp(body.position, targetVelocity, ref velocity, v_smoothing);
        }
        else
        {
            current -= 1;
            Vector3 targetVelocity = new Vector2(0, -1 * speed);
            body.velocity = Vector3.SmoothDamp(body.position, targetVelocity, ref velocity, v_smoothing);
        }
    }
}

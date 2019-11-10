using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallController : MonoBehaviour
{
    Vector3 dir;
    float speed, speedDecay = 2f, minSpeed = 0.1f, startSpeed = 20;

    private Rigidbody2D snowBallRigidbody2D;

    public float snowBallVelocity = 100f;
    public float lifeTime = .5f;

    private string enemy;

    // Start is called before the first frame update
    void Start()
    {
        snowBallRigidbody2D = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //snowBallRigidbody2D.velocity = new Vector2(Time.deltaTime * snowBallVelocity, snowBallRigidbody2D.velocity.y);
        Move();
    }

    public void Setup(Vector3 _dir, string _enemy)
    {
        enemy = _enemy;
        dir = _dir; //passed in from player
        speed = startSpeed; //start moving
    }

    private void Move()
    {
        speed -= speedDecay * speed * Time.fixedDeltaTime; //slow down the bullet over time
        if (speed < minSpeed)
        {
            speed = 0; //clamp down speed so it doesnt take too long to stop
        }
        Vector3 tempPos = transform.position; //capture current position
        tempPos += dir * speed * Time.fixedDeltaTime; //find new position
        transform.position = tempPos; //update position
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(enemy))
        {
            Destroy(collision.gameObject);
            Debug.Log("Morreu");
        }
    }
}

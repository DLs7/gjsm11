using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    int countSnow = 0;

    float horizontalMove;
    float verticalMove;

    private GameObject snowBall;
    public float moveSpeed = 100f;

    private Rigidbody2D playerRigidbody2D;
    public GameObject crossHair;

    private Animator animator;

    Vector3 mousePos, mouseVector, aim;
    bool canShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        snowBall = Resources.Load<GameObject>("Prefabs/SnowBall");
        crossHair.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        MoveCrossHair();
        Shoot();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInput()
    {
        horizontalMove = Input.GetAxis("HorizontalJoystick");
        verticalMove = Input.GetAxis("VerticalJoystick");
        animator.SetFloat("horizontalMove", horizontalMove);
        animator.SetFloat("verticalMove", verticalMove);
        GetMouseInput();
    }

    void Move()
    {
        Vector2 movement = new Vector2(horizontalMove * moveSpeed * Time.fixedDeltaTime, verticalMove * moveSpeed * Time.fixedDeltaTime);
        playerRigidbody2D.velocity = movement;
    }

    void Shoot()
    {
        if (Input.GetButtonDown("Fire1Joystick") && canShoot && aim.magnitude > 0.0f)
        {
            //shoot if the mouse button is held and its been enough time since last shot
            Quaternion spawnRot = Quaternion.identity; //no rotation, bullets here are round
            SnowBallController fireSnowBall = Instantiate(snowBall,new Vector3(playerRigidbody2D.transform.position.x, playerRigidbody2D.transform.position.y, 0), Quaternion.identity).GetComponent<SnowBallController>();
            fireSnowBall.Setup(aim, "Player2"); //give the bullet a direction to fly
            canShoot = false;
        }
    }

    void GetMouseInput()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //position of cursor in world
        mousePos.z = transform.position.z; //keep the z position consistant, since we're in 2d
        mouseVector = (mousePos - transform.position).normalized; //normalized vector from player pointing to cursor
        //mouseLeft = Input.GetMouseButton(0); //check left mouse button
    }

    private void MoveCrossHair()
    {
        aim = new Vector3(Input.GetAxis("AimHorizontal"), Input.GetAxis("AimVertical"), 0.0f);

        if (aim.magnitude > 0.0f)
        {
            aim.Normalize();
            aim *= 2f;
            crossHair.transform.localPosition = aim;
            crossHair.SetActive(true);
        }
        else
        {
            crossHair.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Passei");
        if (collision.gameObject.CompareTag("Snow") && !canShoot)
        {
            countSnow++;
            if (countSnow == 3)
            {
                countSnow = 0;
                canShoot = true;
            }
            Destroy(collision.gameObject);
            Debug.Log(countSnow);
        }
    }
}

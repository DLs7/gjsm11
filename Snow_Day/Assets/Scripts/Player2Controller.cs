using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Controller : MonoBehaviour
{
    private AudioManager audioManager;

    int countSnow = 0;

    float horizontalMove;
    float verticalMove;

    private GameObject snowBall;
    public float moveSpeed = 200f;

    private Rigidbody2D playerRigidbody2D;

    private Animator animator, hudAnimator;

    Vector3 mousePos, mouseVector, aim;
    bool canShoot = false;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        hudAnimator = GameObject.Find("HudP2").GetComponent<Animator>();
        snowBall = Resources.Load<GameObject>("Prefabs/SnowBall");
    }

    // Update is called once per frame
    void Update()
    {
        hudAnimator.SetInteger("countSnow", countSnow);
        GetInput();
        Shoot();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInput()
    {
        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");
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
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            audioManager.Play("throwing_efect");
            //shoot if the mouse button is held and its been enough time since last shot
            //Quaternion spawnRot = Quaternion.identity; //no rotation, bullets here are round
            SnowBallController fireSnowBall = Instantiate(snowBall, new Vector3(playerRigidbody2D.transform.position.x, playerRigidbody2D.transform.position.y, 0), Quaternion.identity).GetComponent<SnowBallController>();
            fireSnowBall.Setup(mouseVector, "Player1"); //give the bullet a direction to fly
            countSnow = 0;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Snow") && !canShoot)
        {
            countSnow++;
            if (countSnow == 3)
            {
                canShoot = true;
            }
            Destroy(collision.gameObject);
        }
    }
}

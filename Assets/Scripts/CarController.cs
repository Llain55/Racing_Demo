using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    public ParticleSystem ps;
    public GameObject leftBooster;
    public GameObject RightBooster;

    public float boostForce;
    public float rotateForce;
    public float acceleration;

    public float timeToRecentre = 1f;
    public float flickerOff = .5f;
    public float flickerOn = 0;

    bool held = false;
    public bool leftBoost = false;
    public bool rightBoost = false;
    public bool flipped = false;

    public bool flickOn = false;
    public bool flickOff = false;
    public bool isGrounded = false;

    [Header("Mesh Renderers for Flicker")]
    public MeshRenderer body;
    public MeshRenderer frontLeftWheel;
    public MeshRenderer frontRightWheel;
    public MeshRenderer rearLeftWheel;
    public MeshRenderer rearRightWheel;

    public BoxCollider boxCollider;

    public GameController gameController;
    public bool airbourne = false;
    bool drifitng = false;
    public float boostFuel = 100f;
    public BoostBar boostBar;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ps.gameObject.SetActive(false);
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        boostBar = GameObject.Find("Boost Bar").GetComponent<BoostBar>();
        boostBar.SetBoost(boostFuel);
    }

    private void Update()
    {
        if (flipped)
        {
            float yAxis = transform.localEulerAngles.y;
            Debug.Log("Y AXIS is: " + yAxis);

            timeToRecentre -= Time.deltaTime;

            if (timeToRecentre <= 0)
            {
                body.enabled = true;
                frontLeftWheel.enabled = true;
                frontRightWheel.enabled = true;
                rearRightWheel.enabled = true;
                rearLeftWheel.enabled = true;
                transform.rotation = Quaternion.Euler(0, yAxis, 0);
                timeToRecentre = 5f;
                flipped = false;
                flickOff = false;
                flickOn = false;
            }

            if (flickOff)
            {
                body.enabled = false;
                frontLeftWheel.enabled = false;
                frontRightWheel.enabled = false;
                rearRightWheel.enabled = false;
                rearLeftWheel.enabled = false;

                flickerOff -= Time.deltaTime;

                if (flickerOff <= 0)
                {
                    flickOn = true;
                    flickOff = false;
                    flickerOn = 1f;
                }
            }

            if (flickOn)
            {
                body.enabled = true;
                frontLeftWheel.enabled = true;
                frontRightWheel.enabled = true;
                rearRightWheel.enabled = true;
                rearLeftWheel.enabled = true;

                flickerOn -= Time.deltaTime;
                if (flickerOn <= 0)
                {
                    flickOff = true;
                    flickOn = false;
                    flickerOff = 0.5f;
                }
            }
        }



        if (held)
        {
            if (isGrounded)
            {
                if (acceleration < .75)
                {
                    acceleration += .025f;
                }

                transform.Translate(Vector3.forward * acceleration);
                Debug.Log(gameController.score);
            }
            else
            {
                if (acceleration > 0)
                {
                    acceleration -= .005f;
                    transform.Translate(Vector3.forward * acceleration);
                }
            }
        }
        else if (!held)
        {
            if (acceleration > 0)
            {
                acceleration -= .005f;
                transform.Translate(Vector3.forward * acceleration);
            }
        }

        if(airbourne)
        {
            gameController.AirTime();
        }
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(boxCollider.bounds.center, Vector3.down);
        RaycastHit hit;
        float paddingHeight = 0.22f;
        if (Physics.Raycast(ray,out hit,boxCollider.bounds.extents.y + paddingHeight))
        {
            if (hit.collider.gameObject.tag == "Ground")
            {
                isGrounded = true;
                Debug.DrawRay(boxCollider.bounds.center, Vector3.down * (boxCollider.bounds.extents.y + paddingHeight), Color.green);

            }
            else
            {
                isGrounded = false;
                Debug.DrawRay(boxCollider.bounds.center, Vector3.down * (boxCollider.bounds.extents.y + paddingHeight), Color.yellow);
            }
        }
        else
        {
            Debug.DrawRay(boxCollider.bounds.center, Vector3.down * (boxCollider.bounds.extents.y + paddingHeight), Color.red);
        }

        if (boostFuel > 0)
        {
            var velOverLife = ps.velocityOverLifetime;
            if (rb.angularVelocity.y < -5)
            {
                if (!ps.gameObject.activeSelf)
                {
                    ps.gameObject.SetActive(true);
                }
                ps.Play();
                velOverLife.orbitalY = 1.5f;
                ps.loop = true;
                drifitng = true;
                boostFuel -= .25f;
                boostBar.SetBoost(boostFuel);
                gameController.AirTime();
            }
            else if (rb.angularVelocity.y > 5)
            {
                if (!ps.gameObject.activeSelf)
                {
                    ps.gameObject.SetActive(true);
                }
                ps.Play();
                velOverLife.orbitalY = -1.5f;
                ps.loop = true;
                drifitng = true;
                boostFuel -= .25f;
                boostBar.SetBoost(boostFuel);
                gameController.AirTime();
            }
        }
        else
        {
            leftBoost = false;
            rightBoost = false;
            drifitng = false;
            ps.loop = false;
        }
        if (leftBoost)
        {
            rb.AddForceAtPosition(leftBooster.transform.forward * rotateForce, leftBooster.transform.position, ForceMode.Impulse);
        }
        else if (!leftBoost)
        {

        }

        if (rightBoost)
        {
            rb.AddForceAtPosition(RightBooster.transform.forward * rotateForce, RightBooster.transform.position, ForceMode.Impulse);
        }
    }
    public void Boost()
    {
        Debug.Log("BOOST!!");
        rb.AddRelativeForce(Vector3.forward * boostForce, ForceMode.Impulse);
    }

    public void BoostRight(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Boosting Right!");
            rightBoost = true;
        }
        else if (ctx.canceled)
        {
            rightBoost = false;
        }
    }

    public void BoostLeft(CallbackContext ctx)
    {
        Debug.Log("Boost Left!");
        if (ctx.performed)
        {
            Debug.Log("Boosting Left!");
            leftBoost = true;
        }
        else if (ctx.canceled)
        {
            leftBoost = false;
        }
    }

    public void Accelerate(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (!flipped)
            {
                Debug.Log("Holding + Accelerating");
                held = true;
            }
        }
        else if (ctx.canceled)
        {
            Debug.Log("Releasing + Deccelerating");
            held = false;
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Collecting BOOST!");
        if (other.tag == "BoostFuel")
        {
            if (boostFuel < 100)
            {
                boostFuel += 25;

                if (boostFuel > 100)
                {
                    boostFuel = 100;
                }
            }
            Destroy(other);
            gameController.noBoostInScene--;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.tag == "Ground")    
        {
            if(airbourne)
            {
                airbourne = false;
            }
        }
    }
}

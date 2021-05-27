using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//The playercontroller is only for demo purposes and thus the code quality is not really up to standard, as primary effort of the project has gone into the algorithm
public class PlayerControls : MonoBehaviour
{
    private Rigidbody rigid;
    public Camera playerCamera;
    public float MouseSensitivity = 1;
    public float MoveSpeed = 2;
    public float moveSmoothTime = 0.3f;
    public Image crosshair;
    public float JumpForce = 2;
    public float Gravity = 9.81f;
    public float raycastDistanceHammer;
    public float raycastDistanceGun;
    public Color[] colors;
    public Image colorIndicator;
    public GameObject hammer;
    public GameObject gun;
    public bool canMove = true;
    public AudioSource hammerSound;
    public AudioSource swingHammerAudio;
    public AudioSource gunSound;
    public AudioSource jumpSound;
    public AudioSource landingSound;
    public ParticleSystem gunParticles;
    public float hammerStartRotation;
    public float hammerEndRotation;
    public float hammerSwingSpeedForward;
    public float hammerSwingSpeedBackward;

    // Private variables for mousemovement and looking
    private bool inAir = false;
    private bool swingHammer = false;
    private bool swingHammerForward = true;
    private float swingHammerCounter = 0;
    private bool hammerEquipped = true;
    private Quaternion startHammerQuat;
    private Quaternion endHammerQuat;
    private int currentColor = 0;
    private float cameraPitch = 0.0f;
    private GameObject highlighted;
    private GameObject prevHighlight;
    // Private variables for movement
    private Vector2 currentDir = Vector2.zero;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector3 velocity = Vector3.zero;

    private CharacterController controller;
    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        colorIndicator.color = colors[currentColor];

        startHammerQuat = hammer.transform.localRotation;
        var startRot = startHammerQuat.eulerAngles;
        startRot.x = hammerStartRotation;
        startHammerQuat.eulerAngles = startRot;

        endHammerQuat = hammer.transform.localRotation;
        var endRot = endHammerQuat.eulerAngles;
        endRot.x = hammerEndRotation;
        endHammerQuat.eulerAngles = endRot;
    }

    void Update()
    {
        SwingHammer();
        MouseLook();
        if(canMove)UpdateMovement();      
        if (inAir && controller.isGrounded)
        {
            inAir = false;
            landingSound.Play();
        }
        UpdateRaycast();

        //Add some code for triggering highlight of mesh
        HandleInput();
        HandleColorChange();

    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (highlighted != null)
            {
                if (hammerEquipped)
                {
                    ResetHammerSwing();
                    swingHammer = true;
                    HammerJuice();
                    highlighted.SetActive(false);
                }
                else if (!hammerEquipped)
                {
                    GunJuice();
                    highlighted.GetComponent<HighLightMesh>().baseColor = colors[currentColor];
                }
            }
            else if(highlighted == null)
            {
                if (hammerEquipped)
                {
                    ResetHammerSwing();
                    swingHammer = true;
                    swingHammerAudio.Play();
                }
                else if (!hammerEquipped)
                {
                    GunJuice();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchTool();
        }

    }
    private void HammerJuice()
    {
        hammerSound.Play();
    }
    private void ResetHammerSwing()
    {
        var quat = hammer.transform.localRotation;
        quat.x = hammerStartRotation;
        hammer.transform.localRotation = quat;
        swingHammerCounter = 0f;
        swingHammerForward = true;
        swingHammer = false;
    }
    private void SwingHammer()
    {
        if (!hammer.activeInHierarchy)
        {
            ResetHammerSwing();
        }
        else
        {
            if (swingHammer)
            {
                if (swingHammerForward)
                {
                    swingHammerCounter += Time.deltaTime * hammerSwingSpeedForward;
                    var newRot = Quaternion.Lerp(startHammerQuat, endHammerQuat, swingHammerCounter);
                    hammer.transform.localRotation = newRot;
                    if(swingHammerCounter >= 1f)
                    {
                        swingHammerCounter = 0f;
                        swingHammerForward = false;
                    }
                }
                else if (!swingHammerForward)
                {
                    swingHammerCounter += Time.deltaTime * hammerSwingSpeedBackward;
                    var newRot = Quaternion.Lerp(endHammerQuat, startHammerQuat, swingHammerCounter);
                    hammer.transform.localRotation = newRot;
                    if (swingHammerCounter >= 1f)
                    {
                        ResetHammerSwing();
                    }
                }
            }
        }
    }
    private void GunJuice()
    {
        gunSound.Play();
        var main = gunParticles.main;
        main.startColor = colors[currentColor];
        gunParticles.Play();
    }
    private void SwitchTool()
    {
        hammerEquipped = !hammerEquipped;
        if (hammerEquipped)
        {
            hammer.SetActive(true);
            gun.SetActive(false);
        }
        else if (!hammerEquipped)
        {
            hammer.SetActive(false);
            gun.SetActive(true);
        }
    }

    private void HandleColorChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            currentColor = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            currentColor = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            currentColor = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            currentColor = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            currentColor = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
        {
            currentColor = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
        {
            currentColor = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
        {
            currentColor = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
        {
            currentColor = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
        {
            currentColor = 9;
        }

        colorIndicator.color = colors[currentColor];
    }


    private void UpdateMovement()
    {
        if (controller.isGrounded)
        {
            velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            velocity = transform.TransformDirection(velocity);
            velocity *= MoveSpeed;
            if (Input.GetButton("Jump"))
            {
                inAir = true;
                jumpSound.Play();
                velocity.y = JumpForce;
            }
        }
        else
        {
            velocity.x = Input.GetAxis("Horizontal") * MoveSpeed;
            velocity.z = Input.GetAxis("Vertical") * MoveSpeed;
            velocity = transform.TransformDirection(velocity);
        }

        velocity.y -= Gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void MouseLook()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        cameraPitch -= mouseDelta.y * MouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * mouseDelta.x * MouseSensitivity);

    }
    private void UpdateRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 100f))
        {
            var distance = 0f;
            if (hammerEquipped) distance = raycastDistanceHammer;
            else if (!hammerEquipped) distance = raycastDistanceGun;

            if (hit.distance < distance)
            {
                highlighted = hit.collider.gameObject;
                if (highlighted.CompareTag("Sculpture"))
                {
                    //highlight crosshair, and trigger highlight of mesh
                    crosshair.color = Color.white;
                    if (!hammerEquipped)
                    {
                        if(prevHighlight != null) prevHighlight.GetComponent<HighLightMesh>().highLightColor = Color.white;
                        highlighted.GetComponent<HighLightMesh>().highLightColor = colors[currentColor];
                    }
                    if (hammerEquipped)
                    {
                        highlighted.GetComponent<HighLightMesh>().highLightColor = Color.white;
                    }

                    if (highlighted != prevHighlight)
                    {
                        highlighted.GetComponent<HighLightMesh>().isHighlighted = true;
                        if (prevHighlight != null)
                        {
                            prevHighlight.GetComponent<HighLightMesh>().isHighlighted = false;
                        }
                    }
                    prevHighlight = highlighted;
                    return;
                }
            }
            
        }
        // found no Sculpture piece on raycast, make highlighted null and crosshair gray
        highlighted = null;
        if (prevHighlight != null)
        {
            prevHighlight.GetComponent<HighLightMesh>().isHighlighted = false;
            prevHighlight = null;
        }
        crosshair.color = Color.gray;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Private variables for mousemovement and looking
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
    }

    void Update()
    {
        MouseLook();
        UpdateMovement();
        UpdateRaycast();

        //Add some code for triggering highlight of mesh

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (highlighted != null)
            {
                highlighted.SetActive(false);
            }
        }
    }




    private void UpdateMovement()
    {
        if (controller.isGrounded)
        {
            velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            velocity = transform.TransformDirection(velocity);
            velocity *= MoveSpeed;
            if (Input.GetButton("Jump")) velocity.y = JumpForce;
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
            highlighted = hit.collider.gameObject;
            if (highlighted.CompareTag("Sculpture"))
            {
                //highlight crosshair, and trigger highlight of mesh
                crosshair.color = Color.white;

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

using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [Header("Mouse Look Settings")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 2f;

    private CharacterController controller;
    private PhotonView pv;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private float cameraPitch = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pv = GetComponent<PhotonView>();

        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject); // Destroy the other player's camera
            Destroy(GetComponentInChildren<CharacterController>());
            Destroy(GetComponentInChildren<Rigidbody>());
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!pv.IsMine) return;

        LookAround();
        MovePlayer();
    }

    private void LookAround()
    {
        Vector2 lookInput = InputManager.Instance.GetMouseDelta();

        // Horizontal rotation - rotate the player
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        // Vertical rotation - rotate the camera up/down
        cameraPitch -= lookInput.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        cameraHolder.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    private void MovePlayer()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = InputManager.Instance.GetPlayerMovement();
        Vector3 move = new Vector3(input.x, 0, input.y);

        // Move relative to player's forward
        move = transform.TransformDirection(move);
        controller.Move(move * Time.deltaTime * playerSpeed);

        // Jump
        if (InputManager.Instance.playerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // Gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}

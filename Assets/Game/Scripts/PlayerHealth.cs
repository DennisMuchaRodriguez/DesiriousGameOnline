using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    public TextMesh healthText;
    public Material aliveMaterial;
    public Material deadMaterial;
    public Renderer playerRenderer;

    [Header("Death UI")]
    public GameObject deathPanel;
    public Button respawnButton;

    private PlayerController playerController;
    private CharacterController characterController;
    private WeaponShooter weaponShooter;
    private bool isDead = false;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        weaponShooter = GetComponentInChildren<WeaponShooter>();

        currentHealth = maxHealth;
        UpdateHealthDisplay();

        if (photonView.IsMine)
        {
            deathPanel.SetActive(false);
            respawnButton.onClick.AddListener(Respawn);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviamos nuestros datos a otros jugadores
            stream.SendNext(currentHealth);
            stream.SendNext(isDead);
        }
        else
        {
            // Recibimos datos de otros jugadores
            currentHealth = (float)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
            UpdateHealthDisplay();

            if (isDead)
            {
                ApplyDeathEffects();
            }
            else
            {
                ApplyRespawnEffects();
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, Player attacker)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthDisplay()
    {
        if (healthText != null)
        {
            healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{maxHealth}";

            // Cambiar color según vida
            float healthPercent = currentHealth / maxHealth;
            healthText.color = Color.Lerp(Color.red, Color.green, healthPercent);
        }
    }

    private void Die()
    {
        isDead = true;
        photonView.RPC("RPC_Die", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_Die()
    {
        ApplyDeathEffects();

        if (photonView.IsMine)
        {
            deathPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void ApplyDeathEffects()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material = deadMaterial;
        }

        if (photonView.IsMine)
        {
            playerController.enabled = false;
            characterController.enabled = false;
            weaponShooter.enabled = false;
        }
    }

    public void Respawn()
    {
        photonView.RPC("RPC_Respawn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        currentHealth = maxHealth;
        isDead = false;

        ApplyRespawnEffects();
        UpdateHealthDisplay();
        photonView.RPC("RPC_Reposition", RpcTarget.All);
    }

    private void ApplyRespawnEffects()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material = aliveMaterial;
        }

        if (photonView.IsMine)
        {
            playerController.enabled = true;
            characterController.enabled = true;
            weaponShooter.enabled = true;

            deathPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    [PunRPC]
    private void RPC_Reposition()
    {
        if (SpawnManager.Instance != null)
        {
            Transform spawnPoint = SpawnManager.Instance.GetRandomSpawnPoint();
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.position;
                transform.rotation = spawnPoint.rotation;
            }
        }
    }
}
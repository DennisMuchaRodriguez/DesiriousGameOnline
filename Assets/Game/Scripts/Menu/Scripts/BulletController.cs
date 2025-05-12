using UnityEngine;
using Photon.Pun;
using static UnityEngine.UI.GridLayoutGroup;
using Photon.Realtime;

public class BulletController : MonoBehaviourPun
{
    public float damage = 10f;
    private Player owner;

    public void SetOwner(Player player)
    {
        owner = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        PhotonView otherView = other.GetComponent<PhotonView>();
        if (otherView != null && otherView.Owner == owner) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage, owner);
            PhotonNetwork.Destroy(gameObject);
        }
        else if (!other.isTrigger && !other.CompareTag("Bullet"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
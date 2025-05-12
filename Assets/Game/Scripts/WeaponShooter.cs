using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WeaponShooter : MonoBehaviourPunCallbacks
{
    public string bulletPrefabName = "Bullet";
    public Transform firepoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private PhotonView parentPhotonView;

    private void Awake()
    {
        // Obtener el PhotonView del padre
        parentPhotonView = GetComponentInParent<PhotonView>();

        if (parentPhotonView == null)
        {
            Debug.LogError("No se encontró PhotonView en el padre", this);
        }
    }

    private void Update()
    {
        if (!parentPhotonView.IsMine) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        parentPhotonView.RPC("RPC_Shoot", RpcTarget.AllViaServer, parentPhotonView.ViewID);
    }

    [PunRPC]
    private void RPC_Shoot(int shooterId)
    {
        if (parentPhotonView.ViewID == shooterId)
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefabName, firepoint.position, firepoint.rotation);

            if (bullet != null)
            {
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = firepoint.forward * bulletSpeed;

                    // Asignar el dueño correcto (del padre)
                    BulletController bulletController = bullet.GetComponent<BulletController>();
                    if (bulletController != null)
                    {
                        bulletController.SetOwner(parentPhotonView.Owner);
                    }
                }

                if (bullet.GetPhotonView().IsMine)
                {
                    Destroy(bullet, 3f);
                }
            }
        }
    }
}
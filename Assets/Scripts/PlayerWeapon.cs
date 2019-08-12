using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviour
{
  [Header("Stats")]
  public int damage;
  public int curAmmo;
  public int maxAmmo;
  public float bulletSpeed;
  public float shootRate;

  private float lastShootTime;

  public GameObject bulletPrefab;
  public Transform bulletSpawnPos;

  private PlayerController player;

  void Awake()
  {
    // get components
    player = GetComponent<PlayerController>();
  }

  public void TryShoot()
  {
    // can we shoot?
    if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
      return;

    curAmmo--;
    lastShootTime = Time.time;

    // update ammo ui

    // spawn bullet
    player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.position, Camera.main.transform.forward);
  }

  [PunRPC]
  void SpawnBullet(Vector3 pos, Vector3 dir)
  {
    // spawn and orient
    GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
    bulletObj.transform.forward = dir;

    // get bullet script
    Bullet bulletScript = bulletObj.GetComponent<Bullet>();

    // init bullet with velocity
    bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
    bulletScript.rig.velocity = dir * bulletSpeed;
  }

}

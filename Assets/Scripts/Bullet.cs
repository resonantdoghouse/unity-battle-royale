using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  private int damage;
  private int attackerId;
  private bool isMine;

  public Rigidbody rig;

  public AudioClip shootSound;
  public AudioClip collisionSound;
  public AudioSource audio;

  void Start()
  {
    audio = GetComponent<AudioSource>();
  }

  public void Initialize(int damage, int attackerId, bool isMine)
  {
    this.damage = damage;
    this.attackerId = attackerId;
    this.isMine = isMine;

    audio.PlayOneShot(shootSound, 0.50f);

    // time until destroy
    Destroy(gameObject, 42.0f);
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && isMine)
    {
      audio.PlayOneShot(collisionSound, 0.50f);
      PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

      if (player.id != attackerId)
        player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
    }

    if (other.gameObject.name == "Terrain")
    {
      audio.PlayOneShot(collisionSound, 0.50f);
      // Debug.Log("Terrain Hit");
      rig.velocity = new Vector3(0, 1, 0);

      // change scale of bullet
      gameObject.transform.localScale += new Vector3(1, 1, 1);
      // rig.transform.forward =  new Vector3(0,1,0);
    }
    else
    {
      Destroy(gameObject);
    }


  }
}

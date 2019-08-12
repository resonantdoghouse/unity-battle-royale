using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
  Health,
  Ammo
}

public class Pickup : MonoBehaviour
{
  public PickupType type;
  public int value;

  void OnTriggerEnter(Collider other)
  {
    if (!PhotonNetwork.IsMasterClient)
      return;

    if (other.CompareTag("Player"))
    {
      // get the player
      PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

    }
  }
}

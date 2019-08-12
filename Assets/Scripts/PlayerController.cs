using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
  [Header("Info")]
  public int id;
  private int curAttackerId;

  [Header("Stats")]
  public float moveSpeed;
  public float jumpForce;
  public int curHp;
  public int maxHp;
  public int kills;
  public bool dead;

  private bool flashingDamage;


  [Header("Compontents")]
  public Rigidbody rig;
  public Player photonPlayer;
  public PlayerWeapon weapon;
  public MeshRenderer mr;

  [PunRPC]
  public void Initialize(Player player)
  {
    id = player.ActorNumber;
    photonPlayer = player;

    GameManager.instance.players[id - 1] = this;

    // if not local player
    if (!photonView.IsMine)
    {
      GetComponentInChildren<Camera>().gameObject.SetActive(false);
      rig.isKinematic = true;
    }
    else
    {
      GameUI.instance.Initialize(this);
    }
  }

  void Update()
  {
    if (!photonView.IsMine || dead)
      return;

    Move();

    if (Input.GetKeyDown(KeyCode.Space))
      TryJump();

    if (Input.GetMouseButtonDown(0))
      weapon.TryShoot();

  }

  void Move()
  {
    // get input axis
    float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");

    // calc direction rel to location facing
    Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
    dir.y = rig.velocity.y;

    // set as velocity
    rig.velocity = dir;
  }

  void TryJump()
  {
    // create raycast facing down
    Ray ray = new Ray(transform.position, Vector3.down);

    // shoot raycast
    if (Physics.Raycast(ray, 1.5f))
      rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
  }

  [PunRPC]
  public void TakeDamage(int attackerId, int damage)
  {
    if (dead)
      return;

    curHp -= damage;
    curAttackerId = attackerId;

    // flash target red
    photonView.RPC("DamageFlash", RpcTarget.Others);

    // update health bar UI
    GameUI.instance.UpdateHealthBar();

    // die if no health left
    if (curHp <= 0)
      photonView.RPC("Die", RpcTarget.All);
  }

  [PunRPC]
  void DamageFlash()
  {
    if (flashingDamage)
      return;

    StartCoroutine(DamageFlashCoRoutine());

    IEnumerator DamageFlashCoRoutine()
    {
      flashingDamage = true;
      Color defaultColor = mr.material.color;
      mr.material.color = Color.red;

      yield return new WaitForSeconds(0.05F);

      mr.material.color = defaultColor;
      flashingDamage = false;
    }
  }

  [PunRPC]
  void Die()
  {
    curHp = 0;
    dead = true;
    GameManager.instance.alivePlayers--;
    // host will check the win condition
    if (PhotonNetwork.IsMasterClient)
      GameManager.instance.CheckWinCondition();

    // if is our local player?
    if (photonView.IsMine)
    {
      if (curAttackerId != 0)
        GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);

      // set cam to spectater
      GetComponentInChildren<CameraController>().SetAsSpectator();

      // disable physics and hide player
      rig.isKinematic = true;
      transform.position = new Vector3(0, -50, 0);
    }
  }

  [PunRPC]
  public void AddKill()
  {
    kills++;

    // update the UI
    GameUI.instance.UpdatePlayerInfoText();
  }

  [PunRPC]
  public void Heal(int AmountToHeal)
  {
    curHp = Mathf.Clamp(curHp + AmountToHeal, 0, maxHp);
    // update the UI
    GameUI.instance.UpdateHealthBar();
  }
}

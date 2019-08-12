using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("Look Sensitivity")]
  public float sensX;
  public float sensY;

  [Header("Clamping")]
  public float minY;
  public float maxY;

  [Header("Spectator")]
  public float spectatorMoveSpeed;

  private float rotX;
  private float rotY;

  private bool isSpectator;

  void Start()
  {
    // lock cursor to middle of screen
    Cursor.lockState = CursorLockMode.Locked;
  }

  void LateUpdate()
  {
    // get mouse move inputs
    rotX += Input.GetAxis("Mouse X") * sensX;
    rotY += Input.GetAxis("Mouse Y") * sensY;

    // clamp vert location
    rotY = Mathf.Clamp(rotY, minY, maxY);

    // spectating
    if (isSpectator)
    {
      // rotate cam vert
      transform.rotation = Quaternion.Euler(-rotY, rotX, 0);

      // movement
      float x = Input.GetAxis("Horizontal");
      float z = Input.GetAxis("Vertical");
      float y = 0;

      if (Input.GetKey(KeyCode.E))
        y = 1;
      else if (Input.GetKey(KeyCode.Q))
        y = -1;

      Vector3 dir = transform.right * x + transform.up * y + transform.forward * z;
      transform.position += dir * spectatorMoveSpeed * Time.deltaTime;
    }
    else
    {
        // rotate cam vert
        transform.localRotation = Quaternion.Euler(-rotY, 0, 0);

        // rotate player horz
        transform.parent.rotation = Quaternion.Euler(0, rotX, 0);
    }
  }

  public void SetAsSpectator(){
    isSpectator = true;
    transform.parent = null;
  }
}

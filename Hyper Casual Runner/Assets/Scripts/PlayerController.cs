using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float limitX;//the limit on how far the character goes to right or left
    public float runningSpeed;
    public float xSpeed; //how fast our character slides to the sides
    public float jumpSpeed;
    private float _currentRunningSpeed;
    void Start()
    {
        _currentRunningSpeed = runningSpeed;
    }

    void Update()
    {
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if (Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }
        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    public void changeRunningSpeed(int _change)//speed boost activated
    {
        _currentRunningSpeed = _change;
    }

    public void backToRunningSpeed()//speed boost deactivated,back to normal speed
    {
        _currentRunningSpeed = runningSpeed;
    }

}

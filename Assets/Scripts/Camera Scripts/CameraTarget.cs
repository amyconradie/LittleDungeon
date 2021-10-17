using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public Transform target;
    private bool centered = false;

    void FixedUpdate()
    {
        if (target != null)
        {
            if (centered)
            {
                transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(target.position.x, target.position.y + (GetComponent<UnityEngine.Camera>().orthographicSize / 3), transform.position.z);
            }

        }
        
    }


    public void setCharacter(Transform character)
    {
        this.target = character;
    }

    public void setCentered(bool centered)
    {
        this.centered = centered;
    }


}

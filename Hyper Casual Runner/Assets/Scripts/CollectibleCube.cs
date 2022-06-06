using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleCube : MonoBehaviour
{
    bool isCollected;
    bool isOrdered;
    bool isDestroyed;
    float index;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public bool GetIsCollected()
    {
        return isCollected;
    }

    public void itIsCollected()
    {
        isCollected = true;
    }
    public void setIndex(float index)
    {
        this.index = index;
    }

    public void setCollected()
    {
        if (isCollected == true)
        {
            transform.localPosition = new Vector3(0, -index + 0.5f, 0);
        }
    }

    public void changePos(float cIndex)
    {
        transform.localPosition = new Vector3(0, -cIndex + 2.5f, 0);
    }
}

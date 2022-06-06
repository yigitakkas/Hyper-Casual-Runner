using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryScript : MonoBehaviour
{
    float index;
    public int compareNumber;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void setIndex(float index)
    {
        this.index = index;
        setCollected();
    }

    public void setCollected()
    {
        if (transform.parent != null)
        {
            transform.localPosition = new Vector3(0, -index + 2.5f, 0);
        }
    }
}

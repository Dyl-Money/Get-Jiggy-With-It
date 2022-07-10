using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomBounds : MonoBehaviour
{
    //3
    public Vector3 smallboundsMax;
    public Vector3 smallboundsMin;
    //3.1
    public Vector3 smallQboundsMax;
    public Vector3 smallQboundsMin;
    //3.2
    public Vector3 smallWboundsMax;
    public Vector3 smallWboundsMin;
    //3.3
    public Vector3 smallEboundsMax;
    public Vector3 smallEboundsMin;
    //3.4
    public Vector3 smallRboundsMax;
    public Vector3 smallRboundsMin;
    //3.5
    public Vector3 smallTboundsMax;
    public Vector3 smallTboundsMin;
    //3.6
    public Vector3 smallYboundsMax;
    public Vector3 smallYboundsMin;
    //3.7
    public Vector3 smallUboundsMax;
    public Vector3 smallUboundsMin;
    //3.8
    public Vector3 smallIboundsMax;
    public Vector3 smallIboundsMin;
    //3.9
    public Vector3 smallOboundsMax;
    public Vector3 smallOboundsMin;
    //4
    public Vector3 smallPboundsMax;
    public Vector3 smallPboundsMin;
    //4.1
    public Vector3 bigQboundsMax;
    public Vector3 bigQboundsMin;
    //4.2
    public Vector3 bigWboundsMax;
    public Vector3 bigWboundsMin;
    //4.3
    public Vector3 bigEboundsMax;
    public Vector3 bigEboundsMin;
    //4.4
    public Vector3 bigRboundsMax;
    public Vector3 bigRboundsMin;
    //4.5
    public Vector3 bigTboundsMax;
    public Vector3 bigTboundsMin;
    //4.6
    public Vector3 bigYboundsMax;
    public Vector3 bigYboundsMin;
    //4.7
    public Vector3 bigUboundsMax;
    public Vector3 bigUboundsMin;
    //4.8
    public Vector3 bigIboundsMax;
    public Vector3 bigIboundsMin;
    //4.9
    public Vector3 bigOboundsMax;
    public Vector3 bigOboundsMin;
    //5
    public Vector3 bigboundsMax;
    public Vector3 bigboundsMin;

    void Start()
    {
        GetComponent<Camera>().orthographicSize = 8; // Size u want to start with
    }

    // Update is called once per frame
    void Update()
    {
        //3
        if (GetComponent<Camera>().orthographicSize > 5.99 && GetComponent<Camera>().orthographicSize < 6.09)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallboundsMin.x, smallboundsMax.x), 
            Mathf.Clamp(transform.position.y, smallboundsMin.y, smallboundsMax.y),Mathf.Clamp(transform.position.z, smallboundsMin.z, smallboundsMax.z));
        }
        //3.1
        if (GetComponent<Camera>().orthographicSize > 6.09 && GetComponent<Camera>().orthographicSize < 6.19)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallQboundsMin.x, smallQboundsMax.x),
            Mathf.Clamp(transform.position.y, smallQboundsMin.y, smallQboundsMax.y), Mathf.Clamp(transform.position.z, smallQboundsMin.z, smallQboundsMax.z));
        }
        //3.2
        if (GetComponent<Camera>().orthographicSize > 6.19 && GetComponent<Camera>().orthographicSize < 6.29)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallWboundsMin.x, smallWboundsMax.x),
            Mathf.Clamp(transform.position.y, smallWboundsMin.y, smallWboundsMax.y), Mathf.Clamp(transform.position.z, smallWboundsMin.z, smallWboundsMax.z));
        }
        //3.3
        if (GetComponent<Camera>().orthographicSize > 6.29 && GetComponent<Camera>().orthographicSize < 6.39)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallEboundsMin.x, smallEboundsMax.x),
            Mathf.Clamp(transform.position.y, smallEboundsMin.y, smallEboundsMax.y), Mathf.Clamp(transform.position.z, smallEboundsMin.z, smallEboundsMax.z));
        }
        //3.4
        if (GetComponent<Camera>().orthographicSize > 6.39 && GetComponent<Camera>().orthographicSize < 6.49)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallRboundsMin.x, smallRboundsMax.x),
            Mathf.Clamp(transform.position.y, smallRboundsMin.y, smallRboundsMax.y), Mathf.Clamp(transform.position.z, smallRboundsMin.z, smallRboundsMax.z));
        }
        //3.5
        if (GetComponent<Camera>().orthographicSize > 6.49 && GetComponent<Camera>().orthographicSize < 6.59)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallTboundsMin.x, smallTboundsMax.x),
            Mathf.Clamp(transform.position.y, smallTboundsMin.y, smallTboundsMax.y), Mathf.Clamp(transform.position.z, smallTboundsMin.z, smallTboundsMax.z));
        }
        //3.6
        if (GetComponent<Camera>().orthographicSize > 6.59 && GetComponent<Camera>().orthographicSize < 6.69)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallYboundsMin.x, smallYboundsMax.x),
            Mathf.Clamp(transform.position.y, smallYboundsMin.y, smallYboundsMax.y), Mathf.Clamp(transform.position.z, smallYboundsMin.z, smallYboundsMax.z));
        }
        //3.7
        if (GetComponent<Camera>().orthographicSize > 6.69 && GetComponent<Camera>().orthographicSize < 6.79)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallUboundsMin.x, smallUboundsMax.x),
            Mathf.Clamp(transform.position.y, smallUboundsMin.y, smallUboundsMax.y), Mathf.Clamp(transform.position.z, smallUboundsMin.z, smallUboundsMax.z));
        }
        //3.8
        if (GetComponent<Camera>().orthographicSize > 6.79 && GetComponent<Camera>().orthographicSize < 6.89)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallIboundsMin.x, smallIboundsMax.x),
            Mathf.Clamp(transform.position.y, smallIboundsMin.y, smallIboundsMax.y), Mathf.Clamp(transform.position.z, smallIboundsMin.z, smallIboundsMax.z));
        }
        //3.9
        if (GetComponent<Camera>().orthographicSize > 6.89 && GetComponent<Camera>().orthographicSize < 6.99)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallOboundsMin.x, smallOboundsMax.x),
            Mathf.Clamp(transform.position.y, smallOboundsMin.y, smallOboundsMax.y), Mathf.Clamp(transform.position.z, smallOboundsMin.z, smallOboundsMax.z));
        }
        //4
        if (GetComponent<Camera>().orthographicSize > 6.99 && GetComponent<Camera>().orthographicSize < 7.09)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, smallPboundsMin.x, smallPboundsMax.x),
            Mathf.Clamp(transform.position.y, smallPboundsMin.y, smallPboundsMax.y), Mathf.Clamp(transform.position.z, smallPboundsMin.z, smallPboundsMax.z));
        }
        //4.1
        if (GetComponent<Camera>().orthographicSize > 7.09 && GetComponent<Camera>().orthographicSize < 7.19)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigQboundsMin.x, bigQboundsMax.x),
            Mathf.Clamp(transform.position.y, bigQboundsMin.y, bigQboundsMax.y), Mathf.Clamp(transform.position.z, bigQboundsMin.z, bigQboundsMax.z));
        }
        //4.2
        if (GetComponent<Camera>().orthographicSize > 7.19 && GetComponent<Camera>().orthographicSize < 7.29)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigWboundsMin.x, bigWboundsMax.x),
            Mathf.Clamp(transform.position.y, bigWboundsMin.y, bigWboundsMax.y), Mathf.Clamp(transform.position.z, bigWboundsMin.z, bigWboundsMax.z));
        }
        //4.3
        if (GetComponent<Camera>().orthographicSize > 7.29 && GetComponent<Camera>().orthographicSize < 7.39)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigEboundsMin.x, bigEboundsMax.x),
            Mathf.Clamp(transform.position.y, bigEboundsMin.y, bigEboundsMax.y), Mathf.Clamp(transform.position.z, bigEboundsMin.z, bigEboundsMax.z));
        }
        //4.4
        if (GetComponent<Camera>().orthographicSize > 7.39 && GetComponent<Camera>().orthographicSize < 7.49)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigRboundsMin.x, bigRboundsMax.x),
            Mathf.Clamp(transform.position.y, bigRboundsMin.y, bigRboundsMax.y), Mathf.Clamp(transform.position.z, bigRboundsMin.z, bigRboundsMax.z));
        }
        //4.5
        if (GetComponent<Camera>().orthographicSize > 7.49 && GetComponent<Camera>().orthographicSize < 7.59)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigTboundsMin.x, bigTboundsMax.x),
            Mathf.Clamp(transform.position.y, bigTboundsMin.y, bigTboundsMax.y), Mathf.Clamp(transform.position.z, bigTboundsMin.z, bigTboundsMax.z));
        }
        //4.6
        if (GetComponent<Camera>().orthographicSize > 7.59 && GetComponent<Camera>().orthographicSize < 7.69)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigYboundsMin.x, bigYboundsMax.x),
            Mathf.Clamp(transform.position.y, bigYboundsMin.y, bigYboundsMax.y), Mathf.Clamp(transform.position.z, bigYboundsMin.z, bigYboundsMax.z));
        }
        //4.7
        if (GetComponent<Camera>().orthographicSize > 7.69 && GetComponent<Camera>().orthographicSize < 7.79)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigUboundsMin.x, bigUboundsMax.x),
            Mathf.Clamp(transform.position.y, bigUboundsMin.y, bigUboundsMax.y), Mathf.Clamp(transform.position.z, bigUboundsMin.z, bigUboundsMax.z));
        }
        //4.8
        if (GetComponent<Camera>().orthographicSize > 7.79 && GetComponent<Camera>().orthographicSize < 7.89)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigIboundsMin.x, bigIboundsMax.x),
            Mathf.Clamp(transform.position.y, bigIboundsMin.y, bigIboundsMax.y), Mathf.Clamp(transform.position.z, bigIboundsMin.z, bigIboundsMax.z));
        }
        //4.9
        if (GetComponent<Camera>().orthographicSize > 7.89 && GetComponent<Camera>().orthographicSize < 7.99)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigOboundsMin.x, bigOboundsMax.x),
            Mathf.Clamp(transform.position.y, bigOboundsMin.y, bigOboundsMax.y), Mathf.Clamp(transform.position.z, bigOboundsMin.z, bigOboundsMax.z));
        }
        //5
        if (GetComponent<Camera>().orthographicSize > 7.99 && GetComponent<Camera>().orthographicSize < 8.01)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bigboundsMin.x, bigboundsMax.x),
            Mathf.Clamp(transform.position.y, bigboundsMin.y, bigboundsMax.y), Mathf.Clamp(transform.position.z, bigboundsMin.z, bigboundsMax.z));
        }
    }
}


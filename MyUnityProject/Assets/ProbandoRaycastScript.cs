using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbandoRaycastScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(gameObject.transform.position, new Vector3(0, -1, 0), out hit))
        {
            Debug.Log(hit.point);
        }
    }
}

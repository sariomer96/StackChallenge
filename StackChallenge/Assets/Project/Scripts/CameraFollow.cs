using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    // Start is called before the first frame update

    private void Start()
    {
        offset = GameManager.instance.character.transform.position - transform.position;
    }

    public IEnumerator FollowRoutine()
    {

       
        while (true)
        {
            
            print("folloooww");
            transform.position=Vector3.MoveTowards(transform.position,GameManager.instance.character.transform.position - offset,GameManager.instance.camFollowSpeed);
            
            yield return new WaitForFixedUpdate();
        }
      
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

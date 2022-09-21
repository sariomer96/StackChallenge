using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    // Start is called before the first frame update

    private void Start()
    {
        _offset = GameManager.instance.character.transform.position - transform.position;
    }

    public IEnumerator FollowRoutine()
    { 
        while (true)
        { 
            transform.position=Vector3.MoveTowards(transform.position,GameManager.instance.character.transform.position - _offset,GameManager.instance.camFollowSpeed);
            
            yield return new WaitForFixedUpdate();
        }
       
    }
 
}

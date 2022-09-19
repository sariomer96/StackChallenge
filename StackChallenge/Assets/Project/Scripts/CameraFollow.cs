using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    // Start is called before the first frame update
   
    public IEnumerator FollowRoutine()
    {

        offset = GameManager.instance.character.transform.position - transform.position;
        while (true)
        {
            
            transform.position=Vector3.MoveTowards(transform.position,GameManager.instance.character.transform.position - offset,GameManager.instance.camFollowSpeed);
            
            yield return new WaitForFixedUpdate();
        }
      
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

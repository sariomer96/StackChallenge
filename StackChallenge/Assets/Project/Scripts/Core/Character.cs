using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 lastPos; // for  movement check
    IEnumerator MoveRoutine()
    {
        while (true)
        {
         transform.position = Vector3.MoveTowards(transform.position,
             new Vector3(GameManager.instance.targetStack.x, transform.position.y, GameManager.instance.targetStack.z),
             GameManager.instance.characterMoveSpeed);
         if (transform.position !=lastPos)
         {
             GameManager.instance.isMoveCharacter = true;
         }else
             GameManager.instance.isMoveCharacter = false;

         lastPos = transform.position;
         yield return new WaitForFixedUpdate();
        

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==LayerMask.NameToLayer("collectable"))
        {
              ParticleSystem particleSystem=  other.transform.GetComponentInChildren<ParticleSystem>();
              particleSystem.Play();
              particleSystem.transform.SetParent(null);
              Destroy(other.gameObject);
        }
    }
}

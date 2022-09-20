using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            transform.position=Vector3.MoveTowards(transform.position,new Vector3(GameManager.instance.targetStack.x,transform.position.y,GameManager.instance.targetStack.z),GameManager.instance.characterMoveSpeed);
            
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

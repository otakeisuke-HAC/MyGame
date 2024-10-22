using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordColliderController : MonoBehaviour
{
    
    private GameObject SwrodObject;
    private BoxCollider SwordBollider;

    private void Start()
    {
        //SwordObject‚ÌBoxCollider‚ðŽæ“¾
        SwrodObject = GameObject.FindGameObjectWithTag("Sword");
        SwordBollider = SwrodObject.GetComponent<BoxCollider>();
    }
    public void SwordColliderOn()
    {
        SwordBollider.enabled = true;
    }

    public void SwordColliderOff()
    {
        SwordBollider.enabled = false;
    }
}

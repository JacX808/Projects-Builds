using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimControl : MonoBehaviour
{
    [Header("---- System ----")]
    public GameObject playerGun;
    public GameObject playerAvatar;

   [SerializeField]
    protected float mouseDist = 2f; // default 2m

    [SerializeField]
    protected GameObject mouseTarget;

    protected bool aimOn = true;

    [Header("---- Visuals ----")]
    public Image targetImage;
    public Sprite corsshairSPR;
   

    // ################################# GETS & SETS

    /// <summary>
    /// Get set aim status
    /// </summary>
    public bool AimOn
    {
        get { return aimOn; }

        set { aimOn = value; }
    }

    // ################################## INI
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        
    }

    // ################################## SYSTEM
    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        UpdateTargetPos();
    }

    /// <summary>
    /// Move Gun and crosshair
    /// </summary>
    private void UpdateTargetPos()
    {
        if(aimOn)
        {
            // center pos
            Vector3 forward = mouseTarget.transform.position - this.transform.position;
            playerGun.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

           // Debug.Log("Forward is: " + forward);
            if(forward.x > 0)
            {
                playerAvatar.transform.localEulerAngles = new Vector3(0f, 30f, 0f);
            }
            else
            {
                playerAvatar.transform.localEulerAngles = new Vector3(0f, 130f, 0f);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] private LineRenderer rubber;
    [SerializeField] private Transform first;
    [SerializeField] private Transform second;

    // fields set in the Unity Inspector pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    [SerializeField] private AudioSource redBird;

    void Start()
    {
        rubber.SetPosition(0, first.position);
        rubber.SetPosition(2, second.position);
        //bandSnap = GetComponent<AudioSource>();
    }
    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter()
    {
        //print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        // The player has pressed the mb while over Slingshot
        aimingMode = true;
        // Instantiate a projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        rubber.transform.position = launchPos;
        // set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;
        
    }

    void Update()
    {
        if (!aimingMode) return;

        //get current mouse pos in 2d coords
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the slingshot spherecollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // move projectile to new position
        Vector3 projPos = launchPos + mouseDelta;
        Vector3 rubberPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;
        rubber.transform.position = rubberPos;
        rubber.SetPosition(1, rubber.transform.position); //
        if (Input.GetMouseButtonUp(0))
        {
            //the mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
            redBird.Play();
        }
    }

    // Vector3 GetMousePos()
    // {
    //     Vector3 mousePos = Input.mousePosition;
    //     mousePos.z -= Camera.main.transform.position.z;
    //     Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePos);
    //     return mousePosInWorld - transform.position;
    // }
}

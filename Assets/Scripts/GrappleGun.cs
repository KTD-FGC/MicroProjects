using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{
    [Header("References")]
    private FpController playCon;
    public Transform cam;
    public Transform gunTip;
    public LayerMask WhatIsGrappleable;
    public LayerMask WhatIsPullable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCd;
    private float grappleCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;
    GameObject hitObject;

    [SerializeField]
    FpController controller;

    private void Start()
    {
        playCon = GetComponent<FpController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (grappleCdTimer > 0 && controller.isGrounded)
        {
            grappleCdTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grappleCdTimer > 0)
        {
            return;
        }

        controller.freeze = true;

        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, WhatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, WhatIsPullable))
        {
            grapplePoint = hit.point;
            hitObject = hit.transform.gameObject;
            Invoke(nameof(ExecutePull), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        controller.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }

        controller.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void ExecutePull()
    {
        controller.freeze = false;
        PullableObject obj = hitObject.GetComponent<PullableObject>();
        StartCoroutine(obj.DoPull());

        Invoke(nameof(StopGrapple), 1f);
    }

    private void StopGrapple()
    {
        grappling = false;

        controller.activeGrapple = false;

        controller.freeze = false;

        grappleCdTimer = grappleCd;

        lr.enabled = false;
    }
}

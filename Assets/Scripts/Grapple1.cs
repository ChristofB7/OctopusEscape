using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Grapple1 : MonoBehaviour
{

    //rotation variables
    private static float sinCounterClock = Mathf.Sin(Mathf.PI / 2);
    private static float cosCounterClock = Mathf.Cos(Mathf.PI / 2);
    private static float sinClock = Mathf.Sin(3 * Mathf.PI / 2);
    private static float cosClock = Mathf.Cos(3 * Mathf.PI / 2);


    //perhaps mass * gravity
    private float swingForce = .5f;

    //swing in the right direction
    private bool swingForceRight = true;


    private int grappleDistance = 5;
    private Vector2 mousePosition; //used to store the mousePosition at all times (in onUpdate)
    private Vector3 lineRendererPosition;  //This is a copy of where the user clicked at the time to draw the line


    private bool currentlyGrappling;  //true if hooked onto something 
    private bool hasGrappled;
    private DistanceJoint2D distanceJoint; // distance joint to create the grappling effect
    private LineRenderer lineRenderer;  // renderer to show the grappling hook
    private Rigidbody2D grappler;
    private Transform t;

    [SerializeField] LayerMask grappleMask = default;

    private Vector2 lastVelocity;
    private Vector2 currentVelocity;
    private float currentSpeed;



    /* test stuff */



    /* end test stuff */

    // Start is called before the first frame update
    void Start()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;
        currentlyGrappling = false;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        grappler = gameObject.GetComponent<Rigidbody2D>();
        t = gameObject.GetComponent<Transform>();

    }

  

    void Update()
    {
        if (currentlyGrappling)
        {

            DrawLine();

            //Debug.DrawRay(transform.position, new Vector3(mousePosition.x, mousePosition.y, 0) * 1000, Color.red);

            /* test stuff */



            /* end test stuff */
        }
    }

    void FixedUpdate()
    {
        
    
        
        
       
        if (currentlyGrappling)
        {

            currentVelocity = grappler.velocity;
            currentSpeed = currentVelocity.magnitude;

            //get angle b/t rb and joint point
            //apply swinging physics
            Vector2 swingForceDir = new Vector2(0, 1);
            Vector2 oppositeDir = new Vector2(0, 1);
            Vector2 playerDir;
            Vector2 additionalForce = new Vector2(0, 0);
            Vector2 joint = distanceJoint.connectedAnchor;
            Vector2 player = new Vector2(t.position.x, t.position.y);
            Vector2 line = player - joint;

            float radianAngle = Vector2.Angle(line, new Vector2(0, 1)) * Mathf.Deg2Rad;
            float adjustedSwingForce = swingForce * Mathf.Sin(radianAngle);


            //get clockwise and counter clockwise rotations of the vector by 90deg
            Vector2 clockVec = new Vector2(line.x * cosClock - line.y * sinClock, line.x * sinClock + line.y * cosClock);
            Vector2 counterClockVec = new Vector2(line.x * cosCounterClock - line.y * sinCounterClock, line.x * sinCounterClock + line.y * cosCounterClock);

            //see which one points downwards, that is the the force we wish to apply to the grapple player to swing
            if (clockVec.y < 0)
            {
                swingForceDir = clockVec.normalized;
                oppositeDir = counterClockVec.normalized;
                additionalForce = swingForceDir * swingForce;
            }
            else if (counterClockVec.y < 0)
            {
                swingForceDir = counterClockVec.normalized;
                oppositeDir = clockVec.normalized;
                additionalForce = swingForceDir * swingForce;
            }
            else
            {
                //need to put something here pls
                //playerDir
            }


            float angleVelocityVsForce = Vector2.Angle(currentVelocity, swingForceDir);

            if (angleVelocityVsForce < 90)
            {
                //seems to slow down every single frame, but speeds up here???????????????????????????????????????????????????????????????????????????????
                //Debug.Log("Speeding up " + currentSpeed);
                grappler.velocity = additionalForce + swingForceDir * currentSpeed;
                grappler.velocity = Round(grappler.velocity);
                //Debug.Log("Speeding up " + grappler.velocity.magnitude);
                //Debug.Assert(grappler.velocity.magnitude < currentSpeed);
            }
            else
            {
                //Debug.Log("Slowing Down");
                grappler.velocity = oppositeDir * currentSpeed + additionalForce;
                grappler.velocity = Round(grappler.velocity);
            }



















            Debug.DrawRay(player, line, Color.red, 10f);


            

            //force needs to be relative to
            //grappler.velocity = swingForceDir * currentSpeed;

            //grappler.AddForce(swingForceDir * adjustedSwingForce);
            //grappler.AddForce(tensionDir * swingForce);
            //grappler.AddForce(new Vector2(0, -1) * swingForce);
            //Debug.Log("ADDED FORCE" + swingForceDir.x + " " + swingForceDir.y);
            Debug.DrawRay(player, swingForceDir, Color.green, 5f);
            



        }
    }

    private Vector2 Round(Vector2 vec)
    {
        vec.x = Mathf.Round(vec.x * 10) / 10;
        vec.y = Mathf.Round(vec.y * 10) / 10;
        return vec;
    }

    // Update is called once per frame
    public void ShootGrapple(Vector2 mousePos)
    {
        Debug.Log(mousePos);
        Vector3 mouse = new Vector3(mousePos.x, mousePos.y, 0);
        Vector3 body = gameObject.GetComponent<Rigidbody2D>().position;
        Vector3 direction = mouse - body;

        Vector2 body2d = new Vector2(body.x, body.y);
        Debug.Log(body2d);
        Vector2 direction2d = mousePos - body2d;
        RaycastHit2D hit2D = Physics2D.Raycast(body2d, direction2d, grappleDistance, grappleMask);
        //Debug.DrawRay(body2d, direction2d, Color.red, 20f);
        Debug.Log("DRAW RAY PLS");

        if (!currentlyGrappling && hit2D)
        {
            //Debug.DrawRay(transform.position, new Vector3(mousePos.x, mousePos.y, 0), Color.red, 100f);
            mousePosition = mousePos;
            distanceJoint.enabled = true;
            distanceJoint.connectedAnchor = hit2D.point;
            lineRenderer.positionCount = 2;
            lineRendererPosition = hit2D.point;
            currentlyGrappling = true;
            grappler.gravityScale = 0;
            grappler.velocity = Vector2.zero;
            lastVelocity = grappler.velocity;
            Debug.Log(distanceJoint.distance);
           
            
        }
        else
        {
            distanceJoint.enabled = false;
            currentlyGrappling = false;
            lineRenderer.positionCount = 2;
            lineRendererPosition = mousePos;
            Debug.Log("missed");
        }
        DrawLine();
    }

    public void ReleaseGrapple()
    {
        lineRenderer.positionCount = 0;
        distanceJoint.enabled = false;
        currentlyGrappling = false;
        grappler.gravityScale = 3;
    }

    public void Jump()
    {
        //get current angle to apply jump force
    }

    public void Grounded()
    {
        hasGrappled = false;
    }

    public bool IsGrapple()
    {
        return currentlyGrappling;
    }

    private void DrawLine()
    {
        if (lineRenderer.positionCount <= 0) return;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, lineRendererPosition);
    }

  
}
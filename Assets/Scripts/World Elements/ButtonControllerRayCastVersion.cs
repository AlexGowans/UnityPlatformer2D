using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]

public class ButtonControllerRayCastVersion : RaycastController
{

    public GameObject myObject;
    public LayerMask pressMask;
    public bool permanantSwitch = false;
    public bool reverseSwitch = false;

    bool pressed = false;

    Animator animator;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

    }
    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();
        CheckPressed();
        animator = GetComponent<Animator>();


    }

    void SendSignal(bool p)
    {
        if (myObject.tag == "Platform") { myObject.GetComponent<PlatformController>().SwitchReceiver(p); }
        else { myObject.GetComponent<PlatformController>().SwitchReceiver(p); } //Default
    }

    void CheckPressed()
    {

        float rayLength = 1 + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, pressMask);

            bool b = true;
            if (reverseSwitch == true) { b = !b; }
            if (hit)
            {
                if (pressed == false)
                {
                    SendSignal(b); pressed = true;
                    animator.SetBool("OnOff", true);
                }
                break;
            }
            else
            {
                if (!permanantSwitch)
                {
                    if (pressed == true)
                    {
                        SendSignal(!b); pressed = false;
                        animator.SetBool("OnOff", false);
                    }
                }
            }
        }
    }

}

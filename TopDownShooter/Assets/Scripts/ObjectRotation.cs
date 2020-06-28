using UnityEngine;
using System.Collections;

public class ObjectRotation : MonoBehaviour
{
    public Transform pivot;
    public float rotationSpeed = 0.2f;
    public bool rotateX = true;
    public bool rotateY = false;
    public bool rotateZ = false;
    public bool stopRotating = false;
    public bool floating = false;
    public float floatOffset = 0.5f;
    public float floatSpeed = 0.3f;
    public float floatClampFactor = 2;
    public bool allowRotation = true;

    private Vector3 upPos, downPos, targetPosition;
    private bool floatUp = true;
    private bool set = false;


    void Start()
    {

        upPos = new Vector3(transform.localPosition.x,
                        transform.localPosition.y + floatOffset,
                        transform.localPosition.z);
        downPos = new Vector3(transform.localPosition.x,
                    transform.localPosition.y - floatOffset,
                    transform.localPosition.z);

        targetPosition = upPos;
    }

    void SelfRotation() {
        if (allowRotation)
        {
            if (!stopRotating)
            {
                Vector3 targetRot = Vector3.zero;
                //rotate
                if (rotateX) targetRot.y = 1; //z,x,y
                if (rotateY) targetRot.z = 1;
                if (rotateZ) targetRot.x = 1;

                //rotation technique
                gameObject.transform.Rotate(targetRot * rotationSpeed * Time.fixedDeltaTime * 100);
            }
        }

        if (floating)
        {

            float tollerance = floatOffset / floatClampFactor;
            if (gameObject.transform.localPosition.y >= (upPos.y - tollerance) && !set)
            {
                floatUp = false; set = true;
            }
            if (gameObject.transform.localPosition.y <= (downPos.y + tollerance) && !set)
            {
                floatUp = true; set = true;
            }


            if (set)
            {
                if (floatUp) targetPosition = upPos = new Vector3(gameObject.transform.localPosition.x,
                    gameObject.transform.localPosition.y + floatOffset,
                    gameObject.transform.localPosition.z);
                else targetPosition = downPos = new Vector3(gameObject.transform.localPosition.x,
                    gameObject.transform.localPosition.y - floatOffset,
                    gameObject.transform.localPosition.z);
                set = false;
            }

            //translation technique
            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, targetPosition, Time.fixedDeltaTime * floatSpeed);
        }
    }

    void AroundRotation(Transform target)
    {
        if (allowRotation)
        {
            if (!stopRotating)
            {
                Vector3 targetRot = Vector3.zero;
                //rotate
                if (rotateX) targetRot = Vector3.right;
                if (rotateY) targetRot = Vector3.up;
                if (rotateZ) targetRot = Vector3.forward;

                //rotation technique
                gameObject.transform.RotateAround(target.transform.position,targetRot, rotationSpeed * Time.fixedDeltaTime * 100);
            }
        }
    }

    void Update()
    {

        if (pivot != null)
            AroundRotation(pivot);
        else
            SelfRotation();     
    }





}

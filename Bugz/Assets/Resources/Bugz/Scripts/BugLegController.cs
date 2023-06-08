using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugLegController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isMoving = false;
    public float speed = 3.0F;
    public float rotateSpeed = 3.0F;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Input.GetAxis("Horizontal"));

        transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed, 0);

        if (Input.GetAxis("Vertical") > 0)
        {
            if (!isMoving)
            {
                isMoving = true;
                var anim = GetComponent<Animator>();
                anim.Play("bug-legs-begin");
            }

            //gameObject.transform.position = new Vector3(gameObject.transform.position.x - Input.GetAxis("Vertical") * 0.0049f, gameObject.transform.position.y, gameObject.transform.position.z);
            var controller = GetComponent<CharacterController>();

            // Move forward / backward
            var forward = transform.TransformDirection(Vector3.forward);
            float curSpeed = speed * Input.GetAxis("Vertical");
            controller.SimpleMove(forward * curSpeed);
        }
    }

    void OnRunEvent(AnimationEvent e)
    {
        if (Input.GetAxis("Vertical") <= 0)
        {
            if (isMoving)
            {
                var anim = GetComponent<Animator>();
                anim.Play(e.stringParameter, 0, 0);
            }
            isMoving = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTVRemoteInput : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    public GameObject volumetricModel;
    public GameObject horizontalReference;
    private bool connected = false;
    public Transform target;
    public Transform initTarget;
    public float speed = 1.0f;

    // Start is called before the first frame update

    void Awake()
    {
        //StartCoroutine(CheckForControllers());
        UnityEngine.tvOS.Remote.allowExitToHome = true;
        UnityEngine.tvOS.Remote.touchesEnabled = true;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float horizontalRotation = Input.GetAxis("Horizontal") * 100;
        horizontalRotation *= Time.deltaTime;

        float verticalRotation = Input.GetAxis("Vertical") * 50;
        verticalRotation *= Time.deltaTime;

        //transform.RotateAround(volumetricModel.transform.position, Vector3.up, rotation);

        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.RotateAround(volumetricModel.transform.position, Vector3.up, horizontalRotation);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            transform.RotateAround(horizontalReference.transform.position, Vector3.right, verticalRotation);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Camera.main.transform.position = new Vector3(0, 0.75f, -1.15f);
            Camera.main.transform.localEulerAngles = new Vector3(11, 0, 0);
        }

        if (Input.GetButton("Fire1"))
        {
            transform.position = Vector3.MoveTowards(transform.position, initTarget.position, 1.0f * Time.deltaTime);

            Vector3 targetDirection = target.position - transform.position;

            // The step size is equal to speed times frame time.
            float singleStep = speed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("A");
            transform.RotateAround(volumetricModel.transform.position, Vector3.up, 50.0f * Time.deltaTime);
        }
    }

    IEnumerator CheckForControllers()
    {
        while (true)
        {
            var controllers = Input.GetJoystickNames();
            if (!connected && controllers.Length > 0)
            {
                connected = true;
                Debug.Log("Connected");
            }
            else if (connected && controllers.Length == 0)
            {
                connected = false;
                Debug.Log("Disconnected");
            }
            yield return new WaitForSeconds(1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float LookLeftRight = 100.0f;
    public float LookUpDown = 75.0f;
    public GameObject shotPrefab = null;
    public int NumShots = 3;
    private Camera Cam = null;
    private GameObject[] Shots = null;

    private void Start()
    {
        //reference camera
        Cam = this.transform.GetComponentInChildren<Camera>();
        Cam.transform.position = this.transform.position;

        PoolUpShots();

        //lock cursor to gamescreen, hide
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //look around
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");

        Vector2 look = new Vector2(mX, mY);
        look = Vector2.Scale(look, new Vector2(LookLeftRight, LookUpDown));
        look.y = Mathf.Clamp(look.y, -65.0f, 65.0f);

        this.transform.localRotation *= Quaternion.AngleAxis(look.x * Time.deltaTime, Vector3.up);
        Cam.transform.localRotation *= Quaternion.AngleAxis(-look.y * Time.deltaTime, Vector3.right);

        //fire
        if (Input.GetButtonDown("Fire1") == true)   //ctrl, leftmouse
        {
            doFire();
        }

        //unlock cursor from gamescreen, show
        if (Input.GetButtonDown("Cancel") == true)  //esc key
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void PoolUpShots()
    {
        //add some shot prefabs to scene (but hide them)
        Shots = new GameObject[NumShots];
        for (int i = 0; i < NumShots; i++)
        {
            Shots[i] = Instantiate(shotPrefab, Vector3.zero, Quaternion.identity);
            Shots[i].name = "shot" + i.ToString("00");
            Shot script = Shots[i].GetComponent<Shot>();
            Physics.IgnoreCollision(Shots[i].GetComponent<Collider>(), this.GetComponent<Collider>());
            script.HideShot();
        }
    }
    private void doFire()
    {
        //look for a shot in the pool
        for (int i = 0; i < NumShots; i++)
        {
            Shot script = Shots[i].GetComponent<Shot>();
            //dead means it's not being used, we can fire it (again)
            if (script.isHidden == true)
            {
                //when we fire it, it gets set to Alive (and displays on gamescreen)
                script.Fire(Cam, 1.0f, 10.0f, 3.0f);
                break;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (Cam != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Cam.transform.position, Cam.transform.position + Cam.transform.forward * 2.0f);
            Gizmos.DrawSphere(Cam.transform.position + (Cam.transform.forward * 2.0f), 0.1f);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankCtrl : MonoBehaviour
{
    private Transform tr;
    private PhotonView pv;
    private Rigidbody rb;

    public float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        if (pv.IsMine)
        {
            rb.centerOfMass = new Vector3(0, -5.0f, 0);
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (pv.IsMine == true)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            tr.Translate(Vector3.forward * Time.deltaTime * speed * v);
            tr.Rotate(Vector3.up * Time.deltaTime * 100.0f * h);
        }
    }
}

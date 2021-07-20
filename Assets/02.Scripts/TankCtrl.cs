using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Photon.Pun;

public class TankCtrl : MonoBehaviour
{
    private Transform tr;
    private PhotonView pv;
    private Rigidbody rb;

    private CinemachineVirtualCamera vcam;
    private new Camera camera;
    private Ray ray;
    private RaycastHit hit;

    public float speed = 10.0f;
    public TMP_Text nickNameText;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        vcam = GameObject.Find("VCam").GetComponent<CinemachineVirtualCamera>();
        camera = Camera.main;

        rb.mass = 1000.0f;

        if (pv.IsMine)
        {
            vcam.Follow = tr;
            vcam.LookAt = tr;

            rb.centerOfMass = new Vector3(0, -5.0f, 0);
        }
        else
        {
            rb.isKinematic = true;
        }

        nickNameText.text = pv.Owner.NickName;
    }

    void Update()
    {
        if (pv.IsMine == true)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            tr.Translate(Vector3.forward * Time.deltaTime * speed * v);
            tr.Rotate(Vector3.up * Time.deltaTime * 100.0f * h);

            ray = camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 30.0f, Color.blue);
        }
    }
}

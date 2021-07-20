using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class TankCtrl : MonoBehaviour, IPunObservable
{
    private Transform tr;
    private PhotonView pv;
    private Rigidbody rb;

    private CinemachineVirtualCamera vcam;
    private new Camera camera;
    private Ray ray;
    private RaycastHit hit;
    public Transform turretTr;

    public float speed = 10.0f;
    public TMP_Text nickNameText;

    public Transform firePos;
    public GameObject cannonPrefab;
    public GameObject expEffect;

    private Vector3 currPos;
    private Quaternion currRot;

    private float hp = 100.0f;

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
            Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.blue);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
            {
                Vector3 hitPos = new Vector3(hit.point.x, turretTr.position.y, hit.point.z);

                var rot = Quaternion.LookRotation(hitPos - turretTr.position);

                turretTr.rotation = Quaternion.Slerp(turretTr.rotation, rot, Time.deltaTime * 5.0f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                // ?è¨?ÉÑ Î∞úÏÇ¨Î°úÏßÅ
                Fire(pv.Owner.ActorNumber);
                pv.RPC("Fire", RpcTarget.Others, pv.Owner.ActorNumber);
            }
        }
        else
        {
            if (Vector3.Distance(tr.position, currPos) >= 2.0f)
            {
                tr.position = currPos;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 20.0f);
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 20.0f);
        }
    }

    // RPC (Remote Procedure Call) ?õêÍ≤©ÏúºÎ°? Î∂ÑÎ¶¨?êú ?ã§Î•? PC, Î™®Î∞î?ùº?óê ?Éë?û¨?êú Í∞ôÏ?? ?ï±?ùò ?äπ?†ï ?ï®?àò(Procedure) ?ò∏Ï∂?
    [PunRPC]
    void Fire(int actorNo)
    {
        var cannon = Instantiate(cannonPrefab, firePos.position, firePos.rotation);
        cannon.GetComponent<Cannon>().actorNumber = actorNo;
        Destroy(cannon, 5.0f);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("CANNON"))
        {
            Destroy(coll.gameObject);
            var obj = Instantiate(expEffect, coll.transform.position, Quaternion.identity);
            Destroy(obj, 5.0f);

            hp -= 20.0f;
            if (hp <= 0.0f)
            {
                if (pv.IsMine)
                {
                    // ∆˜≈∫ ActorNumber √ﬂ√‚
                    int actNo = coll.collider.GetComponent<Cannon>().actorNumber;
                    // ActorNumber ∏¶ Player √ﬂ√‚
                    Player lastShooter = PhotonNetwork.CurrentRoom.GetPlayer(actNo);

                    string msg = $"\n<color=#00ff00>[{pv.Owner.NickName}]</color> is killed by <color=#ff0000>{lastShooter.NickName}</color>";

                    GameObject.Find("GameManager").GetComponent<GameManager>().photonView.RPC("SendChatMessage", RpcTarget.All, msg);
                }

                StartCoroutine(TankDie());
            }
        }
    }

    IEnumerator TankDie()
    {
        TankVisible(false);
        yield return new WaitForSeconds(3.0f);

        Vector3 pos = new Vector3(Random.Range(-150.0f, 150.0f), 0.0f, Random.Range(-150.0f, 150.0f));
        tr.position = pos;
        hp = 100.0f;
        TankVisible(true);
    }


    void TankVisible(bool visible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ?Ü°?ã†
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            // ?àò?ã†
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

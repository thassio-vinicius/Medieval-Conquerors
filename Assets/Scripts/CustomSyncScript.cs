using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CustomSyncScript : MonoBehaviour, IPunObservable
{
    public float teleportIfDistanceGreatherThan = .3f;
    public bool isTeleportEnabled = true, isDead;
    public Transform body;
    Rigidbody2D rb;
    PhotonView photonView;
    Vector2 networkPosition;
    Quaternion networkRotation, networkBodyRotation;
    float distance;
    Animator anim;
    bool calledOnce = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        anim = GetComponentInChildren<Animator>();

        networkPosition = new Vector2();
        networkRotation = new Quaternion();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {

            stream.SendNext(rb.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(body.transform.rotation);

            //syncing animations
            stream.SendNext(anim.GetBool("isWalking"));
            stream.SendNext(anim.GetBool("isFalling"));
            stream.SendNext(anim.GetBool("attack1"));
            stream.SendNext(anim.GetBool("attack2"));
            stream.SendNext(anim.GetBool("attack3"));
            stream.SendNext(anim.GetBool("isJumping"));
            stream.SendNext(anim.GetBool("isTakingHit"));
            stream.SendNext(anim.GetBool("isDead"));

            stream.SendNext(rb.velocity);


        }
        else
        {
            //called on my player gameobject that exists
            networkPosition = (Vector2)stream.ReceiveNext();

            networkRotation = (Quaternion)stream.ReceiveNext();
            networkBodyRotation = (Quaternion)stream.ReceiveNext();

            anim.SetBool("isWalking", (bool)stream.ReceiveNext());
            anim.SetBool("isFalling", (bool)stream.ReceiveNext());
            anim.SetBool("attack1", (bool)stream.ReceiveNext());
            anim.SetBool("attack2", (bool)stream.ReceiveNext());
            anim.SetBool("attack3", (bool)stream.ReceiveNext());
            anim.SetBool("isJumping", (bool)stream.ReceiveNext());
            anim.SetBool("isTakingHit", (bool)stream.ReceiveNext());

            isDead = (bool)stream.ReceiveNext();

            anim.SetBool("isDead", isDead);

            if (isDead)
            {

                GetComponent<CapsuleCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = true;
            }

            if (isTeleportEnabled)
            {
                if (Vector2.Distance(rb.position, networkPosition) > teleportIfDistanceGreatherThan)
                {
                    rb.position = networkPosition;
                }
            }

            body.rotation = networkBodyRotation;


            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));


            rb.velocity = (Vector2)stream.ReceiveNext();


            networkPosition += (Vector2)rb.velocity * lag;

            distance = Vector2.Distance(rb.position, networkPosition);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector2.MoveTowards(rb.position, networkPosition, distance * (1f / PhotonNetwork.SerializationRate));
        }



    }

}

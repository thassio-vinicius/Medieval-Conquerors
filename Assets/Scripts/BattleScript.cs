using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;

public class BattleScript : MonoBehaviourPun
{
    private float currentHealth, maxHealth;
    public Image healthBar;
    public LayerMask layerMask;
    private Rigidbody2D rb;
    public GameObject deathPanel;
    [HideInInspector]
    public bool isDead;
    public Transform attackPoint;
    public float attackRadius;
    private Animator animator;
    void Start()
    {
        maxHealth = 50f;
        currentHealth = 50f;

        healthBar.fillAmount = currentHealth / maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && photonView.IsMine && !isDead)
        {
            Attack();
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, layerMask);

        foreach (Collider2D enemy in hitEnemies)
        {


            float defaultDamageAmount = gameObject.GetComponent<PlayerController>().combo;

            enemy.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.All, defaultDamageAmount);

        }
    }

    [PunRPC]
    public void UpdateDeathCount(string sender)
    {
        var photonViews = UnityEngine.Object.FindObjectsOfType<PhotonView>();
        foreach (var view in photonViews)
        {
            var player = view.Owner;
            //Objects in the scene don't have an owner, its means view.owner will be null
            if (player != null)
            {
                var playerPrefabObject = view.gameObject;

                MatchController controller = playerPrefabObject.GetComponent<MatchController>();

                //do works...
                controller.deathCounts += 1;

            }
        }
        isDead = true;
    }

    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        if (!isDead)
        {
            animator.SetBool("isTakingHit", true);
            currentHealth -= damageAmount;

            healthBar.fillAmount = currentHealth / maxHealth;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }



    void Die()
    {

        photonView.RPC("UpdateDeathCount", RpcTarget.OthersBuffered, parameters: gameObject.name);
        isDead = true;


        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<Rigidbody2D>().mass = 100;
            GetComponent<PlayerController>().enabled = false;
            deathPanel.GetComponentInChildren<Text>().text = "You Died!";
            deathPanel.SetActive(true);
            GameObject closeButton = GameObject.Find("Close Button");
            if (closeButton != null) closeButton.SetActive(false);

        }

    }

    IEnumerator DeactivateAfterSeconds()
    {
        yield return new WaitForSeconds(.5f);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "FallArea")
        {
            Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}

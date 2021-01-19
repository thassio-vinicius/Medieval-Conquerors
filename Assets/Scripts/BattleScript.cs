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
    private bool isDead = false;
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
        Debug.Log("attack function called");

        foreach (Collider2D enemy in hitEnemies)
        {

            Debug.Log("loop called");

            float defaultDamageAmount = gameObject.GetComponent<PlayerController>().combo;

            enemy.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.All, defaultDamageAmount);

        }
    }

    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        Debug.Log("Do damage function called");
        if (!isDead)
        {
                animator.SetBool("isTakingHit", true);
            currentHealth -= damageAmount;

            healthBar.fillAmount = currentHealth / maxHealth;

            Debug.Log("current health" + currentHealth);


            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);

        if (photonView.IsMine)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = true;
            GetComponent<Rigidbody2D>().mass = 100;
            isDead = true;
            GetComponent<PlayerController>().enabled = false;
            deathPanel.GetComponentInChildren<Text>().text = "You Died!";
            deathPanel.SetActive(true);
            GameObject.Find("Close Button").SetActive(false);

        }

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

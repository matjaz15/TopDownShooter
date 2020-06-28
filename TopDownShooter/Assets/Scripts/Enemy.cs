using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : Entity
{

    public enum State
    {
        Idle,
        Chasing,
        Attacking
    }

    State currentState;

    public static event System.Action OnDeathStatic;

    public ParticleSystem deathEffect;
    public Transform colorVariantObject;

    NavMeshAgent pathfinder;
    Transform target;
    Material skinMaterial;
    Entity targetEntity;

    Color originalColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    ParticleDecal particleDecal;

    bool hasTarget;


    private void Awake()
    {       
        pathfinder = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null){
            hasTarget = true;
            
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<Entity>();            

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            
        }
    }

    protected override void Start () {
        base.Start();

        if (hasTarget){
            currentState = State.Chasing;
            targetEntity.OnDeath += TargetEntity_OnDeath;

            StartCoroutine(UpdatePath());
        }        
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget) {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;

        skinMaterial = colorVariantObject.GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;

    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.instance.PlaySound("Impact",transform.position);

        if (damage >= health) {
            if (OnDeathStatic != null) OnDeathStatic();
            AudioManager.instance.PlaySound("Enemy death", transform.position);

            ParticleSystem deathpPrticles = Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)).GetComponent<ParticleSystem>();
            Destroy(deathpPrticles.gameObject, deathEffect.main.startLifetime.constant);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }


    private void TargetEntity_OnDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update () {

        if (hasTarget){
            if (Time.time > nextAttackTime){
                float sqrDstTotarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstTotarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)){
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    AudioManager.instance.PlaySound("Enemy attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
	}

    IEnumerator Attack() {
        currentState = State.Attacking;
        pathfinder.enabled = false;
        Vector3 originalPos = transform.position;
        Vector3 dirtoTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirtoTarget * (myCollisionRadius + targetCollisionRadius - attackDistanceThreshold/2);

        float attackSpeed = 3;
        float percent = 0;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1) {

            if (percent >= 0.5f && !hasAppliedDamage) {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);

            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);


            yield return null;
        }

        skinMaterial.color = originalColor;

        currentState = State.Chasing;
        pathfinder.enabled = true;
    }


    IEnumerator UpdatePath() {
        float refreshRate = 0.25f;

        while (target) {
            if (currentState == State.Chasing)
            {
                Vector3 dirtoTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirtoTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }                
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
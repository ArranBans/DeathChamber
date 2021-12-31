using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask whatIsGround;

    [Header("Networking")]
    public int id;
    public EnemySO eSO;

    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public float attackRate;
    bool alreadyAttacked;
    public int targetedPlayer;

    [Header("States")]
    public float sightRange;
    public float visionAngle;
    public float attackRange;
    public bool playerInSightRange;
    public bool playerInAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        ServerSend.EnemyPosition(id, transform.position, transform.rotation);
    }

    private void Update()
    {
        #region VisionCone
        float closestSqrDistance = (sightRange * sightRange) + 1;
        foreach(Client c in Server.clients.Values)
        {
            if(c.playerManager == null)
            {
                continue;
            }
            if (c.playerManager.player == null)
            {
                continue;
            }

            Player p = c.playerManager.player;
            Vector3 playerVector = p.transform.position;
            Vector3 direction = (playerVector - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, direction);

            if(angle <= visionAngle)
            {
                float sqrDistance = (playerVector - transform.position).sqrMagnitude;
                if (sqrDistance <= sightRange * sightRange)
                {
                    if(sqrDistance < closestSqrDistance)
                    {
                        playerInSightRange = true;
                        targetedPlayer = c.id;
                        closestSqrDistance = sqrDistance;
                        Debug.Log($"Player {c.id} has been targeted by Enemy: {this.id}");
                        break;
                        
                    }
                }
            }
            
        }

        if (closestSqrDistance >= (sightRange * sightRange) + 1)
        {
            targetedPlayer = -1;
            playerInSightRange = false;
        }

        if(targetedPlayer != -1)
        {
            float sqrDistance = (Server.clients[targetedPlayer].playerManager.player.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance <= attackRange * attackRange)
            {
                playerInAttackRange = true;
            }
            else
            {
                playerInAttackRange = false;
            }
        }
        #endregion

        if (!playerInSightRange && !playerInAttackRange) 
            Patrolling();

        if(targetedPlayer != -1)
        {
            if (playerInSightRange && !playerInAttackRange)
                ChasePlayer();
            if (playerInAttackRange)
                AttackPlayer();
        }
       
    }

    private void Patrolling()
    {
        if (!walkPointSet)
            SearchWalkPoint();
        else
            agent.SetDestination(walkPoint);

        float sqrDistance = (walkPoint - transform.position).sqrMagnitude;

        if (sqrDistance < 1f)
            walkPointSet = false;
        
    }
    private void ChasePlayer()
    {
        agent.SetDestination(Server.clients[targetedPlayer].playerManager.player.transform.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(new Vector3(Server.clients[targetedPlayer].playerManager.player.transform.position.x, transform.position.y, Server.clients[targetedPlayer].playerManager.player.transform.position.z));
        Debug.Log("Attacking...");
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        Ray ray = new Ray(walkPoint, -transform.up);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, 3f))
        {
            if (hitInfo.collider.gameObject.isStatic) 
                walkPointSet = true;
        }
    }
}

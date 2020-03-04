﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private static AIManager _instance;
    public static AIManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public List<NavAgent> agents = new List<NavAgent>();
    public List<FriendlyMeleeAIAgent> friendlies;
    public List<MeleeAIAgent> enemies;
    public PlayerMovementController playerAgent;

    LinkedList<MeleeAIAgent> enemyQueue;
    List<FriendlyMeleeAIAgent> readyForEnemy;
    
    // Start is called before the first frame update
    void Start()
    {
        playerAgent = transform.parent.gameObject.GetComponent<PlayerMovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        readyForEnemy = new List<FriendlyMeleeAIAgent>();

        foreach (FriendlyMeleeAIAgent friendly in friendlies)
        {
            friendly.ExecuteState();
        }

        enemyQueue = new LinkedList<MeleeAIAgent>();
        MeleeAIAgent[] enemiesCopy = new MeleeAIAgent[enemies.Count];
        enemies.CopyTo(enemiesCopy);
        foreach (MeleeAIAgent enemy in enemiesCopy)
        {
            enemy.ExecuteState();
        }

        LinkedListNode<MeleeAIAgent> enemyHead = enemyQueue.First;
        while (enemyHead != null && readyForEnemy.Count > 0)
        {
            readyForEnemy.Sort((f1, f2) =>
                -Vector3.Distance(f1.transform.position, enemyHead.Value.transform.position).CompareTo(Vector3.Distance(f2.transform.position, enemyHead.Value.transform.position)));
            readyForEnemy[readyForEnemy.Count - 1].AttackEnemy(enemyHead.Value);
            enemyHead.Value.AddPursuer(readyForEnemy[readyForEnemy.Count - 1]);
            readyForEnemy.RemoveAt(readyForEnemy.Count - 1);
            enemyQueue.RemoveFirst();
            enemyHead = enemyHead.Next;
        }

    }


    public void PutOnDispatchQueue(FriendlyMeleeAIAgent friendlyAI)
    {
        readyForEnemy.Add(friendlyAI);
    }

    public void PutOnEnemyQueue(MeleeAIAgent enemyAI)
    {
        enemyQueue.AddLast(enemyAI);
    }

}

﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour {

    Transform BasePool;
    List<Base> myBases;
    int numberMyBases;
    List<Base> otherBases;
    int numberOtherBases;
    List<Base> selectedBases;
    private int partysupplies;
    public float partySuppliesMeter;
    private PlayerSelection player;
    public float timeBetweenAttacks;
    public float timeBetweenBombs;

    private DeployBomb bomber;

    Base target;
    [HideInInspector]
    public Team myTeam;

    SendTroops TroopSender;

    public AlgorithmType algToUse;

    // Use this for initialization
    void Start()
    {
        bomber = GameObject.FindGameObjectWithTag("BombSender").GetComponent<DeployBomb>();
        TroopSender = GameObject.FindGameObjectWithTag("TroopSender").GetComponent<SendTroops>();
        //myTeam = Team.Spooky;
        BasePool = GameObject.FindGameObjectWithTag("BasePool").transform;
        selectedBases = new List<Base>();
        myBases = new List<Base>();
        otherBases = new List<Base>();
        Base b;

        //tell ai whose is its own and whose are enemies
        foreach (Transform place in BasePool)
        {
            b = place.GetComponent<Base>();
            if (b.myTeam().Equals(myTeam))
            {
                myBases.Add(b);
                numberMyBases++;
            }
            else
            {
                otherBases.Add(b);
                numberOtherBases++;
            }
        }

        //Let the carnage begin
        switch (algToUse)
        {
            case AlgorithmType.Random:
                StartCoroutine(RandomMoveAlgorithm());
                break;
            case AlgorithmType.Greedy:
                StartCoroutine(GreedyMoveAlgorithm());
                break;
            case AlgorithmType.GreedyTurtle:
                StartCoroutine(GreedyTurtleMoveAlgorithm());
                break;
        }

    }

    private void categorizeBases()
    {
        Base b;
        myBases.Clear();
        otherBases.Clear();
        foreach (Transform place in BasePool)
        {
            b = place.GetComponent<Base>();
            if (b.myTeam().Equals(myTeam))
            {
                myBases.Add(b);
                numberMyBases++;
            }
            else
            {
                otherBases.Add(b);
                numberOtherBases++;
            }
        }
    }

	IEnumerator RandomMoveAlgorithm()
    {
        if (myBases.Count == 0 && otherBases.Count == 0)
        {
            Debug.Log("Where the bases at, chap :P");
        }
        else
        {
            while (myBases.Count > 0 && otherBases.Count > 0)
            {
                int randomNumberOfMyBases = UnityEngine.Random.Range(0, numberMyBases - 1);
                //random.range(a,b) is a to b inclusive, inclusive
                int randomOtherBaseIndex = UnityEngine.Random.Range(0, numberOtherBases - 1);
                int randomMyBaseIndex = 0;
                HashSet<Base> selectedBases = new HashSet<Base>();

                for (int i = 0; i <= randomNumberOfMyBases; i++)
                {
                    randomMyBaseIndex = UnityEngine.Random.Range(0, numberMyBases - 1);
                    selectedBases.Add(myBases[randomMyBaseIndex]);
                }
                target = otherBases[randomOtherBaseIndex];
                if (target != null && selectedBases.Count != 0)
                    //troop sender does a lot to tie things together
                    TroopSender.main(selectedBases, target, myTeam);
                yield return new WaitForSeconds(UnityEngine.Random.Range(timeBetweenAttacks, 7));
            }
            if (myBases.Count == 0)
                Debug.Log("Enemy: I lost ;(");
            else
                Debug.Log("Enemy: Yay I win B)");
        }
    }
    public void addSupplies(int supplies)
    {
        if (supplies + partysupplies > 1000)
        {
            partysupplies = 1000;
        }
        else
        {
            partysupplies = supplies + partysupplies;
        }
    }
    private IEnumerator dropBombRoutinely()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenBombs);
            if (partysupplies > 250)
            {
                partysupplies -= 250;
                Base strongest = null;
                int itsPop = 0;
                foreach(Base b in otherBases)
                {
                    if (b.myPopulation() > itsPop)
                    {
                        strongest = b;
                        itsPop = b.myPopulation();
                    }
                }
                if (strongest != null)
                {
                    bomber.sendBomb(strongest.gameObject.transform.position, myTeam);
                }
            }
        }
    }
    IEnumerator GreedyMoveAlgorithm()
    {
        StartCoroutine(dropBombRoutinely());
        partySuppliesMeter = partysupplies / 1000;
        if (myBases.Count == 0 && otherBases.Count == 0)
        {
            Debug.Log("Where the bases at, chap :P");
        }
        else
        {

            while (myBases.Count > 0 && otherBases.Count > 0)
            {
                categorizeBases();
                HashSet<Base> selectedBases = new HashSet<Base>();
                int sumOfTroops = 0;
                foreach (Base b in myBases)
                {
                    if (!b.gameObject.GetComponent<ArcherBase>() && !(b.gameObject.GetComponent<MineBase>()))
                    {
                        sumOfTroops += b.myPopulation() / 2;
                        selectedBases.Add(b);
                    }
                }
                if (selectedBases.Count == 0)
                {
                    foreach (Base b in myBases)
                    {
                        sumOfTroops += b.myPopulation() / 2;
                        selectedBases.Add(b);
                    }
                }

                int greatestResult = 0;
                int tempResult;
                Base weakestTarget = null;
                foreach (Base b in otherBases)
                {
                    if (b.gameObject.GetComponent<ArcherBase>() == null)
                    {
                        tempResult = sumOfTroops - b.myPopulation();
                        if (tempResult > greatestResult)
                        {
                            greatestResult = tempResult;
                            weakestTarget = b;
                        }
                    }
                }
                if (weakestTarget == null)
                {
                    foreach (Base b in otherBases)
                    {
                        tempResult = sumOfTroops - b.myPopulation();
                        if (tempResult > greatestResult)
                        {
                            greatestResult = tempResult;
                            weakestTarget = b;
                        }
                    }
                }

                
                if (weakestTarget != null)
                    TroopSender.main(selectedBases, weakestTarget, myTeam);
                yield return new WaitForSeconds(timeBetweenAttacks);
            }
            if (myBases.Count == 0)
                Debug.Log("Enemy: I lost ;(");
            else
                Debug.Log("Enemy: Yay I win B)");
        }
    }

    IEnumerator GreedyTurtleMoveAlgorithm()
    {
        if (myBases.Count == 0 && otherBases.Count == 0)
        {
            Debug.Log("Where the bases at, chap :P");
        }
        else
        {
            while (myBases.Count > 0 && otherBases.Count > 0)
            {
                categorizeBases();

                Vector3 averageMyBasePos = Vector3.zero;

                HashSet<Base> selectedBases = new HashSet<Base>();
                int sumOfTroops = 0;
                foreach (Base b in myBases)
                {
                    if (!b.gameObject.GetComponent<ArcherBase>() && !(b.gameObject.GetComponent<MineBase>()))
                    {
                        sumOfTroops += b.myPopulation() / 2;
                        selectedBases.Add(b);
                    }
                }
                if (selectedBases.Count == 0)
                {
                    foreach (Base b in myBases)
                    {
                        sumOfTroops += b.myPopulation() / 2;
                        selectedBases.Add(b);
                    }
                }

                foreach (Base b in myBases)
                {
                    averageMyBasePos += b.transform.position;
                }
                averageMyBasePos /= myBases.Count;

                Dictionary<float, Base> BaseDistDict = new Dictionary<float, Base>();
                float[] Dists = new float[otherBases.Count];

                int i = 0;
                foreach (Base b in otherBases)
                {
                    float distance = (b.transform.position - averageMyBasePos).sqrMagnitude;
                    BaseDistDict.Add(distance, b);
                    Dists[i] = distance;
                    i++;
                }
                Array.Sort(Dists);

                int greatestResult = 0;
                int tempResult;
                Base weakestTarget = null;
                for (i = 0; i < 5 && i < otherBases.Count; i++)
                {

                    if (BaseDistDict[Dists[i]].gameObject.GetComponent<ArcherBase>() == null)
                    {
                        tempResult = sumOfTroops - BaseDistDict[Dists[i]].myPopulation();
                        if (tempResult > greatestResult)
                        {
                            greatestResult = tempResult;
                            weakestTarget = BaseDistDict[Dists[i]];
                        }
                    }
                }
                if (weakestTarget == null)
                {
                    for (i = 0; i < 5 && i < otherBases.Count; i++)
                    {
                        tempResult = sumOfTroops - BaseDistDict[Dists[i]].myPopulation();
                        if (tempResult > greatestResult)
                        {
                            greatestResult = tempResult;
                            weakestTarget = BaseDistDict[Dists[i]];
                        }
                    }
                }
                if (weakestTarget != null)
                    TroopSender.main(selectedBases, weakestTarget, myTeam);
                yield return new WaitForSeconds(timeBetweenAttacks);
            }
            if (myBases.Count == 0)
            {
                Debug.Log("Enemy: I lost ;(");
                PlayerWin();
            }
            else
            {
                Debug.Log("Enemy: Yay I win B)");
                PlayerLose();
            }
        }
    }

    public void declareBaseMine(Base b)
    {
        otherBases.Remove(b);
        myBases.Add(b);
    }
    public void declareBaseNoLongerMine(Base b)
    {
        otherBases.Add(b);
        myBases.Remove(b);
    }

    private void PlayerWin()
    {
        SceneManager.LoadScene(3);
    }
    private void PlayerLose()
    {
        SceneManager.LoadScene(2);
    }
}

public enum AlgorithmType{
    Random, Greedy, GreedyTurtle
}
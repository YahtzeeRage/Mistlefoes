﻿             using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlightable : MonoBehaviour {

    Sprite normalBase;
    Sprite normalArcher;
    Sprite normalMine;
    Sprite normalGeneric;
    Sprite highlightedBase;
    Sprite highlightedArcher;
    Sprite highlightedMine;
    Sprite highlightedGeneric;
    Sprite merryBase;
    Sprite merryArcher;
    Sprite merryMine;
    Sprite merryGeneric;
    Sprite spookyBase;
    Sprite spookyArcher;
    Sprite spookyMine;
    Sprite spookyGeneric;
    SpriteRenderer mySprite;
    Material highlightedMat;
    Material normalMat;

    Material merryMat;
    Material spookyMat;

    bool hasMouseOverIt;
    bool externallyHighlighted;
    [HideInInspector]
    public Team currentTeam;



    public bool HasMouseOverIt()
    {
        return hasMouseOverIt;
    }
    public void initializeHighlightablePart()
    {
        merryMat = Resources.Load<Material>("red");
        spookyMat = Resources.Load<Material>("purple");
        normalMat = Resources.Load<Material>("normal");
        highlightedMat = Resources.Load<Material>("yellow");
        Sprite[] buildings = Resources.LoadAll<Sprite>("buildings");
        normalBase =buildings[3];
        normalArcher = buildings[5];
        normalMine = buildings[12];
        normalGeneric = Resources.Load<Sprite>("genericSquare");
        highlightedBase = Resources.Load<Sprite>("genericSquareHighlighted");
        highlightedArcher = Resources.Load<Sprite>("genericSquareHighlighted"); 
        highlightedMine = Resources.Load<Sprite>("genericSquareHighlighted");
        highlightedGeneric = Resources.Load<Sprite>("genericSquareHighlighted");
        merryBase = buildings[0];
        merryArcher = buildings[4];
        merryMine = buildings[12];
        merryGeneric = Resources.Load<Sprite>("genericSquareMerry");
        spookyBase = buildings[1];
        spookyArcher = buildings[2];
        spookyMine = buildings[12];
        spookyGeneric = Resources.Load<Sprite>("genericSquareSpooky");

        mySprite = gameObject.GetComponent<SpriteRenderer>();

        makeCorrectTeamSprite();
        externallyHighlighted = false;

    }
    public void highlight()
    {
        mySprite.material = highlightedMat;
        externallyHighlighted = true;
        //mySprite.sprite = highlightedBase;
    }

    public void unhighlight()
    {
        mySprite.material = normalMat;
        makeCorrectTeamSprite();
        externallyHighlighted = false;
    }

    public bool isHighlighted()
    {
        return externallyHighlighted;
    }

    public void makeCorrectTeamSprite()
    {
        //temporary until parT palette swap
        if (gameObject.GetComponent<MineBase>())
        {
            if (currentTeam.Equals(Team.Neutral))
            {
                mySprite.material = normalMat;
            }
            else if (currentTeam.Equals(Team.Merry))
            {
                mySprite.material = merryMat;
            }
            else
            {
                mySprite.material = spookyMat;
            }
        }
        if (currentTeam.Equals(Team.Neutral))
            if (gameObject.GetComponent<ArcherBase>())
            {
                mySprite.sprite = normalArcher;
            }
            else if (gameObject.GetComponent<MineBase>())
            {
                mySprite.sprite = normalMine;
            }
            else
            {
                mySprite.sprite = normalBase;
            }
        if (currentTeam.Equals(Team.Merry))
        {
            if (gameObject.GetComponent<ArcherBase>())
            {
                mySprite.sprite = merryArcher;
            }
            else if (gameObject.GetComponent<MineBase>())
            {
                mySprite.sprite = merryMine;
            }
            else
            {
                mySprite.sprite = merryBase;
            }
        }
        if (currentTeam.Equals(Team.Spooky))
            if (gameObject.GetComponent<ArcherBase>())
            {
                mySprite.sprite = spookyArcher;
            }
            else if (gameObject.GetComponent<MineBase>())
            {
                mySprite.sprite = spookyMine;
            }
            else
            {
                mySprite.sprite = spookyBase;
            }
    }
    private void OnMouseOver()
    {
        //Debug.Log('d');
        hasMouseOverIt = true;

        //however I want nothing to be messed with when PlayerSelection is trying to enforce group highlighting
        if (!externallyHighlighted)
            //mySprite.sprite = highlightedBase;
            mySprite.material = highlightedMat;
    }

    //normally unhighlight when unmoused over
    private void OnMouseExit()
    {
        hasMouseOverIt = false;

        //no disobeying PlayerSelection-san
        if (!externallyHighlighted)
        {
            mySprite.material = normalMat;
            makeCorrectTeamSprite();
        }
    }
}

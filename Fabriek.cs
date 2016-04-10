using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Managers;
using Assets.Scripts.Util;
using UnityEngine;

public abstract class Fabriek : MonoBehaviour, IBuilding
{
    public List<Tuple<int, Currencies>> UpgradeCost { get; set; }
    public Tuple<int, Currencies> Income { get; set; }
    public Tuple<int, Currencies> Expenses { get; set; }
    public List<Tuple<int, Currencies>> Buildcost { get; set; }
    public Tuple<float, float> XDifference { get; set; }
    public Vector3[] Squares { get; set; }

    public Hashtable GetIncome()
    {
        var table = BuildManager.IsNearRoad(Squares) ? new Hashtable {{BuildingTags.Income, Income}} : new Hashtable();
        return table;
    }

    public Hashtable GetExpenses()
    {
        var table = BuildManager.IsNearRoad(Squares)
            ? new Hashtable {{BuildingTags.Expenses, Expenses}}
            : new Hashtable();
        return table;
    }

    public List<Tuple<int, Currencies>> GetBuildCost()
    {
        return Buildcost;
    }

    public Tuple<float, float> GetXZDifferences()
    {
        return XDifference;
    }

    public void BuiltOnSquares(Vector3[] squares)
    {
        Squares = squares;
    }

    public Vector3[] GetSquares()
    {
        return Squares;
    }

    public virtual void Start()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().UpdateValues();
    }

    public Hashtable GetIncomeAndExpenses()
    {
        var table = new Hashtable {{BuildingTags.Income, Income}, {BuildingTags.Expenses, Expenses}};
        return table;
    }
}
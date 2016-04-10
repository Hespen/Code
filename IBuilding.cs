using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public interface IBuilding
{
    Hashtable GetIncome();
    Hashtable GetExpenses();
    List<Tuple<int, Currencies>> GetBuildCost();
    Tuple<float, float> GetXZDifferences();
    void BuiltOnSquares(Vector3[] squares);
    Vector3[] GetSquares();
}
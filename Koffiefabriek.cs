using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;

public class Koffiefabriek : Fabriek
{
    public Hashtable InStorage { get; set; }
    public Koffiefabriek()
    {
        Buildcost = new List<Tuple<int, Currencies>>
        {
            new Tuple<int, Currencies>(250, Currencies.Keuro),
            new Tuple<int, Currencies>(250, Currencies.Koffiebonen)
        };

        Expenses = new Tuple<int, Currencies>(50, Currencies.Koffiebonen);

        Income = new Tuple<int, Currencies>(10, Currencies.Koffie);

        UpgradeCost = new List<Tuple<int, Currencies>>
        {
            new Tuple<int, Currencies>(3000, Currencies.Keuro),
            new Tuple<int, Currencies>(6000, Currencies.Keuro)
        };
        // The x and z differences are the amount of units the building needs to be moved to fit exactly in the grid.
        XDifference = new Tuple<float, float>(-0.36f, -0.17f);
    }

    public override void Start()
    {
        base.Start();
    }

    public void AddToStorage(int amount)
    {
        var currentValue = (int)InStorage[Currencies.Koffie];
        currentValue += amount;
        InStorage[Currencies.Koffie] = currentValue;
    }
}
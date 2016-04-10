using System.Collections.Generic;
using Assets.Scripts;

public class Theefabriek : Fabriek
{
    public Theefabriek()
    {
        Buildcost = new List<Tuple<int, Currencies>>
        {
            new Tuple<int, Currencies>(450, Currencies.Keuro)
        };

        Expenses = new Tuple<int, Currencies>(50, Currencies.Theebladeren);

        Income = new Tuple<int, Currencies>(10, Currencies.Thee);

        UpgradeCost = new List<Tuple<int, Currencies>>
        {
            new Tuple<int, Currencies>(3000, Currencies.Keuro),
            new Tuple<int, Currencies>(6000, Currencies.Keuro)
        };

        // The x and z differences are the amount of units the building needs to be moved to fit exactly in the grid.
        XDifference = new Tuple<float, float>(-0.61f, -1.17f);
    }
}
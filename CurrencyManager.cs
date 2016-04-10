using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Util;
using JetBrains.Annotations;
using UnityEngine;

public class CurrencyManager : MonoBehaviour, IManager
{
    private static Hashtable _incomeRates, _usageRates, _finalIncomeRates, _referenceTable;
    public static Hashtable Wallet;
    public static CurrencyManager cm;
    private readonly String[] _labels = {"Plantage", "Fabriek", "Hoofdkantoor", "Distributiecentrum"};

    public CurrencyManager()
    {
        cm = this;
        _referenceTable = new Hashtable
        {
            {Currencies.Koffiebonen, 0},
            {Currencies.Cacaobonen, 0},
            {Currencies.Theebladeren, 0},
            {Currencies.Koffie, 0},
            {Currencies.Keuro, 0},
            {Currencies.Thee, 0},
            {Currencies.Chocomel, 0},
            {Currencies.Espresso, 0},
            {Currencies.Blauwdruk, 0},
            {Currencies.Cappuccino, 0}
        };


        if (SceneProperties.IncomeRates == null)
        {
            SceneProperties.IncomeRates = new Hashtable(_referenceTable);
        }

        if (SceneProperties.UsageRates == null)
        {
            SceneProperties.UsageRates = new Hashtable(_referenceTable);
        }

        _finalIncomeRates = new Hashtable(_referenceTable);

        Wallet = LoadWallet();
    }

    public void UpdateValues()
    {
    }

    // Use this for initialization
    private void Start()
    {
        //Call this Method every 5 seconds with an initial delay of 2.
        InvokeRepeating("CheckGameStatus", 5, 2);
    }

    private void DetermineIncomeAndExpenses()
    {
        foreach (var label in _labels)
        {
            var buildings = GameObject.FindGameObjectsWithTag(label);
            foreach (var building in buildings)
            {
                var b = building.GetComponent<IBuilding>();
                if (b == null) continue;
                UpdateRatesFor(b.GetIncome(), _incomeRates);
                UpdateRatesFor(b.GetExpenses(), _usageRates);
            }
        }
    }

    public void CheckGameStatus()
    {
        //If we are currently building something (Grid is shown) return, we don't want to update values if building has not been completed
        if (GridOverlay.ShowGrid) return;
        UpdateCurrencyValues();
    }

    private void UpdateCurrencyValues()
    {
        if (SceneProperties.UpdateRates)
        {
            //Reset tables
            _incomeRates = new Hashtable(_referenceTable);
            _usageRates = new Hashtable(_referenceTable);

            //Get the new rates
            DetermineIncomeAndExpenses();
        }
        _finalIncomeRates = new Hashtable(_referenceTable);

        foreach (DictionaryEntry dictionaryEntry in _referenceTable)
        {
            var income = (int) _incomeRates[dictionaryEntry.Key];
            var expenses = (int) _usageRates[dictionaryEntry.Key];

            //The current value of the currency in the wallet.
            var currentWallet = (int) Wallet[dictionaryEntry.Key];

            var balans = income + expenses;
            //If there are more resources gathered than spent per X time
            if (balans >= 0)
            {
                //If the rate is already specified, or if it doesn't have the resources
                if ((int) _finalIncomeRates[dictionaryEntry.Key] != 0) continue;
                _finalIncomeRates[dictionaryEntry.Key] = balans;
            }
            //If the expenses can be covered by the already gathered resources
            else if (currentWallet + balans >= 0)
            {
                if ((int) _finalIncomeRates[dictionaryEntry.Key] != 0) continue;
                _finalIncomeRates[dictionaryEntry.Key] = balans;
            }
            //If we don't have enough resources, we have to tune down the income
            else
            {
                CalculateIncomeRate(dictionaryEntry, income);
            }
        }

        UpdateWallet();
    }

    private void CalculateIncomeRate(DictionaryEntry dictionaryEntry, int income)
    {
        var tempIncome = income;

        //Loop through all labeled buildings
        for (var i = 1; i < _labels.Length; i++)
        {
            var buildings = GameObject.FindGameObjectsWithTag(_labels[i]);
            foreach (var b in buildings.Select(building => building.GetComponent<IBuilding>()))
            {
                //Get the amount of resources it costs to keep it running
                var expenseRate = (Tuple<int, Currencies>) b.GetExpenses()[BuildingTags.Expenses];
                //If the factory uses the current resource
                if (expenseRate == null) continue;

                if (expenseRate.Second == (Currencies) dictionaryEntry.Key)
                {
                    //Substract the expenses, until it is below 0
                    tempIncome -= expenseRate.First;
                    if (tempIncome < 0)
                    {
                        //Get the income for the building
                        var incomeRate = (Tuple<int, Currencies>) b.GetIncome()[BuildingTags.Income];

                        //Cross-multiply the normal income and expenses with the current expenses
                        var finalIncomeRatio = (expenseRate.First + tempIncome)*incomeRate.First/expenseRate.First;
                        //The used resource will now be 0
                        _finalIncomeRates[dictionaryEntry.Key] = 0;

                        //If there are no resources gathered for this factory, the finalIncomeRatio will be 0. 
                        //We will set the incomeRate to -999, so we will know that the income and expenses are already covered in UpdateCurrencyValue()
                        _finalIncomeRates[incomeRate.Second] = finalIncomeRatio == 0
                            ? -999
                            : incomeRate.First + finalIncomeRatio;
                        break;
                    }
                }
            }
        }
    }

    private void UpdateWallet()
    {
        foreach (DictionaryEntry currency in _finalIncomeRates)
        {
            var currencyAmount = (int) Wallet[currency.Key];
            var currencyValue = (int) currency.Value;
            currencyAmount += currencyValue == -999 ? 0 : currencyValue;
            Wallet[currency.Key] = currencyAmount;
        }
    }

    /// <summary>
    ///     Loop through the currency data and update the IncomeRates table
    /// </summary>
    /// <param name="currencyValues">Income and Expenses</param>
    /// <param name="incomeRates"></param>
    private Boolean UpdateRatesFor([NotNull] Hashtable currencyValues, Hashtable rates)
    {
        if (currencyValues == null) throw new ArgumentNullException("currencyValues");
        foreach (DictionaryEntry currencyValue in currencyValues)
        {
            if (currencyValue.Value == null) continue;

            //currencyValue<BuildingTags, Tuple<int, Currencies>>
            //eg. currencyValue<Income, <10, Keuro>>
            var key = (BuildingTags) currencyValue.Key;
            var income = (Tuple<int, Currencies>) currencyValue.Value;

            var incomeFor = (int) rates[income.Second];
            switch (key)
            {
                case BuildingTags.Income:
                    incomeFor += income.First;
                    break;
                case BuildingTags.Expenses:
                    incomeFor -= income.First;
                    break;
            }
            rates[income.Second] = incomeFor;
        }
        return true;
    }

    /// <summary>
    ///     Returns Wallet value
    /// </summary>

    public void ChangeContentWallet(Tuple<Currencies, int> bill)
    {
        Wallet[bill.First] = Int32.Parse(Wallet[bill.First].ToString()) + bill.Second;
        SaveWallet();
    }

    public static void SaveWallet(bool firstSave = false)
    {
        var dbUserData = new DBUserData();

        // If data has never been inserted, do this now
        if (firstSave)
        {
            dbUserData.SaveResources();
            return;
        }

        // Before we save, we want to check if there has been any tradeoffers the user placed which have been bought.
        // If there are, we want to add the income of that tradeoffer to the wallet before saving it, so that income won't get overwritten.
        var offers = dbUserData.CheckForCompletedTrades();
        var dbTrading = new DBTrading();

        foreach (var offer in offers)
        {
            // Add the resources that were requested to the wallet.
            var currency = (Currencies) Enum.Parse(typeof (Currencies), offer.RequestedCurrency);
            Wallet[currency] = Convert.ToInt32(Wallet[currency]) + Convert.ToInt32(offer.RequestedAmount);
            dbTrading.UpdateProcessed(offer);
        }

        dbUserData.UpdateResources(Wallet);
    }

    public Hashtable LoadWallet()
    {
        var db = new DBUserData();
        Wallet = db.LoadResources();

        // It Wallet is null, this is the first time the user plays the game and we need to insert a new resources record for him
        if (Wallet == null)
        {
            SaveWallet(true);
            Wallet = db.LoadResources();
        }
        return Wallet;
    }
}
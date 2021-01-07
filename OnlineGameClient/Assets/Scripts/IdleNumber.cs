﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NumberPrefixes
{
    
    Unit = 1,
    K = 2,
    M = 3,
    B = 4,
    T = 5,
    Quad = 6,
    Quin = 7,
    Sex = 8,
    Sep = 9,

}

public enum NumberDisplayTypes
{
    
    Scientific,
    Named,
    Full,
    NamedNoDecimals,

}

public class IdleNumber
{

    public static NumberDisplayTypes NumberDisplayType = NumberDisplayTypes.Named;
    
    double decimals;
    private List<int> number;
    private string negative;


    public IdleNumber()
    {
        
        number = new List<int>();
        number.Add(0);
        negative = "";

    }
    
    public IdleNumber(double nb)//ex: 13 764 356 421 974.7535674
    {

        //handling the negative
        if (nb < 0)
        {

            negative = "-";
            nb = Math.Abs(nb);

        } else negative = "";
        
        //init the nb list
        number = new List<int>();
        
        //storing the decimals (keeping 3 decimals)
        decimals = nb - Math.Truncate(nb); //0.7535...
        decimals = Math.Round(decimals, 3);

        //converting remaining integer to string so we can split it
        double truncated = Math.Truncate(nb);
        string intStr = truncated.ToString(); //"13 764 356 421 974"
        
        //splitting the number into multiple < 1000 ints, so nb can be any size I want
        while (intStr.Length > 3)
        {

            string nbToStore = intStr.Substring(intStr.Length - 3, 3); //974, 421, 764
            number.Add(Int32.Parse(nbToStore));
            intStr = intStr.Substring(0, intStr.Length - 3);
                
        }
        
        number.Add(Int32.Parse(intStr));//13

    }

    public override string ToString()
    {

        
        
        if (number.Count == 1)
        {

            if (NumberDisplayType == NumberDisplayTypes.NamedNoDecimals) return ((int)(number[0] + decimals)).ToString();
            else return (number[0] + decimals).ToString("##0.###");

        }
        else
        {

            switch (NumberDisplayType)
            {
                
                case NumberDisplayTypes.Named:

                    //ex: 946.453 K
                return $"{negative}{number[number.Count - 1]}.{number[number.Count-2]:000} {GetNumberSuffix()}";
                case NumberDisplayTypes.Full:

                    string nb = $"{negative}{number[number.Count-1]} ";

                    for (int i = number.Count - 2; i > 0; i--)
                    {

                        nb += $"{number[i]:000} ";

                    }

                    nb += (number[0] + decimals).ToString("000.000");
                    
                return nb;
                case NumberDisplayTypes.Scientific:

                return $"{negative}{number[number.Count - 1]}.{number[number.Count - 2]}e{number.Count}"; //TODO Fix scientific display, currently a semi-tempate.
                
                case NumberDisplayTypes.NamedNoDecimals:
                    
                return $"{negative}{number[number.Count - 1]} {GetNumberSuffix()}";
                
            }

            return "ohgod something happened pls report this to dev thanks!!!";

        }
    }
    
    public string ToString(NumberDisplayTypes displayType)
    {

        
        
        if (number.Count == 1)
        {

            if (displayType == NumberDisplayTypes.NamedNoDecimals) return ((int)(number[0] + decimals)).ToString();
            else return (number[0] + decimals).ToString("##0.###");

        }
        else
        {

            switch (displayType)
            {
                
                case NumberDisplayTypes.Named:

                    //ex: 946.453 K
                return $"{negative}{number[number.Count - 1]}.{number[number.Count-2]:000} {GetNumberSuffix()}";
                case NumberDisplayTypes.Full:

                    string nb = $"{negative}{number[number.Count-1]} ";

                    for (int i = number.Count - 2; i > 0; i--)
                    {

                        nb += $"{number[i]:000} ";

                    }

                    nb += (number[0] + decimals).ToString("000.000");
                    
                return nb;
                case NumberDisplayTypes.Scientific:

                return $"{negative}{number[number.Count - 1]}.{number[number.Count - 2]}e{number.Count}"; //TODO Fix scientific display, currently a semi-tempate.
                
                case NumberDisplayTypes.NamedNoDecimals:
                    
                return $"{negative}{number[number.Count - 1]} {GetNumberSuffix()}";
                
            }

            return "ohgod something happened pls report this to dev thanks!!!";

        }
    }

    private string GetNumberSuffix()
    {

        string suffix = "";
        
        switch (number.Count)
        {
            
            case (int)NumberPrefixes.K: suffix = "K";
            break;
            case (int)NumberPrefixes.M: suffix = "M";
            break;
            case (int)NumberPrefixes.B: suffix = "B";
            break;
            case (int)NumberPrefixes.T: suffix = "T";
            break;
            case (int)NumberPrefixes.Quad: suffix = "q";
            break;
            case (int)NumberPrefixes.Quin: suffix = "Q";
            break;
            case (int)NumberPrefixes.Sep: suffix = "s";
            break;
            case (int)NumberPrefixes.Sex: suffix = "S";
            break;
            
        }

        return suffix;

    }

    public static IdleNumber operator +(IdleNumber a, IdleNumber b)
    {

        IdleNumber result = a;
        int retenue;

        if (a.number.Count < b.number.Count) return b + a;

        //adding decimals and rounding it
        result.decimals += b.decimals;
        
        //if nb >= 1, add the nb to the first 1k
        retenue = (int) result.decimals;
        result.number[0] += retenue;
        
        //remove the 1 if there is one
        result.decimals %= 1f;

        result.decimals = Math.Round(result.decimals, 3);
        Debug.Log($"DECIMAL RESULT = {result.decimals}");

        retenue = 0;
        //add all the numbers and check if they go beyond 1k
        for (int i = 0; i < b.number.Count;  i++)//123, 456
        {

            //pure add
            result.number[i] += b.number[i];
            
            //retenue for next nb
            retenue = (int)((float)result.number[i] / 1000f);
            Debug.Log($"RETENUE IS {retenue}");
            result.number[i] %= 1000;

            /* OLD METHOD WITH IFs
            //check if > 1000
            if (result.number[i] >= 1000f)
            {

                result.number[i] -= 1000;

                //move the number backwards to distribute the retenue
                for (int j = i; j < result.number.Count; j++)
                {
                    
                    //check if there is already next number, else add it
                    if (result.number.Count > j + 1)
                    {
                    
                        result.number[j + 1]++;
                        if (result.number[j + 1] < 1000) break;
                        else result.number[j + 1] -= 1000;

                    }
                    else
                    {
                    
                        //this means we're done checking
                        result.number.Add(1);
                        break;

                    }   
                    
                }

            }*/

        }
        
        //if theres still a remainder after addition, add a new int
        if (retenue != 0) result.number.Add(retenue);
        
        //addition is done, return
        return result;

    }

    public static IdleNumber operator -(IdleNumber a, IdleNumber b)
    {

        IdleNumber result = a;
        IdleNumber toAdd = b;
        int retenue = 0;

        //if a < b, then nb will be negative, so return 0
        if (a <= b) return new IdleNumber(0);

        //decimals
        result.decimals -= toAdd.decimals;
        if (result.decimals < 0)
        {

            result.decimals++;
            retenue = 1;

        }
        
        //add all the numbers and check if they go beyond 1k
        for (int i = 0; i < result.number.Count;  i++)
        {

            //pure subtract
            if (i < toAdd.number.Count) result.number[i] -= (toAdd.number[i] + retenue);
            else result.number[i] -= retenue;
            
            if (result.number[i] < 0)
            {
                
                retenue = 1;
                result.number[i] += 1000;

            }

        }

        result.Trim();

        return result;

    }

    public static bool operator >(IdleNumber a, IdleNumber b)
    {

        //simple cases
        if (a.number.Count < b.number.Count) return false;
        if (a.number.Count > b.number.Count) return true;
        
        //complicated case (=)
        for (int i = a.number.Count - 1; i >= 0; i--)
        {

            if (a.number[i] > b.number[i]) return true;

        }

        if (a.decimals > b.decimals) return true;

        return false;

    }
    
    public static bool operator >=(IdleNumber a, IdleNumber b)
    {

        //simple cases
        if (a.number.Count < b.number.Count) return false;
        if (a.number.Count > b.number.Count) return true;
        
        //complicated case (=)
        for (int i = a.number.Count - 1; i >= 0; i--)
        {

            if (a.number[i] > b.number[i]) return true;

        }

        if (a.decimals >= b.decimals) return true;

        return false;

    }
    
    public static bool operator <(IdleNumber a, IdleNumber b)
    {
        
        //simple cases
        if (a.number.Count < b.number.Count) return true;
        if (a.number.Count > b.number.Count) return false;
        
        //complicated case (=)
        for (int i = a.number.Count - 1; i >= 0; i--)
        {

            if (a.number[i] < b.number[i]) return true;

        }

        if (a.decimals < b.decimals) return true;

        return false;
        
    }
    
    public static bool operator <=(IdleNumber a, IdleNumber b)
    {
        
        //simple cases
        if (a.number.Count < b.number.Count) return true;
        if (a.number.Count > b.number.Count) return false;
        
        //complicated case (=)
        for (int i = a.number.Count - 1; i >= 0; i--)
        {

            if (a.number[i] < b.number[i]) return true;

        }

        if (a.decimals <= b.decimals) return true;

        return false;
        
    }

    public void Trim()
    {

        List<int> tempNumber = number;
        for (int i = number.Count - 1; i >= 0; i--)
        {

            if (tempNumber[i] == 0) tempNumber.Remove(i);
            else break;

        }
        
    }

}

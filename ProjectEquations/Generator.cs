using OxMath;
using System;
using System.Collections.Generic;

public class Generator
{
    public const byte BASE = 2, SEPERATOR = 0;
    public enum SeedType
    {
        COMPRESSED,
        SBYTE,
        SHORT,
        INT,
        LONG,
        FLOAT,
        DOUBLE,
        INFININT,
    }
    public enum Sign
    {
        POSITIVE = 1,
        NEGATIVE,
    }

    public static Vector3[] GenerateModel(int seed)
    {
        Vector3[] vertices = null;

        return vertices;
    }
    public static int GenerateSeedFromModel(Vector3[] vertices)
    {
        int seed = -1;
        foreach (Vector3 vector in vertices)
        {

        }
        return seed;
    }

    /*public static Vector3 GenerateVector3Point(int seed, int decimalPlaces)
    {
        int seedCopy = seed;

        float thirdNumber = (((seedCopy) / 4) > 0) ? (((seedCopy / 8) + 1) * ((float)(1 / Math.Pow(10, decimalPlaces)))) : 0;
        seedCopy -= (((seedCopy) / 4) > 0) ? 4 : 0;
        float secondNumber = (((seedCopy) / 2) > 0) ? (((seedCopy / 8) + 1) * ((float)(1 / Math.Pow(10, decimalPlaces)))) : 0;
        seedCopy -= (((seedCopy) / 2) > 0) ? 2 : 0;
        float firstNumber = (seedCopy % 2) == 1 ? (((seedCopy / 8) + 1) * ((float)(1 / Math.Pow(10, decimalPlaces)))) : 0;
        seedCopy -= (seedCopy % 2) == 1 ? 1 : 0;
        

        return new Vector3(firstNumber, secondNumber, thirdNumber);
    }*/
    public static Vector3 GenerateVector3Point(infinint seed, int decimalPlaces)
    {
        return Vector3.zero;
    }
    public static infinint GenerateSeedFromVector3Point(Vector3 point)
    {
        infinint seed;

        string numberString = point.x.ToString();
        int dotIndex = numberString.IndexOf("."), decimalSize = 0;
        if (dotIndex > -1) decimalSize = numberString.Length - dotIndex - 1;
        seed = (long)Math.Round(point.x * Math.Pow(10, decimalSize));

        numberString = point.y.ToString();
        dotIndex = numberString.IndexOf(".");
        decimalSize = 0;
        if (dotIndex > -1) decimalSize = numberString.Length - dotIndex - 1;
        seed.AppendToEnd((long)Math.Round(point.y * Math.Pow(10, decimalSize)));

        numberString = point.z.ToString();
        dotIndex = numberString.IndexOf(".");
        decimalSize = 0;
        if (dotIndex > -1) decimalSize = numberString.Length - dotIndex - 1;
        seed.AppendToEnd((long)Math.Round(point.z * Math.Pow(10, decimalSize)));

        return seed;
    }

    public static infinint GenerateSeedFromNumber(object number)
    {
        infinint seed = -1;
        string info = "";

        if (number is double || number is float)
        {
            double castedNumber = Convert.ToDouble(number.ToString());
            
            string numberString = castedNumber.ToString();
            int dotIndex = numberString.IndexOf("."), decimalSize = 0;
            if (dotIndex > -1) decimalSize = numberString.Length - dotIndex - 1;
            seed = Math.Abs((long)Math.Round(castedNumber * Math.Pow(10, decimalSize)));

            info += SEPERATOR;
            info += decimalSize;

            if (castedNumber < 0) info += (byte)Sign.NEGATIVE;
            else info += (byte)Sign.POSITIVE;

            if (number is double) info += (byte)SeedType.DOUBLE;
            else if (number is float) info += (byte)SeedType.FLOAT;
        }
        else if (number is sbyte || number is short || number is int || number is long)
        {
            long castedNumber = Convert.ToInt64(number);

            seed = Math.Abs(castedNumber);

            if (castedNumber < 0) info += (byte)Sign.NEGATIVE;
            else info += (byte)Sign.POSITIVE;

            if (number is sbyte) info += (byte)SeedType.SBYTE;
            else if (number is short) info += (byte)SeedType.SHORT;
            else if (number is int) info += (byte)SeedType.INT;
            else if (number is long) info += (byte)SeedType.LONG;
        }
        else if (number is infinint)
        {
            infinint castedNumber = new infinint((infinint)number);

            seed = infinint.Abs(castedNumber);

            if (castedNumber < 0) info += (byte)Sign.NEGATIVE;
            else info += (byte)Sign.POSITIVE;

            info += (byte)SeedType.INFININT;
        }

        seed.AppendToEnd((infinint)info);

        byte compressionBase = 2;
        infinint power = 0;
        infinint offset = 0;
        for (; compressionBase < 20; compressionBase++)
        {
            power = infinint.Log(compressionBase, seed);
            infinint powered = infinint.Pow(compressionBase, power);
            offset = new infinint(seed - powered);
            Console.WriteLine("seed: " + seed + " base: " + compressionBase + " power: " + power + " powered: " + powered + " offset: " + offset);
        }
        //infinint power = infinint.Log(BASE, seed) - 1;
        //infinint powered = infinint.Pow(BASE, power);
        //infinint offset = new infinint(seed - powered);
        //Console.WriteLine("seed: " + seed + " power: " + power + " powered: " + powered + " offset: " + offset);
        
        offset.AppendToEnd(SEPERATOR);
        info = infinint.DecimalToOctal(power).ShiftNumbers(1).ToString();
        info += (byte)SeedType.COMPRESSED;
        offset.AppendToEnd((infinint)info);

        return offset;
    }
    public static object GenerateNumberFromSeed(infinint seed)
    {
        object number = null;
        SeedType seedType = (SeedType)seed.DigitAt(seed.length - 1);
        infinint seedCopy = new infinint(seed);
        seedCopy.RemoveAt(seedCopy.length - 1);

        Console.WriteLine(seedType);

        if (seedType == SeedType.COMPRESSED)
        {
            byte nextDigit = seedCopy.DigitAt(seedCopy.length - 1);
            seedCopy.RemoveAt(seedCopy.length - 1);
            infinint power = nextDigit;

            while (nextDigit != SEPERATOR)
            {
                nextDigit = seedCopy.DigitAt(seedCopy.length - 1);
                seedCopy.RemoveAt(seedCopy.length - 1);
                
                if(nextDigit != SEPERATOR) power.AppendToBeginning(nextDigit);
            }
            
            power.ShiftNumbers(-1);
            power = infinint.OctalToDecimal(power);

            Console.WriteLine(seedCopy + " + " + BASE + "^" + power);
            number = new infinint(infinint.Pow(BASE, power) + seedCopy);
        }
        else if (seedType == SeedType.FLOAT || seedType == SeedType.DOUBLE)
        {

        }
        else if (seedType == SeedType.SHORT || seedType == SeedType.INT || seedType == SeedType.LONG)
        {

        }
        else if(seedType == SeedType.INFININT)
        {

        }

        return number;
    }

    public static void GetCombinations(int number, int numberOfNumbers, byte key)
    {
        if (number > 0 && numberOfNumbers > 1 && key > 1 && key < 10)
        {
            //byte[] decimalKey = { 0, (byte)(1 * key), (byte)(2 * key), (byte)(3 * key), (byte)(4 * key), (byte)(5 * key), (byte)(6 * key), (byte)(7 * key), (byte)(8 * key), (byte)(9 * key) };
            List<int[]> validCombinations = new List<int[]>();

            TCombinator combinator = new TCombinator(9, numberOfNumbers - 1);
            Console.WriteLine(combinator.CalcCombNum(9, (ulong)numberOfNumbers - 1));

            int combinationNumber = 1;
            while(!combinator.Finished)
            {
                Console.Write(combinationNumber + ": ");
                int[] combination = combinator.CombSet;
                int sum = 0;
                foreach (int comb in combination)
                {
                    Console.Write(comb * key + " ");
                    sum += comb * key;
                }
                if (sum == number) validCombinations.Add(combination);
                combinator.NextCombin();
                Console.WriteLine("= " + sum);
                combinationNumber++;
            }
            Console.WriteLine("Valid Combinations: " + validCombinations.Count);
        }
        else Console.WriteLine("Bad Number or Length or Key");
    }
}

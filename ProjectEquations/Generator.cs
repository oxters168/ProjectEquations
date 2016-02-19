using OxMath;
using System;
//using System.Collections.Generic;

public class Generator
{
    public const byte SEPERATOR = 3;
    public enum SeedType
    {
        SBYTE,
        BYTE,
        SHORT,
        USHORT,
        INT,
        UINT,
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
        else if (number is sbyte || number is byte || number is short || number is ushort || number is int || number is uint || number is long)
        {
            long castedNumber = Convert.ToInt64(number);

            seed = Math.Abs(castedNumber);

            if (castedNumber < 0) info += (byte)Sign.NEGATIVE;
            else info += (byte)Sign.POSITIVE;

            if (number is sbyte) info += (byte)SeedType.SBYTE;
            else if (number is byte) info += (byte)SeedType.BYTE;
            else if (number is short) info += (byte)SeedType.SHORT;
            else if (number is ushort) info += (byte)SeedType.USHORT;
            else if (number is int) info += (byte)SeedType.INT;
            else if (number is uint) info += (byte)SeedType.UINT;
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

        return seed;
    }
}

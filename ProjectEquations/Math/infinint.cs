using System;
using System.Collections.Generic;

namespace OxMath
{
    public class infinint
    {
        private List<byte> digits;
        private bool negative = false;
        public int length { get { return digits != null ? digits.Count : -1; } private set { } }

        #region Constructors
        public infinint(string number)
        {
            if (number.Length > 0 && number[0] == '-')
            {
                bool zero = true;
                for (int i = 1; i < number.Length; i++)
                {
                    if (number[i] != '0') { zero = false; break; }
                }
                if (!zero) negative = true;
                number = number.Substring(1);
            }
            digits = Parse(number);
        }
        public infinint(long number)
        {
            if (number < 0) { negative = true; number *= -1; }
            digits = Parse(number);
        }
        public infinint(ulong number)
        {
            digits = Parse(number);
        }
        public infinint(infinint other)
        {
            Clone(other);
        }
        #endregion

        #region Operator Overloading
        public static implicit operator infinint(long number)
        {
            return new infinint(number);
        }
        public static explicit operator infinint(string number)
        {
            return new infinint(number);
        }
        public static explicit operator long(infinint number)
        {
            if (number > long.MaxValue || number < long.MinValue) throw new InvalidCastException();
            string numberString = "";
            for (int i = number.digits.Count - 1; i >= 0; i--)
            {
                numberString += number.digits[i].ToString();
            }
            return Convert.ToInt64(numberString);
        }

        public static infinint operator -(infinint infinint)
        {
            infinint flipped = new infinint(infinint);
            if (flipped == 0) { flipped.negative = false; return flipped; }
            flipped.negative = !flipped.negative;
            return flipped;
        }

        public static infinint operator +(infinint infinint1, infinint infinint2)
        {
            infinint sum = new infinint(infinint1);
            if (!infinint1.negative && infinint2.negative) return infinint1 - Abs(infinint2);
            else if (infinint1.negative && !infinint2.negative) return -(Abs(infinint1) - infinint2);

            for (int i = 0; i < infinint2.digits.Count; i++)
            {
                sum.AddAt(i, infinint2.digits[i]);
            }

            return sum;
        }
        public static infinint operator -(infinint infinint1, infinint infinint2)
        {
            infinint difference = new infinint(infinint1);
            if (!infinint1.negative && infinint2.negative) return infinint1 + Abs(infinint2);
            else if (infinint1.negative && !infinint2.negative) return -(Abs(infinint1) + infinint2);
            else if ((infinint2 > infinint1 && !infinint1.negative && !infinint2.negative) || (infinint1 > infinint2 && infinint1.negative && infinint2.negative)) { difference = -infinint2; infinint2 = infinint1; }

            for (int i = infinint2.digits.Count - 1; i >= 0; i--)
            {
                difference.SubtractAt(i, infinint2.digits[i]);
            }

            return difference;
        }
        public static infinint operator *(infinint infinint1, infinint infinint2)
        {
            if (infinint1.length == 1 || infinint2.length == 1) return SlowMultiplication(infinint1, infinint2);
            return SchonhageStrassenMultiplication(infinint1, infinint2);
        }
        public static infinint operator /(infinint infinint1, infinint infinint2)
        {
            return SlowDivision(infinint1, infinint2);
        }
        public static infinint operator %(infinint infinint1, infinint infinint2)
        {
            if (infinint2 == 0) throw new DivideByZeroException();

            infinint quotient = 0;
            infinint remainder = Abs(infinint1);
            infinint abs2 = Abs(infinint2);

            while (remainder >= abs2) { remainder -= abs2; quotient++; }

            if ((infinint1.negative && !infinint2.negative) || (!infinint1.negative && infinint2.negative)) { quotient = -quotient; remainder = -remainder; }

            return remainder;
        }

        public static infinint operator ++(infinint infinint)
        {
            return infinint += 1;
        }
        public static infinint operator --(infinint infinint)
        {
            return infinint -= 1;
        }

        public static bool operator >(infinint infinint1, infinint infinint2)
        {
            if (infinint1.negative && !infinint2.negative) return false;
            else if (infinint2.negative && !infinint1.negative) return true;
            
            if (infinint1.negative && infinint2.negative) return Abs(infinint1) < Abs(infinint2);

            if (infinint1.digits.Count > infinint2.digits.Count) return true;
            else if (infinint2.digits.Count > infinint1.digits.Count) return false;

            for (int i = infinint1.digits.Count - 1; i >= 0; i--)
            {
                if (infinint2.digits[i] > infinint1.digits[i]) return false;
                else if (infinint1.digits[i] > infinint2.digits[i]) return true;
            }

            return false;
        }
        public static bool operator >=(infinint infinint1, infinint infinint2)
        {
            if (infinint1.negative && !infinint2.negative) return false;
            else if (infinint2.negative && !infinint1.negative) return true;

            if (infinint1.negative && infinint2.negative) return Abs(infinint1) <= Abs(infinint2);

            if (infinint1.digits.Count > infinint2.digits.Count) return true;
            else if (infinint2.digits.Count > infinint1.digits.Count) return false;

            for (int i = infinint1.digits.Count - 1; i >= 0; i--)
            {
                if (infinint2.digits[i] > infinint1.digits[i]) return false;
                else if (infinint1.digits[i] > infinint2.digits[i]) return true;
            }

            return true;
        }
        public static bool operator <(infinint infinint1, infinint infinint2)
        {
            if (!infinint1.negative && infinint2.negative) return false;
            else if (!infinint2.negative && infinint1.negative) return true;
            
            if (infinint1.negative && infinint2.negative) return Abs(infinint1) > Abs(infinint2);

            if (infinint1.digits.Count < infinint2.digits.Count) return true;
            else if (infinint2.digits.Count < infinint1.digits.Count) return false;

            for (int i = infinint1.digits.Count - 1; i >= 0; i--)
            {
                if (infinint2.digits[i] < infinint1.digits[i]) return false;
                else if (infinint1.digits[i] < infinint2.digits[i]) return true;
            }

            return false;
        }
        public static bool operator <=(infinint infinint1, infinint infinint2)
        {
            if (!infinint1.negative && infinint2.negative) return false;
            else if (!infinint2.negative && infinint1.negative) return true;

            if (infinint1.negative && infinint2.negative) return Abs(infinint1) >= Abs(infinint2);

            if (infinint1.digits.Count < infinint2.digits.Count) return true;
            else if (infinint2.digits.Count < infinint1.digits.Count) return false;

            for (int i = infinint1.digits.Count - 1; i >= 0; i--)
            {
                if (infinint2.digits[i] < infinint1.digits[i]) return false;
                else if (infinint1.digits[i] < infinint2.digits[i]) return true;
            }

            return true;
        }
        public static bool operator ==(infinint infinint1, infinint infinint2)
        {
            return infinint1.Equals(infinint2);
        }
        public static bool operator !=(infinint infinint1, infinint infinint2)
        {
            return !infinint1.Equals(infinint2);
        }
        #endregion

        #region object Overrides
        public override bool Equals(object obj)
        {
            if (obj is infinint)
            {
                if (negative && !((infinint)obj).negative || digits.Count != ((infinint)obj).digits.Count) return false;
                for (int i = 0; i < digits.Count; i++)
                {
                    if (digits[i] != ((infinint)obj).digits[i]) return false;
                }
                return true;
            }
            return false;
        }
        public override String ToString()
        {
            String infinint = "";
            if (negative) infinint += "-";
            for (int i = digits.Count - 1; i >= 0; i--)
            {
                infinint += digits[i];
            }
            return infinint;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        public void Clone(infinint other)
        {
            negative = other.negative;

            digits = new List<byte>(other.digits);
        }
        public void AppendToEnd(infinint other)
        {
            if (this == 0)
            {
                digits = new List<byte>(other.digits);
            }
            else
            {
                digits.InsertRange(0, other.digits);
            }
        }
        public void AppendToBeginning(infinint other)
        {
            digits.AddRange(other.digits);
        }
        public infinint ShiftNumbers(sbyte amount)
        {
            if (amount < -9 || amount > 9) throw new Exception("Value must be between -9 and 9");

            for(int i = 0; i < digits.Count; i++)
            {
                if (digits[i] + amount > 9) digits[i] = (byte)(digits[i] + amount - 10);
                else if (digits[i] + amount < 0) digits[i] = (byte)(10 + digits[i] + amount);
                else digits[i] = (byte)(digits[i] + amount);
            }

            return this;
        }
        public byte DigitAt(int index)
        {
            if (index < 0 || index >= digits.Count) throw new IndexOutOfRangeException();
            return digits[digits.Count - index - 1];
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= digits.Count) throw new IndexOutOfRangeException();
            if (digits.Count == 1) digits[0] = 0;
            else digits.RemoveAt(digits.Count - index - 1);
        }

        #region Arithmetics
        private void AddAt(int index, byte number)
        {
            //if (number >= 0 && number < 10)
            //{
                if (index > digits.Count || index < 0) throw new Exception("Index out of range");

                if (index > digits.Count - 1)
                {
                    digits.Add(number);
                }
                else if (digits[index] + number > 9)
                {
                    byte sum = (byte)(digits[index] + number);
                    digits[index] = (byte)(sum % 10);
                    AddAt(index + 1, (byte)(sum / 10));
                }
                else
                {
                    digits[index] += number;
                }
            //}
            //else throw new Exception("Number too large");
        }
        private void SubtractAt(int index, byte number)
        {
            //if (number >= 0 && number < 10)
            //{
                if (index >= digits.Count || index < 0) throw new Exception("Index out of range");

                if (digits[index] <= number && index == digits.Count - 1 && index != 0)
                {
                    digits.RemoveAt(digits.Count - 1);
                }
                else if (digits[index] < number)
                {
                    if (digits.Count - 1 > index)
                    {
                        digits[index] = (byte)((digits[index] + 10) - number);
                        SubtractAt(index + 1, 1);
                    }
                    else
                    {
                        digits[index] = (byte)(number - digits[index]);
                        //SubtractAt(index + 1, 1);
                    }
                }
                else
                {
                    digits[index] -= number;
                }
            //}
            //else throw new Exception("Number too large");
        }

        public static infinint SlowMultiplication(infinint infinint1, infinint infinint2)
        {
            infinint product = 0;

            for (int i = 0; i < infinint2; i++)
            {
                product += infinint1;
            }

            if ((infinint1.negative && !infinint2.negative) || (!infinint1.negative && infinint2.negative)) { product = -product; }

            return product;
        }
        /*public static infinint SchonhageStrassenMultiplication(infinint infinint1, infinint infinint2)
        {
            int n = infinint1.digits.Count, m = infinint2.digits.Count;
            infinint[] linearConvolution = new infinint[n + m - 1];

            for (int i = 0; i < (n + m - 1); i++)
                linearConvolution[i] = 0;

            infinint p = infinint1;

            for (int i = 0; i < m; i++)
            {
                infinint1 = p;
                for (int j = 0; j < n; j++)
                {
                    linearConvolution[i + j] += infinint.SlowMultiplication((infinint2 % 10), (infinint1 % 10));
                    infinint1 /= 10;
                }
                infinint2 /= 10;
            }
            Console.Write("The Linear Convolution is: ( ");

            for (int i = (n + m - 2); i >= 0; i--)
            {
                Console.Write(linearConvolution[i] + " ");
            }

            Console.WriteLine(")");

            infinint product = 0;
            infinint nextCarry = 0, baseInt = 1; ;

            for (int i = 0; i < n + m - 1; i++)
            {
                linearConvolution[i] += nextCarry;
                product = product + infinint.SlowMultiplication(baseInt, (linearConvolution[i] % 10));
                nextCarry = linearConvolution[i] / 10;
                infinint.SlowMultiplication(baseInt, 10);
            }

            Console.WriteLine("The Product of the numbers is: " + product);
            return product;
        }*/
        public static infinint SchonhageStrassenMultiplication(infinint infinint1, infinint infinint2)
        {
            //Source: http://www.sanfoundry.com/java-program-implement-schonhage-strassen-algorithm-for-multiplicatitwo-numbers/

            int infinint1Length = infinint1.length, infinint2Length = infinint2.length;
            infinint[] linearConvolution = new infinint[infinint1Length + infinint2Length - 1];

            for (int i = 0; i < (infinint1Length + infinint2Length - 1); i++)
                linearConvolution[i] = 0;

            for (int i = infinint1Length - 1; i >= 0; i--)
            {
                for (int j = infinint2Length - 1; j >= 0; j--)
                {
                    linearConvolution[linearConvolution.Length - (i + j) - 1] += (infinint1.DigitAt(i)) * (infinint2.DigitAt(j));
                }
            }

            infinint product = 0, nextCarry = 0, baseNumber = 1;

            for (int i = 0; i < infinint1Length + infinint2Length - 1; i++)
            {
                linearConvolution[i] += nextCarry;
                product = product + infinint.SlowMultiplication(baseNumber, (linearConvolution[i].DigitAt(linearConvolution[i].length - 1)));
                nextCarry = new infinint(linearConvolution[i]);
                nextCarry.RemoveAt(nextCarry.length - 1);
                baseNumber.AppendToEnd(0);
            }

            return product;
        }

        public static infinint SlowDivision(infinint infinint1, infinint infinint2)
        {
            if (infinint2 == 0) throw new DivideByZeroException();

            infinint quotient = 0;
            infinint remainder = Abs(infinint1);
            infinint abs2 = Abs(infinint2);

            while (remainder >= abs2) { remainder -= abs2; quotient++; }

            if ((infinint1.negative && !infinint2.negative) || (!infinint1.negative && infinint2.negative)) { quotient = -quotient; remainder = -remainder; }

            return quotient;
        }

        public static infinint Abs(infinint infinint)
        {
            infinint abs = new infinint(infinint);
            abs.negative = false;
            return abs;
        }
        public static infinint Pow(infinint baseInt, infinint power)
        {
            if (power == 0) return 1;
            if (baseInt == 0) return 0;

            infinint powered = new infinint(baseInt);
            //Console.WriteLine("Base: " + baseInt + " Power: " + power);
            for (int i = 1; i < power; i++)
            {
                powered *= baseInt;
            }
            return powered;
        }
        public static infinint Log(infinint baseInt, infinint powered)
        {
            if (baseInt > powered) throw new Exception("Base can't be larger than product");
            if (powered == 0 || powered == 1) return 0;

            infinint unpowered = new infinint(baseInt);
            infinint power = 1;
            infinint distance = powered - unpowered;

            while (unpowered < powered)
            {
                power++;
                unpowered *= baseInt;
                if (powered < unpowered && distance < unpowered - powered) power--;
                distance = powered - unpowered;
            }
            //power--;
            return power;
        }
        public static infinint DecimalToOctal(infinint value)
        {
            infinint octal = 0;
            infinint quotient = new infinint(value);
            infinint index = 0;

            while(quotient > 0)
            {
                octal += (quotient % 8) * infinint.Pow(10, index);
                quotient /= 8;
                index++;
            }

            return octal;
        }
        public static infinint OctalToDecimal(infinint value)
        {
            infinint decim = 0;

            for(int i = value.digits.Count - 1; i >= 0; i--)
            {
                decim *= 8;
                decim += value.digits[i];
            }

            return decim;
        }

        public static infinint Convertish(infinint original, byte baseNumber)
        {
            infinint converted = 0;

            for(int i = original.digits.Count - 1; i >= 0; i--)
            {
                converted *= (baseNumber / 2);
                //converted /= 2;
                converted += original.digits[i];
            }

            return converted;
        }
        public static infinint Unconvertish(infinint converted, byte baseNumber)
        {
            infinint original = 0;
            infinint unconverter = new infinint(converted);

            infinint place = 0;
            while(unconverter > baseNumber)
            {
                
                byte nextNumber = (byte)(unconverter % baseNumber);
                unconverter /= baseNumber;
                unconverter *= 2;
                if (place == 0) original = nextNumber;
                else
                {
                    original += nextNumber * infinint.Pow(10, place);
                }
                
                place++;
            }
            original += unconverter * infinint.Pow(10, place);

            return original;
        }
        #endregion

        #region Parsers
        private static List<byte> Parse(string number)
        {
            number = number.TrimStart('0');
            List<byte> digits = new List<byte>();

            for (int i = number.Length - 1; i >= 0; i--)
            {
                if (!char.IsDigit(number[i])) throw new Exception("Could not convert " + number[i] + " to a digit");
                digits.Add(Convert.ToByte(number[i].ToString()));
            }

            return digits;
        }
        private static List<byte> Parse(long number)
        {
            List<byte> parsedDigits = new List<byte>();
            //number = Math.Abs(number);
            String reverseParse = number.ToString();
            if (reverseParse.IndexOf("-") > -1) reverseParse = reverseParse.Substring(1);
            for (int i = reverseParse.Length - 1; i >= 0; i--)
            {
                parsedDigits.Add(Convert.ToByte(reverseParse[i].ToString()));
            }
            return parsedDigits;
        }
        private static List<byte> Parse(ulong number)
        {
            List<byte> parsedDigits = new List<byte>();
            String reverseParse = number.ToString();
            for (int i = reverseParse.Length - 1; i >= 0; i--)
            {
                parsedDigits.Add(Convert.ToByte(reverseParse[i].ToString()));
            }
            return parsedDigits;
        }
        #endregion
    }
}

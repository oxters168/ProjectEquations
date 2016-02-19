using System;
using System.Collections.Generic;

namespace OxMath
{
    public class infinint
    {
        private List<byte> digits;
        private bool negative = false;

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
        public static explicit operator infinint(ulong number)
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
            for (int i = number.digits.Count; i >= 0; i--)
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
            infinint product = 0;

            for (int i = 0; i < infinint2.digits.Count; i++)
            {
                for (int j = 0; j < infinint1.digits.Count; j++)
                {
                    byte miniProduct = (byte)(infinint2.digits[i] * infinint1.digits[j]);
                    if (miniProduct < 10)
                    {
                        if(miniProduct > 0) product.AddAt((i) + j, miniProduct);
                    }
                    else
                    {
                        product.AddAt((i) + j + 0, (byte)(miniProduct % 10));
                        product.AddAt((i) + j + 1, (byte)(miniProduct / 10));
                    }
                }
            }

            product.negative = infinint1.negative;
            if (infinint2.negative) product = -product;
            return product;
        }
        /*public static infinint operator /(infinint infinint1, infinint infinint2)
        {
            if (infinint2 == 0) throw new DivideByZeroException();

            infinint quotient = 0;

            for (int i = infinint1.digits.Count - 1; i >= 0; i--)
            {
                for (int j = infinint2.digits.Count - 1; j >= 0; j--)
                {
                    byte miniQuotient = (byte)(infinint1.digits[i] / infinint2.digits[j]);
                    if (miniQuotient < 10)
                    {
                        if (miniQuotient > 0) quotient.AddAt(i + j, miniQuotient);
                    }
                    else
                    {
                        quotient.AddAt((i) + j + 0, (byte)(miniQuotient % 10));
                        quotient.AddAt((i) + j + 1, (byte)(miniQuotient / 10));
                    }
                }
            }

            quotient.negative = infinint1.negative;
            if (infinint2.negative) quotient = -quotient;
            return quotient;
        }*/
        /*public static infinint operator +(infinint infinint, long number)
        {
            return infinint + number;
        }
        public static infinint operator -(infinint infinint, long number)
        {
            return infinint - number;
        }
        public static infinint operator *(infinint infinint, long number)
        {
            return infinint * number;
        }
        public static infinint operator +(long number, infinint infinint)
        {
            return number + infinint;
        }
        public static infinint operator -(long number, infinint infinint)
        {
            return number - infinint;
        }
        public static infinint operator *(long number, infinint infinint)
        {
            return number * infinint;
        }*/

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
        /*public static bool operator >(infinint infinint, long aLong)
        {
            return infinint > aLong;
        }
        public static bool operator >=(infinint infinint, long aLong)
        {
            return infinint >= aLong;
        }
        public static bool operator <(infinint infinint, long aLong)
        {
            return infinint < aLong;
        }
        public static bool operator <=(infinint infinint, long aLong)
        {
            return infinint <= aLong;
        }
        public static bool operator >(long aLong, infinint infinint)
        {
            return aLong > infinint;
        }
        public static bool operator >=(long aLong, infinint infinint)
        {
            return aLong >= infinint;
        }
        public static bool operator <(long aLong, infinint infinint)
        {
            return aLong < infinint;
        }
        public static bool operator <=(long aLong, infinint infinint)
        {
            return aLong <= infinint;
        }*/
        public static bool operator ==(infinint infinint1, infinint infinint2)
        {
            return infinint1.Equals(infinint2);
        }
        public static bool operator !=(infinint infinint1, infinint infinint2)
        {
            return !infinint1.Equals(infinint2);
        }
        /*public static bool operator ==(infinint infinint, long aLong)
        {
            return infinint == aLong;
        }
        public static bool operator !=(infinint infinint, long aLong)
        {
            return infinint != aLong;
        }
        public static bool operator ==(long aLong, infinint infinint)
        {
            return aLong == infinint;
        }
        public static bool operator !=(long aLong, infinint infinint)
        {
            return aLong != infinint;
        }*/
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

            digits = new List<byte>();
            for (int i = 0; i < other.digits.Count; i++)
            {
                AddAt(i, other.digits[i]);
            }
            //return clone;
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

        #region Arithmetics
        private void AddAt(int index, byte number)
        {
            if (number >= 0 && number < 10)
            {
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
            }
            else throw new Exception("Number too large");
        }
        private void SubtractAt(int index, byte number)
        {
            if (number >= 0 && number < 10)
            {
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
            }
            else throw new Exception("Number too large");
        }
        public static infinint Abs(infinint infinint)
        {
            infinint abs = new infinint(infinint);
            abs.negative = false;
            return abs;
        }
        #endregion

        #region Parsers
        private static List<byte> Parse(string number)
        {
            number = number.TrimStart('0');
            List<byte> digits = new List<byte>();

            for (int i = number.Length - 1; i >= 0; i--)
            {
                digits.Add(Convert.ToByte(number[i].ToString()));
            }

            return digits;
        }
        private static List<byte> Parse(long number)
        {
            List<byte> parsedDigits = new List<byte>();
            //number = Math.Abs(number);
            String reverseParse = number.ToString();
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

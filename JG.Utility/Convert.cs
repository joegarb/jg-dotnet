using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JG.Utility
{
    public class Convert
    {
        private static string[] zero_to_nineteen = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        private static string[] twenty_to_ninety = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        private static string[] thousand_and_up_denominations = { "thousand", "million", "billion" };

        private Convert() { }

        /// <summary>
        /// Convert an integer to English language words
        /// </summary>
        public static string ToWords(int value)
        {
            value = Math.Abs(value);

            if (value < 1000)
                return ToWordsLessThanOneThousand(value);

            string result = "";
            for (int i = 0; i < thousand_and_up_denominations.Length; i++)
            {
                if ((long)Math.Pow(1000, i + 2) > value)
                {
                    int denominationValue = (int)Math.Pow(1000, i + 1);
                    int leadingValue = value / denominationValue;
                    int remainder = value - (leadingValue * denominationValue);

                    result = ToWordsLessThanOneThousand(leadingValue) + " " + thousand_and_up_denominations[i];
                    if (remainder > 0)
                        result = result + " " + ToWords(remainder);

                    break;
                }
            }
            return result;
        }

        private static string ToWordsLessThanOneThousand(int value)
        {
            string result = "";

            // Convert the hundreds digit
            int hundredsDigit = value / 100;
            int remainderLessThanOneHundred = value % 100;
            if (hundredsDigit > 0)
            {
                result = zero_to_nineteen[hundredsDigit] + " hundred";
                if (remainderLessThanOneHundred > 0)
                    result += " ";
            }

            // Convert the tens and single digits
            if (remainderLessThanOneHundred > 0 || value == 0)
            {
                if (remainderLessThanOneHundred < 20)
                {
                    result += zero_to_nineteen[remainderLessThanOneHundred];
                }
                else
                {
                    result += twenty_to_ninety[(remainderLessThanOneHundred - 20) / 10];
                    int remainderLessThanTwenty = (remainderLessThanOneHundred - 20) % 10;
                    if (remainderLessThanTwenty  > 0)
                        result += " " + zero_to_nineteen[remainderLessThanTwenty];
                }
            }

            return result;
        }
    }
}
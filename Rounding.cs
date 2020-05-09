using System;
using System.Collections.Generic;
using System.Text;

namespace NablaUtils
{
    public static class Rounding
    {
        public static decimal? RoundUp(this decimal? input, int places)
        {
            if (!input.HasValue)
            {
                return null;
            }
            else
            {
                return RoundUp(input.Value, places);
            }
        }

        public static decimal RoundUp(this decimal input, int places)
        {
            decimal multiplier = Convert.ToDecimal(Math.Pow(10, Convert.ToDouble(places)));
            return Math.Ceiling(input * multiplier) / multiplier;
        }

        public static double? RoundUp(this double? input, int places)
        {
            if (!input.HasValue)
            {
                return null;
            }
            else
            {
                return RoundUp(input.Value, places);
            }
        }

        public static double RoundUp(this double input, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));
            return Math.Ceiling(input * multiplier) / multiplier;
        }
    }
}

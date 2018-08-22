using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stargazer
{
    public static class StarCalculator
    {
        public static bool isStarVisible(double userLatitude, double starDeclination)
        {
            double result = (90 - userLatitude) + starDeclination;

            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
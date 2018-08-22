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
            double result;
            if (userLatitude > 0)
            {
                result = (90 - userLatitude) + starDeclination;
                if (result > 0) { return true; }
            }
            else
            {
                result = (-90 - userLatitude) + starDeclination;
                if (result < 0) { return true; }
            }

            return false;
        }

        public static double getVisibleLatitude(double userLatitude, double starDeclination)
        {
            if (userLatitude > 0)
            {
                return (-starDeclination - 90) * -1;
            }
            else
            {
                return (-starDeclination + 90) * -1;
            }
        }

    }
}
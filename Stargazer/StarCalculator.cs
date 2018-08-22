using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Stargazer
{
    public static class StarCalculator
    {
        private static DateTime vernalEquinox = DateTime.Parse("March 21");
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

        public static string GetPeakVisibilityMonth(double rightAscension)
        {
            if (Math.Floor(rightAscension)%2 == 0)
            {
                rightAscension = Math.Floor(rightAscension);
            }
            else
            {
                rightAscension = Math.Ceiling(rightAscension);
            }

            int monthsToWorstMonth = (int)rightAscension / 2;
            DateTime worstMonth = vernalEquinox.AddMonths(monthsToWorstMonth);
            DateTime bestMonth = worstMonth.AddMonths(6);
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(bestMonth.Month);

        }

    }
}
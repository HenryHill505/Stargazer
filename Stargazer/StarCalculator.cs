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

        public static string GetRequiredMagnification(double magnitude)
        {
            if (magnitude <= 6)
            {
                return "Naked Eye";
            }
            if (magnitude <= 8) { return "20mm Telescope"; }
            if (magnitude <= 9) { return "30mm Telescope"; }
            if (magnitude <= 10) { return "50mm Telescope"; }
            if (magnitude <= 11) { return "70mm Telescope"; }
            if (magnitude <= 12) { return "120mm Telescope"; }
            if (magnitude <= 13) { return "180mm Telescope"; }
            return "Go to an observatory";

        }

    }
}
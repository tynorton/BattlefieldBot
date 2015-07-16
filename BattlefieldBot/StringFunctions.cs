using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattlefieldBot
{
    public static class StringFunctions
    {
        private const string AGE_FORMAT = "{0} {1}{2} ago";

        private static PluralizationService s_pluralizationService = PluralizationService.CreateService(CultureInfo.CurrentUICulture);

        public static string GetAgeString(DateTime oldUtcDate)
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeSpan age = utcNow - oldUtcDate;

            string word = string.Empty;
            string bonusWord = string.Empty;
            int totalUnit = 0;
            int totalBonusUnit = 0;
            if (age.TotalMinutes < 1)
            {
                totalUnit = Convert.ToInt32(Math.Floor(age.TotalSeconds));
                word = "second";
            }
            else if (age.TotalHours < 1)
            {
                totalUnit = Convert.ToInt32(Math.Floor(age.TotalMinutes));
                word = "minute";
            }
            else if (age.TotalHours > 1 && age.TotalDays < 1)
            {
                totalUnit = Convert.ToInt32(Math.Floor(age.TotalHours));
                word = "hour";

                totalBonusUnit = (Convert.ToInt32(Math.Floor(age.TotalMinutes)) > 60) ? (Convert.ToInt32(Math.Floor(age.TotalHours)) - (totalUnit * 60)) : 0;
                bonusWord = "minute";
            }
            else if (age.TotalDays > 1 && age.TotalDays < 365)
            {
                totalUnit = Convert.ToInt32(Math.Floor(age.TotalDays));
                word = "day";

                // Only show this if it's less than 7 days old (168 hours)
                totalBonusUnit = (Convert.ToInt32(Math.Floor(age.TotalHours)) > 24 && Convert.ToInt32(Math.Floor(age.TotalHours)) < 168) ? (Convert.ToInt32(Math.Floor(age.TotalHours)) - (totalUnit * 24)) : 0;
                bonusWord = "hour";
            }
            else if (age.TotalDays > 365)
            {
                totalUnit = Convert.ToInt32(Math.Floor(age.TotalDays / 365));
                word = "year";

                totalBonusUnit = (Convert.ToInt32(Math.Floor(age.TotalDays)) > 365) ? (Convert.ToInt32(Math.Floor(age.TotalDays)) - (totalUnit * 365)) : 0;
                bonusWord = "day";
            }

            string contextualWord = totalUnit > 1 ? StringFunctions.PluralizeWord(word) : word;
            string bonusStr = string.Empty;
            if (totalBonusUnit > 0)
            {
                bonusStr = string.Format(", {0} {1}", totalBonusUnit,
                                         totalBonusUnit > 1 ? StringFunctions.PluralizeWord(bonusWord) : bonusWord);
            }

            return string.Format(AGE_FORMAT, totalUnit, contextualWord, bonusStr);
        }

        public static string PluralizeWord(string word)
        {
            return s_pluralizationService.Pluralize(word);
        }
    }
}

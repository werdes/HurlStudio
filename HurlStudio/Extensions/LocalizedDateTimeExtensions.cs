using HurlStudio.UI.Localization;
using MsBox.Avalonia.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HurlStudio.Extensions
{
    public static class LocalizedDateTimeExtensions
    {
        /// <summary>
        /// see https://stackoverflow.com/questions/11/calculate-relative-time-in-c-sharp
        /// </summary>
        private static readonly SortedList<double, Func<TimeSpan, string>> _offsets = new SortedList<double, Func<TimeSpan, string>>
            {
                { 0.75, _ => Localization.Common_DateTime_LessThanAMinute},
                { 1.5, _ => Localization.Common_DateTime_AboutAMinute},
                { 45, x => $"{x.TotalMinutes:F0} {Localization.Common_DateTime_Minutes}"},
                { 90, x => Localization.Common_DateTime_AboutAnHour},
                { 1440, x => $"{Localization.Common_DateTime_About} {x.TotalHours:F0} {Localization.Common_DateTime_Hours}"},
                { 2880, x => Localization.Common_DateTime_ADay},
                { 43200, x => $"{x.TotalDays:F0} {Localization.Common_DateTime_Days}"},
                { 86400, x => Localization.Common_DateTime_AboutAMonth},
                { 525600, x => $"{x.TotalDays / 30:F0} {Localization.Common_DateTime_Months}"},
                { 1051200, x => Localization.Common_DateTime_AboutAYear},
                { double.MaxValue, x => $"{x.TotalDays / 365:F0} {Localization.Common_DateTime_Years}"}
            };

        /// <summary>
        /// Returns a localized string describing a timespan between the given date and now
        /// ( ex. "a year ago")
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToLocale(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now - dateTime;
            return TimeSpanToLocale(timeSpan);
        }

        /// <summary>
        /// Returns a localized string describing a timespan between the given date and UTC now
        /// ( ex. "a year ago")
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string UtcToLocale(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.UtcNow - dateTime;
            return TimeSpanToLocale(timeSpan);
        }

        /// <summary>
        /// Returns a localized string describing a timespan
        /// ( ex. "a year ago")
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        private static string TimeSpanToLocale(TimeSpan timeSpan)
        {
            string suffix = timeSpan.TotalMinutes > 0 ? Localization.Common_DateTime_Suffix_Ago : Localization.Common_DateTime_Suffix_FromNow;
            string prefix = timeSpan.TotalMinutes > 0 ? Localization.Common_DateTime_Prefix_Ago : Localization.Common_DateTime_Prefix_FromNow;
            timeSpan = new TimeSpan(Math.Abs(timeSpan.Ticks));
            return prefix + _offsets.First(n => timeSpan.TotalMinutes < n.Key).Value(timeSpan) + suffix;
        }
    }
}

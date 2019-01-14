using System;
using System.Collections.Generic;
using System.Globalization;

namespace ShiftServer.Proto.Helper
{
    public static class DateTimeHelper
    {
        private static List<string> PARSE_FORMATS = new List<string>()
                {
                "dd.MM.yyyy HH:mm",
                "dd.MM.yyyy HH:mm:ss",
                "HH:mm:ss",
                "dd.MM.yyyy",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                "yyyy'-'MM'-'dd HH':'mm':'ss",
                "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffzzz", //2014-04-30T13:49:04.863+03:00
                "MM/dd/yyyy HH:mm",
                "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy",
                "MM/dd/yyyy HH:mm [zzz]",
                "yyyy-MM-dd",
                "yyyy-MM-dd'T'HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd'T'HH:mm:sszzz",
                "yyyy-MM-dd'T'HH:mm:ss.ff'Z'",
                "dd-MMM-yyyy", //"2-Mar-2017",
                "d-MMM-yyyy"
            };

        public static string ToString_UTC_MMddyyyyHHmmss(this DateTime? dt)
        {
            if (dt == null)
                return "-";
            var dt2 = dt.Value.ToUniversalTime();
            return ToString_MMddyyyyHHmmss(dt2);
        }

        public static string ToString_MMddyyyyHHmmss(this DateTime? dt)
        {
            if (dt == null)
              return "-";

            return ToString_MMddyyyyHHmmss(dt.Value);
        }

        public static string ToString_MMddyyyyHHmmss(this DateTime dt)
        {
            return string.Format("{0:MM'/'dd'/'yyyy HH:mm:ss}", dt);
        }

        public static string ToString_MMddyyyy(this DateTime? dt)
        {
            if (dt == null)
              return "-";

            return ToString_MMddyyyy(dt.Value);
        }

        public static string ToString_MMddyyyy(this DateTime dt)
        {
            return string.Format("{0:MM'/'dd'/'yyyy}", dt);
        }

        public static string ToString_yyyy_MM_dd(this DateTime dt)
        {
            return string.Format("{0:yyyy-MM-dd}", dt);
        }

        public static string ToString_MMM_dd_yyyy(this DateTime dt)
        {
            return string.Format("{0:MMM dd, yyyy}", dt);
        }

        public static string ToString_yyyyddMMHHmmssfffff(this DateTime dt)
        {
            return string.Format("{0:yyyyMMddHHmmssfffff}", dt);
        }

        public static string ToString_yyyyddMMHHmmss(this DateTime dt)
        {
            return string.Format("{0:yyyyMMddHHmmss}", dt);
        }
        public static string ToString_yyyyddMMHHmmss_Fancy(this DateTime dt)
        {
            return string.Format("{0:yyyyMMdd_HHmm}", dt);
        }

        public static string ToString_Monthddyyyy(this DateTime? dt)
        {
            if (dt == null)
              return "-";

            return ToString_Monthddyyyy(dt.Value, new CultureInfo("en-US"));
        }

        public static string ToString_Monthddyyyy(this DateTime dt, CultureInfo culture = null)
        {
            return culture != null
                ? string.Format(culture, "{0:MMMM dd, yyyy}", dt)
                : string.Format(new CultureInfo("en-US"), "{0:MMMM dd, yyyy}", dt);
        }

        public static bool IsOlderThanNDays(this DateTime dt, int days)
        {
            return dt.AddDays(days) < DateTime.UtcNow;
        }
		public static bool IsOlderThanNMins(this DateTime dt, int mins)
		{
			return dt.AddMinutes(mins) < DateTime.UtcNow;
		}
		public static DateTime? TryParseWithNullable(string inDate)
        {
            if (String.IsNullOrEmpty(inDate))
              return null;

            DateTime? expireDate = null;
            inDate = inDate.Trim();

            foreach (string formatt in PARSE_FORMATS)
            {
                try
                {
                  expireDate = DateTime.ParseExact(inDate, formatt, null);
                }
                catch (Exception e)
                {
                  string happyCompiler = e.Message;
                }

                if (expireDate != null)
                  break;
            }

          return expireDate;
        }

        public static DateTime TryParseWithDefault(string inDate, DateTime defaultDate)
        {
            if (String.IsNullOrEmpty(inDate))
              return defaultDate;

            DateTime resultDate = DateTime.MinValue;
            inDate = inDate.Trim();
            if (resultDate == DateTime.MinValue)
            {
                foreach (string formatt in PARSE_FORMATS)
                {
                    try
                    {
                      resultDate = DateTime.ParseExact(inDate, formatt, null);
                    }
                    catch (Exception e)
                    {
                      string happyCompiler = e.Message;
                    }

                    if (resultDate != DateTime.MinValue)
                      break;
                }
            }

            if (resultDate == DateTime.MinValue)
            {
                resultDate = defaultDate;
            }

            return resultDate;
        }

        public static string ToRelativeTime(this DateTime? dt)
        {
            if (dt == null)
              return "-";

            return ToRelativeTime(dt.Value);
        }

        public static string ToRelativeTime(this DateTime dt)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
              return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * minute)
              return "a minute ago";

            if (delta < 45 * minute)
              return ts.Minutes + " minutes ago";

            if (delta < 90 * minute)
              return "an hour ago";

            if (delta < 24 * hour)
              return ts.Hours + " hours ago";

            if (delta < 48 * hour)
              return "yesterday";

            if (delta < 30 * day)
              return ts.Days + " days ago";

            if (delta < 12 * month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        public static string ToRelativeEllapsedTime(this DateTime dt)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
                return ts.Seconds == 1 ? "one second" : ts.Seconds + " seconds";

            if (delta < 2 * minute)
                return ts.Seconds + " seconds";

            if (delta < 45 * minute)
                return ts.Minutes + " minutes";

            if (delta < 24 * hour)
                return ts.Hours + " hours";

            return ts.Hours + " hours";
        }
        public static string ToRelativeEllapsedTime(this TimeSpan dt)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = dt;
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
                return ts.Seconds == 1 ? "one second" : ts.Seconds + " s";

            if (delta < 2 * minute)
                return ts.Seconds + " s";

            if (delta < 45 * minute)
                return ts.Minutes + " min";

            if (delta < 24 * hour)
                return ts.Hours + " hour";

            if (delta > 24 * hour)
            {
                return ts.Days + " day "+ ts.Hours + " hour";

            }
            return ts.Hours + " hour";
        }
    }
}

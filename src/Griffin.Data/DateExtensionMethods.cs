using System;

namespace Griffin.Data
{
    /// <summary>
    /// Helps with SQL dates
    /// </summary>
    public static class DateExtensionMethods
    {
        /// <summary>
        /// SQL Server mindate (for datetime fields)
        /// </summary>
        public static readonly DateTime SqlServerDate = new DateTime(1753, 1, 1);


        /// <summary>
        /// Converts from SqlServer min date to <c>DateTime.MinValue</c> if required.
        /// </summary>
        /// <param name="dateTime">Date/Time in sqlServer</param>
        /// <returns>Proper .NET date</returns>
        public static DateTime FromSqlServer(this DateTime dateTime)
        {
            if (dateTime.Year == 1753 && dateTime.Month == 1 && dateTime.Day == 1)
                return DateTime.MinValue;

            return dateTime;
        }

        /// <summary>
        /// Converts from SqlServer min date to <c>DateTime.MinValue</c> if required.
        /// </summary>
        /// <param name="dateTime">Date/Time in sqlServer</param>
        /// <returns>Proper .NET date</returns>
        public static DateTime ToSqlServer(this DateTime dateTime)
        {
            return dateTime == DateTime.MinValue ? new DateTime(1753, 1, 1) : dateTime;
        }
    }
}

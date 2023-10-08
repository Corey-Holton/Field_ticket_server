namespace CA.Ticketing.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsWithinMonth(DateTime? date)
        {
            if (date == null)
            {
                return false;
            }

            var birthday = new DateTime(DateTime.Now.Year, date.Value.Month, date.Value.Day);
            var difference = birthday - DateTime.Now;
            if (difference.TotalDays <= 30 && difference.TotalDays >= 0)
            {
                return true;
            }

            return false;
        }

        public static string ToUSDateTime(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }
    }
}

namespace ComChienMaDui.Helpers
{
    public class Format
    {
        public static string FormatPrice(decimal price)
        {
            return price.ToString("N0", new System.Globalization.CultureInfo("vi-VN"));
        }
    }
}

namespace LockToyApp.Helpers
{
    static class DataHelper
    {
        public static byte[] ToByteArray(this string input)
        {
            return System.Text.Encoding.ASCII.GetBytes(input);
        }
    }
}

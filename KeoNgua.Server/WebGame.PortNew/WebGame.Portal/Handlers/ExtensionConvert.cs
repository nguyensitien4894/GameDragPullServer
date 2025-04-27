namespace MsWebGame.Portal.Handlers
{
    public static class ExtensionConvert
    {
        public static string IntToRankFormat(this int inputValue)
        {
            string val = string.Empty;
            if (inputValue == 1)
                val = "Diamond";
            else if (inputValue == 2)
                val = "Gold";
            else if (inputValue == 3)
                val = "Silver";
            else if (inputValue == 4)
                val = "Copper";
            else if (inputValue == 5)
                val = "Stone";
            else
                val = inputValue.ToString();

            return val;
        }
    }
}
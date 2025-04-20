namespace GrabDemSite.Constants
{
    public static class ConstantsVars
    {
        public const string Wallet = "randomWallet";
        public const string FakeWallet = "randomFakeWallet";
        public const string adminName = "Test1";
        public static readonly HashSet<string> listOfNamesToAvoid = new() { "SkAg1", "BlAg2", "5aAg3", "TyAg4", "66Ag5", "SpecAg" };
        public readonly static char[] alphnum = "1234567890abcdefghijklmnopqrstuvwxyz".ToCharArray();
    }
}

using Microsoft.Identity.Client;

namespace GrabDemSite.Extension_methods
{
    public static class StringExtensionMethods
    {
        public static bool IsNameToAvoidHere(this string name, string[] names)
        {
            foreach(var name1 in names)
            {
                if (name == name1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

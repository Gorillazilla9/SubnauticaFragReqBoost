using System.Reflection;
using Harmony;

namespace FragmentCountChanger
{
    public static class QPatch
    {
        public static void Patch()
        {
            HarmonyInstance.Create("com.ccacic.subnautica.fragmentcountchanger.mod").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}

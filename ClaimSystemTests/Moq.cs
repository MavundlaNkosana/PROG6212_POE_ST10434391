// A simple mock helper to satisfy the test setup for TempData
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Contract_Monthly_Claim_System.Tests
{
    public static class Mock
    {
        public static T Of<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), true)!;
        }
    }
}

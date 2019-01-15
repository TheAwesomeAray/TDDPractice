using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Pluralsight.MaintainableUnitTests.DesignPrinciples.Rules
{
    public class ImplementsIEquatable<T> : ITestRule
    {
        public IEnumerable<string> GetErrorMessages()
        {
            if (!typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
                yield return $"{typeof(T).Name} should implement IEquatable<{typeof(T).Name}>.";
        }

        public IEnumerable<MethodInfo> TryGetTargetMethod()
        {
            MethodInfo method = typeof(T).GetMethod("Equals", new[] { typeof(T) });
            if (method == null || method.GetParameters()[0].ParameterType != typeof(T))
                return Enumerable.Empty<MethodInfo>();
            return new[] { method };
        }
    }
}

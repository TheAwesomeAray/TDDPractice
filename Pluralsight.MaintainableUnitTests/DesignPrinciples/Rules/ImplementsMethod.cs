using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pluralsight.MaintainableUnitTests.DesignPrinciples.Rules
{
    public abstract class ImplementsMethod<T> : ITestRule
    {
        private string MethodName { get; }
        private string MethodLabel { get; }
        private Type[] ArgumentTypes { get; }

        private IEnumerable<MethodInfo> targetMethod;
        private Action DiscoverTargetMethod { get; set; }

        protected ImplementsMethod(string methodName, params Type[] argumentTypes)
            : this(methodName, methodName, argumentTypes)
        {
        }

        protected ImplementsMethod(string methodName, string methodLabel, params Type[] argumentTypes)
        {
            MethodName = methodName;
            MethodLabel = methodLabel;
            ArgumentTypes = argumentTypes;

            this.DiscoverTargetMethod = () =>
            {
                MethodInfo method = typeof(T).GetMethod(MethodName, ArgumentTypes);
                if (method == null)
                    targetMethod = Enumerable.Empty<MethodInfo>();
                else
                    targetMethod = new[] { method };

                this.DiscoverTargetMethod = () => { };
            };
        }

        public IEnumerable<MethodInfo> TryGetTargetMethod()
        {
            this.DiscoverTargetMethod();
            return this.targetMethod;
        }

        public IEnumerable<string> GetErrorMessages()
        {
            if (this.TryGetTargetMethod().All(method => method.DeclaringType != typeof(T)))
                yield return $"{typeof(T).Name} should {this.OverrideLabel} {this.MethodSignature}).";
        }

        private string OverrideLabel
        {
            get
            {
                if (this.TryGetTargetMethod().Any(m => m.GetBaseDefinition() != null))
                    return "override";
                return "overload";
            }
        }
        private string MethodSignature =>
            $"{this.MethodLabel}({string.Join(", ", this.ArgumentTypes.Select(type => type.Name))}";
    }
}

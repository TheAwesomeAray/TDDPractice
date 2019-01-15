using System;
using System.Collections.Generic;
using System.Reflection;
namespace Pluralsight.MaintainableUnitTests.DesignPrinciples.Rules
{
    class GetHashCodeEqualReturns<T> : ITestRule
    {
        private MethodInfo Method { get; }
        private T TargetInstance { get; }
        private T OtherObject { get; }

        public GetHashCodeEqualReturns(MethodInfo method, T obj1, T obj2)
        {
            this.Method = method;
            this.TargetInstance = obj1;
            this.OtherObject = obj2;
        }

        public IEnumerable<string> GetErrorMessages()
        {
            IList<string> errors = new List<string>();

            try
            {
                var hash1 = (int)Method.Invoke(TargetInstance, new object[0]);
                var hash2 = (int)Method.Invoke(OtherObject, new object[0]);

                if (hash1 != hash2)
                    errors.Add($"{Method.GetSignature()} returned distinct values on equal objects.");
            }
            catch (TargetInvocationException invocationExc)
            {
                errors.Add($"{Method.GetSignature()} failed - {invocationExc.Message}.");
            }
            catch (Exception exc)
            {
                errors.Add($"{Method.GetSignature()} failed - {exc.Message}.");
            }

            return errors;
        }
    }
}


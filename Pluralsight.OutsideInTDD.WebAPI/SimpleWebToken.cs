using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Pluralsight.OutsideInTDD.WebAPI
{
    public class SimpleWebToken : IEnumerable<Claim>
    {
        public IEnumerable<Claim> claims;

        public SimpleWebToken(params Claim[] claims)
        {
            this.claims = claims;
        }

        public override string ToString()
        {
            return claims.Select(c => c.Type + "=" + c.Value)
                         .DefaultIfEmpty(string.Empty)
                         .Aggregate((x,y) => x + "&" + y);
        }

        public IEnumerator<Claim> GetEnumerator()
        {
            return claims.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static bool TryParse(string tokenString, out SimpleWebToken token)
        {
            token = null;

            if (tokenString == string.Empty)
            {
                token = new SimpleWebToken();
                return true;
            }

            if (tokenString == null)
                return false;

            var claimPairs = tokenString.Split("&").ToArray();
            if (!claimPairs.All(x => x.Contains("=")))
                return false;

            var claims = claimPairs
                .Select(s => s.Split("=".ToArray()))
                .Select(s => new Claim(s[0], s[1])).ToArray();

            token = new SimpleWebToken(claims);
            return true;
        }
    }
}

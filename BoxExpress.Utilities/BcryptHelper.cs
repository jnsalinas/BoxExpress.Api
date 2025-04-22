using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Utilities
{
    public static class BcryptHelper
    {
        public static string Hash(string plain) =>
            BCrypt.Net.BCrypt.HashPassword(plain, workFactor: 12);

        public static bool Verify(string plain, string hash) =>
            BCrypt.Net.BCrypt.Verify(plain, hash);
    }
}

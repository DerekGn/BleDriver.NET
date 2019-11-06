using System;
using System.Linq;

using BgApiDriver;

namespace BgApiApp
{
    internal static class BdAddrExtensions
    {
        public static string GetValue(this bd_addr addr)
        {
            return BitConverter.ToString(addr.Address.ToArray().Reverse().ToArray()).Replace('-', ':');
        }
    }
}

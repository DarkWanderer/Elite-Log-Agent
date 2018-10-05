using DW.ELA.Utility.Crypto;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests.Utility
{
    public class CryptoTests
    {
        [Test]
        public void Sha1Test()
        {
            var str = "The quick brown fox jumps over the lazy dog";
            var hash = "2fd4e1c67a2d28fced849ee1bb76e7391b93eb12";
            StringAssert.AreEqualIgnoringCase(hash, Hash.Sha1(str));
        }
    }
}

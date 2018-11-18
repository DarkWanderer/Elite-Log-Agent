using Controller;
using DW.ELA.UnitTests.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DW.ELA.UnitTests
{
    public partial class LogEnumeratorTests
    {
        [Test]
        public void ShouldPlayEvents()
        {
            var directoryProvider = new TestDirectoryProvider();
            Directory.Delete(directoryProvider.Directory, true);

            string testFile1 = Path.Combine(directoryProvider.Directory, "Journal.1234.log");
            string testFile2 = Path.Combine(directoryProvider.Directory, "JournalBeta.2345.log");

            File.WriteAllText(testFile1, "asd");
            File.WriteAllText(testFile2, "asd");

            var files = LogEnumerator.GetLogFiles(directoryProvider.Directory);
            CollectionAssert.Contains(files, testFile1);
            CollectionAssert.DoesNotContain(files, testFile2);
        }
    }
}

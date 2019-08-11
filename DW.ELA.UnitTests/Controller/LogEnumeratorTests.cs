namespace DW.ELA.UnitTests.Controller
{
    using System.IO;
    using DW.ELA.Controller;
    using DW.ELA.UnitTests.Utility;
    using NUnit.Framework;

    public partial class LogEnumeratorTests
    {
        [Test]
        public void ShouldPlayEvents()
        {
            var directoryProvider = new TestDirectoryProvider();

            string testFile1 = Path.Combine(directoryProvider.Directory, "Journal.1234.log");
            string testFile2 = Path.Combine(directoryProvider.Directory, "JournalBeta.2345.log");

            File.WriteAllText(testFile1, "asd");
            File.WriteAllText(testFile2, "asd");

            string[] files = JournalFileEnumerator.GetLogFiles(directoryProvider.Directory);
            CollectionAssert.Contains(files, testFile1);
            CollectionAssert.DoesNotContain(files, testFile2);
        }
    }
}

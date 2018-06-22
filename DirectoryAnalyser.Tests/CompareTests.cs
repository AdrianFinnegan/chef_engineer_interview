using System.Linq;
using System.IO;
using NUnit.Framework;
using System.Reflection;

namespace DirectoryAnalyser.Tests
{
    [TestFixture]
    public class CompareTests
    {
        private string _executingFolder;

        private const string COMMON_OUTPUT_NAME = "./common";
        private const string ONLY_IN_A_OUTPUT_NAME = "./a_only";
        private const string ONLY_IN_B_OUTPUT_NAME = "./b_only";

        [SetUp]
        public void Setup()
        {
            File.Delete(COMMON_OUTPUT_NAME);
            File.Delete(ONLY_IN_A_OUTPUT_NAME);
            File.Delete(ONLY_IN_B_OUTPUT_NAME);

            _executingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            File.Delete(_executingFolder + "\\TestData\\Directory4\\EmptyFile.txt");
        }

        [TestCase]
        public void CompareDirectories_Same_Common_Output()
        {
            //Arrange
            var source = _executingFolder + "\\TestData\\Directory1";
            var destination = _executingFolder + "\\TestData\\Directory2";

            //Act
            Compare.CompareDirectories(source, destination);

            //Assert
            CheckOutputFilesCreated();

            //Check all files have been written to common
            foreach (var testFile in Directory.GetFiles(source))
            {
                var commonFile = File.ReadAllText(COMMON_OUTPUT_NAME);
                Assert.AreEqual(true, commonFile.Contains(testFile));
            }

            Assert.AreEqual(0, File.ReadAllLines(ONLY_IN_A_OUTPUT_NAME).Count());
            Assert.AreEqual(0, File.ReadAllLines(ONLY_IN_B_OUTPUT_NAME).Count());
        }

        /// <summary>
        /// Same filenames in A  + B
        /// Original File in B has different content
        /// </summary>
        [TestCase]
        public void CompareDirectories_Folder_3_Same_File_Different_Content()
        {
            //Arrange
            var source = _executingFolder + "\\TestData\\Directory2";
            var destination = _executingFolder + "\\TestData\\Directory3";

            //Act
            Compare.CompareDirectories(source, destination);

            //Assert
            CheckOutputFilesCreated();

            //Should be one less in common
            Assert.AreEqual(false, File.ReadAllLines(COMMON_OUTPUT_NAME).Contains("Directory3\\Subfolder\\New Text Document.txt"));
            Assert.AreEqual(0, File.ReadAllLines(ONLY_IN_A_OUTPUT_NAME).Count());


            Assert.AreEqual(1, File.ReadAllLines(ONLY_IN_B_OUTPUT_NAME).Count());

            var bContent = File.ReadAllLines(ONLY_IN_B_OUTPUT_NAME);
            Assert.AreEqual(1, bContent.Count());

            Assert.IsTrue(bContent.First().Contains(@"Directory3\Subfolder\New Text Document.txt"));
        }

        [TestCase]
        public void CompareDirectories_Both_Destinations_Empty_Should_Produce_Empty_Reports()
        {
            //Arrange
            var source = _executingFolder + "\\TestData\\Directory4";
            var destination = _executingFolder + "\\TestData\\Directory4";

            //Act
            Compare.CompareDirectories(source, destination);

            //Assert
            CheckOutputFilesCreated();
        }

        [TestCase("\\TestData\\Directory4", null)]
        [TestCase(null, "\\TestData\\Directory4")]
        public void CompareDirectories_Parameter_Missing(string source, string destination)
        {
            //Act
            Compare.Main(new[] { source, destination });

            //Assert
            CheckOutputFilesNotCreated();
        }

        #region Helpers

        private void CheckOutputFilesCreated()
        {
            FileAssert.Exists(COMMON_OUTPUT_NAME);
            FileAssert.Exists(ONLY_IN_A_OUTPUT_NAME);
            FileAssert.Exists(ONLY_IN_B_OUTPUT_NAME);
        }

        private void CheckOutputFilesNotCreated()
        {
            FileAssert.DoesNotExist(COMMON_OUTPUT_NAME);
            FileAssert.DoesNotExist(ONLY_IN_A_OUTPUT_NAME);
            FileAssert.DoesNotExist(ONLY_IN_B_OUTPUT_NAME);
        }

        #endregion
    }
}

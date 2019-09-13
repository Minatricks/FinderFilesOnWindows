using System;
using System.IO;
using FileFinder.Managers;
using FluentAssertions;
using Xunit;

namespace FileFinder.Tests
{
    public class FinderTest
    {
        private Finder _finder;

        public FinderTest()
        {
            _finder = new Finder(new DirectoryManager(), new DriverManager());
        }

        [Fact]
        public void OrganizeWork_CountShouldBeEqualToCountInFile()
        {
            //Arrange 
            var pathToFile = $"C:\\Users\\{Environment.UserName}\\Folders.txt";
            File.Delete(pathToFile);

            //Act
            _finder.OrganizeWork(pathToFile);

            //Assert
            _finder.Count.Should().Be(File.ReadAllLines(pathToFile).Length);
        }
    }
}

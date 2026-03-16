using FluentAssertions;

namespace CsvToYealink.Tests.Unit
{
    [TestFixture]
    public class CsvParserTests
    {
        private ICsvParser _parser;

        [SetUp]
        public void Setup()
        {
            // Initialize the service before each test
            _parser = new CsvParser();
        }

        [Test]
        [Category("Unit")]
        public void ParseLine_WithValidCsv_ShouldReturnCorrectContact()
        {
            // Arrange
            string csvLine = "John,Doe,123456";

            // Act
            var result = _parser.ParseLine(csvLine);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.PhoneNumber.Should().Be("123456");
        }

        [TestCase("", Description = "Empty string")]
        [TestCase("InvalidData", Description = "Malformed CSV")]
        public void ParseLine_WithInvalidData_ShouldThrowArgumentException(string invalidInput)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _parser.ParseLine(invalidInput));
        }
    }
}
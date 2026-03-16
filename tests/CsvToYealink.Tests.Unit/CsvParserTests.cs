namespace CsvToYealink.Tests.Unit;

using CsvToYealink.Core.Entities;
using CsvToYealink.Core.Services;

using Xunit;

public class CsvServiceTests
{
    [Fact]
    public void ReadRecords_ShouldReturnValidData_WhenFileExists()
    {
        // Arrange
        var service = new CsvService(); // Constructor without logger for now
        var filePath = "test_data.csv";
        var csvContent = "key,label,line,type,value\r\n1,Segreteria,1,16,402\r\n2,SOUP Postazione 1,,2,0012035488";

        File.WriteAllText(filePath, csvContent);

        try
        {
            // Act
            var results = service.ReadRecords<Terminal>(filePath).ToList();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
            Assert.Equal("Segreteria", results[0].Label);
            Assert.Equal(2, results[1].Key);
        }
        finally
        {
            // Clean up: delete the file after the test
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
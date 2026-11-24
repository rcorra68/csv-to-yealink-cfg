namespace CsvToYealink.Tests.Unit;

using ClosedXML.Excel;

using CsvToYealink.Core.Entities;
using CsvToYealink.Core.Services;

using Xunit;

public class ExcelServiceTests
{
    [Fact]
    public void ReadTerminals_ShouldReadBothSides_AndResetEmptyKeys()
    {
        // Arrange: same layout as Schema_Estensione.xlsx
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.AddWorksheet("Foglio1");
            ws.Cell("A1").Value = 1;
            ws.Cell("B1").Value = "Segreteria";
            ws.Cell("C1").Value = "16";
            ws.Cell("D1").Value = "402";
            ws.Cell("F1").Value = "Centro TLC";
            ws.Cell("G1").Value = "13";
            ws.Cell("H1").Value = "0765 123-456";
            ws.Cell("I1").Value = 2;
            ws.Cell("A2").Value = 3; // empty slot
            ws.Cell("I2").Value = 4; // empty slot
            ws.Cell("A3").Value = "PAGINA 1";
            wb.SaveAs(filePath);
        }

        try
        {
            // Act
            var results = new ExcelService().ReadTerminals(filePath).ToList();

            // Assert: 2 configured keys + 2 reset keys, separator row ignored
            Assert.Equal(4, results.Count);

            Assert.Equal(1, results[0].Key);
            Assert.Equal(ExtensionType.BLF, results[0].ExtensionType);
            Assert.Equal(1, results[0].Line);

            Assert.Equal(2, results[1].Key);
            Assert.Equal("Centro TLC", results[1].Label);
            Assert.Equal(ExtensionType.SpeedDial, results[1].ExtensionType);
            Assert.Equal(1, results[1].Line);
            Assert.Equal("00765123456", results[1].ExtensionValue); // default "0" prefix
            Assert.Equal("402", results[0].ExtensionValue);          // BLF: no prefix

            Assert.Equal(3, results[2].Key);
            Assert.Equal(ExtensionType.None, results[2].ExtensionType);
            Assert.Equal(4, results[3].Key);
            Assert.Equal(ExtensionType.None, results[3].ExtensionType);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadTerminals_ShouldResetAndWarn_WhenLabelHasNoNumber()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.AddWorksheet("Foglio1");
            ws.Cell("A1").Value = 1;
            ws.Cell("B1").Value = "Segreteria";
            ws.Cell("C1").Value = "16";
            wb.SaveAs(filePath);
        }

        try
        {
            var service = new ExcelService();
            var result = Assert.Single(service.ReadTerminals(filePath));

            Assert.Equal(ExtensionType.None, result.ExtensionType);
            Assert.Contains("D1", Assert.Single(service.Warnings));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadTerminals_ShouldThrow_WhenTypeIsInvalid()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.AddWorksheet("Foglio1");
            ws.Cell("A1").Value = 1;
            ws.Cell("B1").Value = "Segreteria";
            ws.Cell("C1").Value = "99";
            ws.Cell("D1").Value = "402";
            wb.SaveAs(filePath);
        }

        try
        {
            var ex = Assert.Throws<InvalidDataException>(
                () => new ExcelService().ReadTerminals(filePath).ToList());
            Assert.Contains("C1", ex.Message);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadTerminals_ShouldNotPrefix_WhenDialPrefixIsEmpty()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.AddWorksheet("Foglio1");
            ws.Cell("A1").Value = 1;
            ws.Cell("B1").Value = "Centro TLC";
            ws.Cell("C1").Value = "2";
            ws.Cell("D1").Value = "0765123456";
            wb.SaveAs(filePath);
        }

        try
        {
            var result = Assert.Single(new ExcelService(dialPrefix: "").ReadTerminals(filePath));
            Assert.Equal("0765123456", result.ExtensionValue);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadTerminals_ShouldNotPrefix_PbxShortcuts()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.AddWorksheet("Foglio1");
            ws.Cell("A1").Value = 1;
            ws.Cell("B1").Value = "Autista Direttore";
            ws.Cell("C1").Value = "13";
            ws.Cell("D1").Value = "*5 234";
            wb.SaveAs(filePath);
        }

        try
        {
            var result = Assert.Single(new ExcelService().ReadTerminals(filePath));
            Assert.Equal("*5234", result.ExtensionValue);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Theory]
    [InlineData("400", 4, "400")]          // queue: internal, no prefix
    [InlineData("4001", 4, "4001")]        // 4-digit extension: still internal
    [InlineData("40012", 4, "040012")]     // longer: external, prefixed
    [InlineData("0765 123456", 4, "00765123456")]
    [InlineData("4001", 3, "04001")]       // custom limit
    public void ReadTerminals_ShouldPrefixOnlyExternalNumbers(string number, int internalMaxLength, string expected)
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xlsx");
        using (var wb = new XLWorkbook())
        {
            var ws = wb.AddWorksheet("Foglio1");
            ws.Cell("A1").Value = 1;
            ws.Cell("B1").Value = "Coda";
            ws.Cell("C1").Value = "13";
            ws.Cell("D1").Value = number;
            wb.SaveAs(filePath);
        }

        try
        {
            var result = Assert.Single(
                new ExcelService(dialPrefix: "0", internalMaxLength: internalMaxLength).ReadTerminals(filePath));
            Assert.Equal(expected, result.ExtensionValue);
        }
        finally
        {
            File.Delete(filePath);
        }
    }
}

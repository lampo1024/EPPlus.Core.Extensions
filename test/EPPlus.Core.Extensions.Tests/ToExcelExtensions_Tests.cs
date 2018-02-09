﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;

using FluentAssertions;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Xunit;

namespace EPPlus.Core.Extensions.Tests
{
    public class ToExcelExtensions_Tests : TestBase
    {
        private readonly List<Person> _personList;

        public ToExcelExtensions_Tests()
        {
            _personList = new List<Person>
                          {
                              new Person { FirstName = "Daniel", LastName = "Day-Lewis", YearBorn = 1957 },
                              new Person { FirstName = "Sally", LastName = "Field", YearBorn = 1946 },
                              new Person { FirstName = "David", LastName = "Strathairn", YearBorn = 1949 },
                              new Person { FirstName = "Joseph", LastName = "Gordon-Levitt", YearBorn = 1981 },
                              new Person { FirstName = "James", LastName = "Spader", YearBorn = 1960 }
                          };
        }

        [Fact]
        public void Columns_should_be_autogenerated_if_no_columns_are_specified()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList.ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Columns.Should().Be(2);
        }

        [Fact]
        public void Rowcount_should_match_listcount_plus_header_if_not_title()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList.ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(_personList.Count + 1);
        }

        [Fact]
        public void Rowcount_should_match_listcount_without_header()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList.ToExcelPackage(false);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(_personList.Count);
        }

        [Fact]
        public void Rowcount_should_match_listcount_with_header()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList.ToExcelPackage(true);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(_personList.Count + 1);
        }

        [Fact]
        public void Header_texts_should_be_same_as_defined_on_ExcelTableColumn_attribute()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList.ToExcelPackage(true);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Cells[1, 1, 1, 1].Value.Should().Be("Last Name");
            package.Workbook.Worksheets[1].Cells[1, 2, 1, 2].Value.Should().Be("Year of Birth");
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(_personList.Count + 1);
        }

        [Fact]
        public void Rowcount_should_match_listcount_plus_header_plus_one_title()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList
                      .ToWorksheet("Actors")
                      .WithTitle("Actors")
                      .ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(_personList.Count + 2);
        }

        [Fact]
        public void Rowcount_should_match_listcount_plus_header_plus_two_titles()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList
                      .ToWorksheet("Actors")
                      .WithTitle("Actors")
                      .WithTitle("In the movie Lincoln")
                      .ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(_personList.Count + 3);
        }

        [Fact]
        public void Worksheet_should_match_specified_string_columns()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList
                      .ToWorksheet("Actors")
                      .WithColumn(x => x.LastName, "Last Name")
                      .ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Columns.Should().Be(1);
            for (var i = 0; i < _personList.Count; i++)
            {
                package.Workbook.Worksheets[1].Cells[i + 2, 1].Value.ToString().Should().Be(_personList[i].LastName);
            }
        }

        [Fact]
        public void Worksheet_should_match_specified_int_columns()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            package = _personList
                      .ToWorksheet("Actors")
                      .WithColumn(x => x.YearBorn, "Year Born")
                      .ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets[1].Dimension.Columns.Should().Be(1);

            for (var i = 0; i < _personList.Count; i++)
            {
                package.Workbook.Worksheets[1].Cells[i + 2, 1].Value.Should().Be(_personList[i].YearBorn);
            }
        }

        [Fact]
        public void Multiple_lists_should_create_multiple_worksheets()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            List<Person> pre50 = _personList.Where(x => x.YearBorn < 1950).ToList();
            List<Person> post50 = _personList.Where(x => x.YearBorn >= 1950).ToList();

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package = pre50
                                   .ToWorksheet("< 1950")
                                   .WithColumn(x => x.FirstName, "First Name")
                                   .WithColumn(x => x.LastName, "Last Name")
                                   .WithColumn(x => x.YearBorn, "Year of Birth")
                                   .WithTitle("< 1950")
                                   .NextWorksheet(post50, "> 1950")
                                   .WithColumn(x => x.LastName, "Last Name")
                                   .WithColumn(x => x.YearBorn, "Year of Birth")
                                   .WithTitle("> 1950")
                                   .ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets.Count.Should().Be(2);
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(pre50.Count + 2);
            package.Workbook.Worksheets[1].Dimension.Columns.Should().Be(3);
            package.Workbook.Worksheets[2].Dimension.Rows.Should().Be(post50.Count + 2);
            package.Workbook.Worksheets[2].Dimension.Columns.Should().Be(2);
        }

        [Fact]
        public void Multiple_lists_should_create_multiple_worksheets_without_header()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            List<Person> pre50 = _personList.Where(x => x.YearBorn < 1950).ToList();
            List<Person> post50 = _personList.Where(x => x.YearBorn >= 1950).ToList();

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package = pre50
                                   .ToWorksheet("< 1950", configuration =>
                                   {
                                       configuration.ConfigureColumn = x => { x.SetFontColor(Color.Purple); };

                                       configuration.ConfigureHeader = x =>
                                       {
                                           x.SetFont(new Font("Arial", 13, FontStyle.Bold));
                                           x.SetFontColor(Color.White);
                                           x.SetBackgroundColor(Color.Black);
                                       };

                                       configuration.ConfigureHeaderRow = x =>
                                       {
                                           x.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                           x.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                           x.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                           x.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                           x.Style.Font.Name = "Verdana";
                                       };

                                       configuration.ConfigureCell = (x, y) =>
                                       {
                                           x.SetFont(new Font("Times New Roman", 13));
                                           y.YearBorn = y.YearBorn % 2 == 0 ? y.YearBorn : 1990;
                                       };
                                   })
                                   .WithTitle("< 1950")
                                   .NextWorksheet(post50, "> 1950", configuration =>
                                   {
                                       configuration.ConfigureColumn = x => { x.Style.Font.Color.SetColor(Color.Black); };

                                       configuration.ConfigureHeader = x =>
                                       {
                                           x.Style.Font.Bold = true;
                                           x.Style.Font.Size = 11;
                                           x.Style.Font.Color.SetColor(Color.White);
                                           x.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                           x.Style.Fill.BackgroundColor.SetColor(Color.Black);
                                       };

                                       configuration.ConfigureHeaderRow = x =>
                                       {
                                           x.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                           x.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                           x.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                           x.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                           x.Style.Font.Name = "Verdana";
                                       };

                                       configuration.ConfigureCell = (x, y) =>
                                       {
                                           x.Style.Font.Name = "Times New Roman";
                                           y.YearBorn = y.YearBorn % 2 != 0 ? y.YearBorn : 1990;
                                       };
                                   })
                                   .WithoutHeader()
                                   .WithTitle("> 1950")
                                   .ToExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Workbook.Worksheets.Count.Should().Be(2);
            package.Workbook.Worksheets[1].Dimension.Rows.Should().Be(pre50.Count + 2);
            package.Workbook.Worksheets[1].Dimension.Columns.Should().Be(2);
            package.Workbook.Worksheets[2].Dimension.Rows.Should().Be(post50.Count + 1);
            package.Workbook.Worksheets[2].Dimension.Columns.Should().Be(2);
        }

        [Fact]
        public void Should_convert_list_of_objects_to_byte_array_of_xlsx_file_with_header_row()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            List<Person> pre50 = _personList.Where(x => x.YearBorn < 1950).ToList();
            string tempFile = Path.Combine(Path.GetTempPath(), "persons_pre50.xlsx");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            byte[] result = pre50.ToXlsx(true);
            File.WriteAllBytes(tempFile, result);
            var excelPackage = new ExcelPackage(new FileInfo(tempFile));

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            excelPackage.Workbook.Worksheets.Any().Should().Be(true);
            excelPackage.Workbook.Worksheets.First().Should().NotBe(null);
            excelPackage.Workbook.Worksheets.First().Dimension.Rows.Should().Be(3);
            excelPackage.Workbook.Worksheets.First().Cells[1, 1, 1, 1].Text.Should().Be("Last Name");
            excelPackage.Workbook.Worksheets.First().Cells[1, 2, 1, 2].Text.Should().Be("Year of Birth");
        }

        [Fact]
        public void Should_convert_list_of_objects_to_byte_array_of_xlsx_file_without_header_row()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            List<Person> pre50 = _personList.Where(x => x.YearBorn < 1950).ToList();
            string tempFile = Path.Combine(Path.GetTempPath(), "persons_pre50.xlsx");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            byte[] result = pre50.ToXlsx(false);
            File.WriteAllBytes(tempFile, result);
            var excelPackage = new ExcelPackage(new FileInfo(tempFile));

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            excelPackage.Workbook.Worksheets.Any().Should().Be(true);
            excelPackage.Workbook.Worksheets.First().Should().NotBe(null);
            excelPackage.Workbook.Worksheets.First().Dimension.Rows.Should().Be(2);
            excelPackage.Workbook.Worksheets.First().Cells[1, 1, 1, 1].Text.Should().Be("Field");
            excelPackage.Workbook.Worksheets.First().Cells[1, 2, 1, 2].Text.Should().Be("1946");
        }

        [Fact]
        public void Should_convert_a_worksheet_to_byte_array_of_xlsx_file()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            List<Person> post50 = _personList.Where(x => x.YearBorn > 1950).ToList();
            string tempFile = Path.Combine(Path.GetTempPath(), "persons_post50.xlsx");
            WorksheetWrapper<Person> worksheetWrapper = post50.ToWorksheet("> 1950");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            byte[] result = worksheetWrapper.ToXlsx();
            File.WriteAllBytes(tempFile, result);
            var excelPackage = new ExcelPackage(new FileInfo(tempFile));

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            excelPackage.Workbook.Worksheets.Any().Should().Be(true);
            excelPackage.Workbook.Worksheets.First().Dimension.Rows.Should().Be(4);
        }

        [Fact]
        public void Should_convert_multiple_worksheets_to_byte_array_of_xlsx_file()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            List<Person> pre50 = _personList.Where(x => x.YearBorn < 1950).ToList();
            List<Person> post50 = _personList.Where(x => x.YearBorn >= 1950).ToList();

            WorksheetWrapper<Person> worksheetWrapper = pre50
                                                        .ToWorksheet("< 1950")
                                                        .WithTitle("< 1950")
                                                        .NextWorksheet(post50, "> 1950")
                                                        .WithTitle("> 1950");

            string tempFile = Path.Combine(Path.GetTempPath(), "persons_pre50_post50.xlsx");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            byte[] result = worksheetWrapper.ToXlsx();
            File.WriteAllBytes(tempFile, result);
            var excelPackage = new ExcelPackage(new FileInfo(tempFile));

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            excelPackage.Workbook.Worksheets.Count().Should().Be(2);
            excelPackage.Workbook.Worksheets[1].Dimension.Rows.Should().Be(4);
            excelPackage.Workbook.Worksheets[2].Dimension.Rows.Should().Be(5);
        }

        [Fact]
        public void Should_convert_a_byte_array_into_an_excelPackage()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            byte[] buffer = excelPackage.GetAsByteArray();

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package = buffer.AsExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Should().NotBeNull();
            package.Workbook.Worksheets.Count.Should().Be(excelPackage.Workbook.Worksheets.Count);
            package.GetTables().Count().Should().Be(excelPackage.GetTables().Count());
        }

        [Fact]
        public void Should_not_convert_a_byte_array_into_an_excelPackage_with_empty_password()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            byte[] buffer = excelPackage.GetAsByteArray();

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action action = () =>
            {
                ExcelPackage package = buffer.AsExcelPackage("");
            };

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_convert_a_byte_array_into_an_excelPackage_with_password()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            byte[] buffer = excelPackage.GetAsByteArray("Test1234");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package = buffer.AsExcelPackage("Test1234");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Should().NotBeNull();
            package.Workbook.Worksheets.Count.Should().Be(excelPackage.Workbook.Worksheets.Count);
            package.GetTables().Count().Should().Be(excelPackage.GetTables().Count());
        }

        [Fact]
        public void Should_not_convert_a_byte_array_into_an_excelPackage_with_wrong_password()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            byte[] buffer = excelPackage.GetAsByteArray("Test1234");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action action = () =>
            {
                ExcelPackage package = buffer.AsExcelPackage("test12345");
            };

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            action.Should().Throw<SecurityException>();
        }

        [Fact]
        public void Should_convert_a_stream_into_an_excelPackage()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            Stream stream = new MemoryStream(excelPackage.GetAsByteArray());

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package = stream.AsExcelPackage();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Should().NotBeNull();
            package.Workbook.Worksheets.Count.Should().Be(excelPackage.Workbook.Worksheets.Count);
            package.GetTables().Count().Should().Be(excelPackage.GetTables().Count());
        }

        [Fact]
        public void Should_convert_a_stream_into_an_excelPackage_with_correct_password()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            byte[] buffer = excelPackage.GetAsByteArray("Test1234");
            var stream = new MemoryStream(buffer);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            ExcelPackage package = stream.AsExcelPackage("Test1234");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            package.Should().NotBeNull();
            package.Workbook.Worksheets.Count.Should().Be(excelPackage.Workbook.Worksheets.Count);
            package.GetTables().Count().Should().Be(excelPackage.GetTables().Count());
        }

        [Fact]
        public void Should_not_convert_a_stream_into_an_excelPackage_with_incorrect_password()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            byte[] buffer = excelPackage.GetAsByteArray("Test1234");
            var stream = new MemoryStream(buffer);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action action = () =>
            {
                ExcelPackage package = stream.AsExcelPackage("test1234");
            };

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            action.Should().Throw<SecurityException>();
        }
    }
}

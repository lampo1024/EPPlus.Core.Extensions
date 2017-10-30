﻿using System.Drawing;

using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EPPlus.Core.Extensions
{
    public static class ExcelColumnExtensions
    {
        /// <summary>
        ///     Sets the font of ExcelColumn from a Font object
        /// </summary>
        /// <param name="column"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static ExcelColumn SetFont(this ExcelColumn column, Font font)
        {
            column.Style.Font.SetFromFont(font);
            return column;
        }

        /// <summary>
        ///     Sets the font color of ExcelColumn from a Color object
        /// </summary>
        /// <param name="column"></param>
        /// <param name="fontColor"></param>
        /// <returns></returns>
        public static ExcelColumn SetFontColor(this ExcelColumn column, Color fontColor)
        {
            column.Style.Font.Color.SetColor(fontColor);
            return column;
        }

        /// <summary>
        ///     Sets the background color of ExcelColumn from a Color object
        /// </summary>
        /// <param name="column"></param>
        /// <param name="backgroundColor"></param>
        /// <param name="fillStyle"></param>
        /// <returns></returns>
        public static ExcelColumn SetBackgroundColor(this ExcelColumn column, Color backgroundColor, ExcelFillStyle fillStyle = ExcelFillStyle.Solid)
        {
            column.Style.Fill.PatternType = fillStyle;
            column.Style.Fill.BackgroundColor.SetColor(backgroundColor);
            return column;
        }

        /// <summary>
        ///     Sets the horizontal alignment of ExcelColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="horizontalAlignment"></param>
        /// <returns></returns>
        public static ExcelColumn SetHorizontalAlignment(this ExcelColumn column, ExcelHorizontalAlignment horizontalAlignment)
        {
            column.Style.HorizontalAlignment = horizontalAlignment;
            return column;
        }

        /// <summary>
        ///     Sets the vertical alignment of ExcelColumn
        /// </summary>
        /// <param name="column"></param>
        /// <param name="verticalAlignment"></param>
        /// <returns></returns>
        public static ExcelColumn SetVerticalAlignment(this ExcelColumn column, ExcelVerticalAlignment verticalAlignment)
        {
            column.Style.VerticalAlignment = verticalAlignment;
            return column;
        }
    }
}

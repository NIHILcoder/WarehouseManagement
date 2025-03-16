using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Npgsql.Internal;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Drawing;
using System.Xml.Linq;
using WarehouseManagement.Models;
using Font = iTextSharp.text.Font;

namespace WarehouseManagement.Utils
{
    public static class DataExportHelper
    {
        // Экспорт DataTable в CSV
        public static bool ExportToCSV(DataTable dataTable, string filePath)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                // Заголовки столбцов
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(dataTable.Columns[i].ColumnName);
                    if (i < dataTable.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.AppendLine();

                // Данные
                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        // Экранируем двойными кавычками, если значение содержит запятую
                        string value = row[i].ToString();
                        if (value.Contains(","))
                        {
                            sb.Append("\"" + value + "\"");
                        }
                        else
                        {
                            sb.Append(value);
                        }

                        if (i < dataTable.Columns.Count - 1)
                        {
                            sb.Append(",");
                        }
                    }
                    sb.AppendLine();
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в CSV: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Экспорт DataTable в Excel
        public static bool ExportToExcel(DataTable dataTable, string filePath)
        {
            try
            {
                // Устанавливаем лицензию EPPlus (для некоммерческого использования)
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Отчет");

                    // Заголовки столбцов
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    // Данные
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                        }
                    }

                    // Автоматически подгоняем ширину столбцов
                    worksheet.Cells.AutoFitColumns();

                    // Сохраняем файл
                    package.SaveAs(new FileInfo(filePath));
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в Excel: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Импорт данных из CSV в DataTable
        public static DataTable ImportFromCSV(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                if (lines.Length > 0)
                {
                    // Парсим заголовки
                    string[] headers = lines[0].Split(',');
                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header.Trim());
                    }

                    // Парсим данные
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            // Парсим строку с учетом экранированных кавычками значений
                            List<string> values = new List<string>();
                            StringBuilder sb = new StringBuilder();
                            bool inQuotes = false;

                            foreach (char c in lines[i])
                            {
                                if (c == '\"')
                                {
                                    inQuotes = !inQuotes;
                                }
                                else if (c == ',' && !inQuotes)
                                {
                                    values.Add(sb.ToString());
                                    sb.Clear();
                                }
                                else
                                {
                                    sb.Append(c);
                                }
                            }
                            values.Add(sb.ToString());

                            // Добавляем строку в DataTable
                            if (values.Count > 0)
                            {
                                DataRow row = dataTable.NewRow();
                                for (int j = 0; j < Math.Min(values.Count, headers.Length); j++)
                                {
                                    row[j] = values[j];
                                }
                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при импорте из CSV: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Импорт данных из Excel в DataTable
        public static DataTable ImportFromExcel(string filePath)
        {
            DataTable dataTable = new DataTable();

            try
            {
                // Устанавливаем лицензию EPPlus (для некоммерческого использования)
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Берем первый лист

                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // Создаем столбцы в DataTable
                    for (int col = 1; col <= colCount; col++)
                    {
                        string headerValue = worksheet.Cells[1, col].Value?.ToString() ?? $"Column{col}";
                        dataTable.Columns.Add(headerValue);
                    }

                    // Заполняем DataTable данными, начиная со второй строки
                    for (int row = 2; row <= rowCount; row++)
                    {
                        DataRow dataRow = dataTable.NewRow();

                        for (int col = 1; col <= colCount; col++)
                        {
                            dataRow[col - 1] = worksheet.Cells[row, col].Value;
                        }

                        dataTable.Rows.Add(dataRow);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при импорте из Excel: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Генерация PDF из DataTable
        public static bool GeneratePDF(DataTable dataTable, string filePath, string title)
        {
            try
            {
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                document.Open();

                // Добавляем заголовок
                Paragraph titleParagraph = new Paragraph(title, new Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD));
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                titleParagraph.SpacingAfter = 20f;
                document.Add(titleParagraph);

                // Добавляем дату создания отчета
                Paragraph dateParagraph = new Paragraph($"Дата создания: {DateTime.Now:dd.MM.yyyy HH:mm:ss}",
                    new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL));
                dateParagraph.Alignment = Element.ALIGN_RIGHT;
                dateParagraph.SpacingAfter = 20f;
                document.Add(dateParagraph);

                // Создаем таблицу
                PdfPTable table = new PdfPTable(dataTable.Columns.Count);
                table.WidthPercentage = 100;

                // Заголовки столбцов
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dataTable.Columns[i].ColumnName,
                        new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Padding = 5;
                    table.AddCell(cell);
                }

                // Данные
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dataTable.Rows[i][j].ToString(),
                            new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Padding = 5;
                        table.AddCell(cell);
                    }
                }

                document.Add(table);

                // Добавляем информацию о пользователе
                Paragraph userParagraph = new Paragraph($"Отчет сформирован пользователем: {UserSession.FullName}",
                    new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.NORMAL));
                userParagraph.Alignment = Element.ALIGN_RIGHT;
                userParagraph.SpacingBefore = 20f;
                document.Add(userParagraph);

                document.Close();
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании PDF: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Генерация накладной в PDF
        public static bool GenerateInvoicePDF(DataTable detailsTable, string filePath, string invoiceType,
            string invoiceNumber, string supplierOrCustomer, DateTime date, decimal totalAmount)
        {
            try
            {
                Document document = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                document.Open();

                // Добавляем заголовок
                string title = invoiceType == "Supply" ? "НАКЛАДНАЯ НА ПОСТАВКУ" : "НАКЛАДНАЯ НА ОТГРУЗКУ";
                Paragraph titleParagraph = new Paragraph(title, new Font(Font.FontFamily.TIMES_ROMAN, 16, Font.BOLD));
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                titleParagraph.SpacingAfter = 20f;
                document.Add(titleParagraph);

                // Добавляем информацию о накладной
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 1, 3 });

                infoTable.AddCell(CreateInfoCell("Номер:"));
                infoTable.AddCell(CreateDataCell(invoiceNumber));

                infoTable.AddCell(CreateInfoCell("Дата:"));
                infoTable.AddCell(CreateDataCell(date.ToString("dd.MM.yyyy")));

                string partyLabel = invoiceType == "Supply" ? "Поставщик:" : "Получатель:";
                infoTable.AddCell(CreateInfoCell(partyLabel));
                infoTable.AddCell(CreateDataCell(supplierOrCustomer));

                document.Add(infoTable);

                // Добавляем пустую строку
                document.Add(new Paragraph(" "));

                // Создаем таблицу товаров
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 5, 1, 2, 3 });

                // Заголовки столбцов
                table.AddCell(CreateHeaderCell("№"));
                table.AddCell(CreateHeaderCell("Наименование товара"));
                table.AddCell(CreateHeaderCell("Кол-во"));
                table.AddCell(CreateHeaderCell("Цена, руб."));
                table.AddCell(CreateHeaderCell("Сумма, руб."));

                // Данные
                decimal total = 0;
                for (int i = 0; i < detailsTable.Rows.Count; i++)
                {
                    table.AddCell(CreateCell((i + 1).ToString()));
                    table.AddCell(CreateCell(detailsTable.Rows[i]["ProductName"].ToString()));
                    table.AddCell(CreateCell(detailsTable.Rows[i]["Quantity"].ToString()));

                    decimal unitPrice = Convert.ToDecimal(detailsTable.Rows[i]["UnitPrice"]);
                    table.AddCell(CreateCell(unitPrice.ToString("0.00")));

                    decimal itemTotal = unitPrice * Convert.ToInt32(detailsTable.Rows[i]["Quantity"]);
                    table.AddCell(CreateCell(itemTotal.ToString("0.00")));

                    total += itemTotal;
                }

                // Итоговая строка
                PdfPCell totalLabelCell = new PdfPCell(new Phrase("ИТОГО:", new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
                totalLabelCell.Colspan = 4;
                totalLabelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalLabelCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                totalLabelCell.Padding = 5;
                table.AddCell(totalLabelCell);

                PdfPCell totalValueCell = new PdfPCell(new Phrase(total.ToString("0.00"), new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
                totalValueCell.HorizontalAlignment = Element.ALIGN_CENTER;
                totalValueCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                totalValueCell.Padding = 5;
                table.AddCell(totalValueCell);

                document.Add(table);

                // Добавляем подписи
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));

                PdfPTable signatureTable = new PdfPTable(2);
                signatureTable.WidthPercentage = 100;

                signatureTable.AddCell(CreateSignatureCell("Отпустил / Принял:"));
                signatureTable.AddCell(CreateSignatureCell("Получил / Сдал:"));

                document.Add(signatureTable);

                document.Close();
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании накладной: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Вспомогательные методы для создания ячеек PDF
        private static PdfPCell CreateHeaderCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            return cell;
        }

        private static PdfPCell CreateCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            return cell;
        }

        private static PdfPCell CreateInfoCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            return cell;
        }

        private static PdfPCell CreateDataCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)));
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            return cell;
        }

        private static PdfPCell CreateSignatureCell(string text)
        {
            PdfPCell cell = new PdfPCell();
            cell.AddElement(new Paragraph(text, new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)));
            cell.AddElement(new Paragraph("________________________", new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.NORMAL)));
            cell.AddElement(new Paragraph("        (подпись)", new Font(Font.FontFamily.TIMES_ROMAN, 10, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.PaddingTop = 20;
            return cell;
        }
    }
}
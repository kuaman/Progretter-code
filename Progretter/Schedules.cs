using System.Windows;
using System.Data;
using ClosedXML.Excel;

namespace Progretter
{
    class Schedules
    {
        public static DataTable ImportExcel(string path)
        {
            DataTable dt = new DataTable();
            //Checking file content length and Extension must be .xlsx  
            using (XLWorkbook workbook = new XLWorkbook(path))
            {
                IXLWorksheet worksheet = workbook.Worksheet(1);
                bool FirstRow = true;
                //Range for reading the cells based on the last cell used.  
                string readRange = "1:1";
                foreach (IXLRow row in worksheet.RowsUsed())
                {
                    //If Reading the First Row (used) then add them as column name  
                    if (FirstRow)
                    {
                        //Checking the Last cellused for column generation in datatable  
                        readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                        foreach (IXLCell cell in row.Cells(readRange))
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }
                        FirstRow = false;
                    }
                    else
                    {
                        //Adding a Row in datatable  
                        dt.Rows.Add();
                        int cellIndex = 0;
                        //Updating the values of datatable  
                        foreach (IXLCell cell in row.Cells(readRange))
                        {
                            dt.Rows[dt.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                            cellIndex++;
                        }
                    }
                }
                //If no data in Excel file  
                if (FirstRow)
                {
                    MessageBox.Show("빈 파일!", "Error");
                }
            }
            return dt;
        }

        public static DataTable ExportExcel(DataView dataView, string path)
        {
            DataTable dt = new DataTable();
            dt = dataView.ToTable();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(dt, "Sheet1");
                workbook.SaveAs(path);
            }
            return dt;
        }
    }
}

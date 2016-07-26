using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace IntegracionPDF.Integracion_PDF.Utils.Integracion.XLSX
{
    public class ExcelExport
    {
        Hashtable _myHashtable;
        int _myExcelProcessId;

        Application _excel;
        Workbook _wbk;
        //Worksheet _worksheet1;

        readonly object _missing = Missing.Value;

        public enum FormatType
        {
            Xls,
            Xlsx,
            Pdf,
            Xps,
            Csv
        }
        public void Convert(FormatType formatType, string originalFile, string targetFile)
        {

            Console.WriteLine("1");
            CheckForExistingExcellProcesses();

            _excel = new Application
            {
                Visible = false,
                ScreenUpdating = false,
                DisplayAlerts = false
            };

            Console.WriteLine("2");
            GetTheExcelProcessIdThatUsedByThisInstance();

            Console.WriteLine("3");
            _wbk = _excel.Workbooks.Open(originalFile, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


            switch (formatType)
            {
                case FormatType.Xls:
                {
                    _wbk.SaveAs(targetFile, XlFileFormat.xlExcel8, _missing, _missing, _missing, _missing,
                        XlSaveAsAccessMode.xlExclusive, _missing, _missing, _missing, _missing, _missing);
                }
                    break;
                case FormatType.Xlsx:
                {
                    _wbk.SaveAs(targetFile, XlFileFormat.xlWorkbookDefault, _missing, _missing, _missing, _missing,
                        XlSaveAsAccessMode.xlExclusive, _missing, _missing, _missing, _missing, _missing);
                }
                    break;
                case FormatType.Pdf:
                {
                    _wbk.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, targetFile,
                        XlFixedFormatQuality.xlQualityStandard, false, true, _missing, _missing, false, _missing);
                }
                    break;
                case FormatType.Xps:
                {
                    _wbk.ExportAsFixedFormat(XlFixedFormatType.xlTypeXPS, targetFile,
                        XlFixedFormatQuality.xlQualityStandard, false, true, _missing, _missing, false, _missing);
                }
                    break;
                case FormatType.Csv:
                {
                    _wbk.SaveAs(targetFile, XlFileFormat.xlCSV, _missing, _missing, _missing, _missing,
                        XlSaveAsAccessMode.xlExclusive, _missing, _missing, _missing, _missing, _missing);
                }
                    break;
            }

            Console.WriteLine("4");
            ReleaseExcelResources();
            Console.WriteLine("5");
            KillExcelProcessThatUsedByThisInstance();
        }


        void ReleaseExcelResources()
        {
            //try
            //{
            //    Marshal.ReleaseComObject(_worksheet1);
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            //finally
            //{
            //    _worksheet1 = null;
            //}

            try
            {
                _wbk?.Close(false, _missing, _missing);
                if (_wbk != null) Marshal.ReleaseComObject(_wbk);
            }
            catch
            { }
            finally
            {
                _wbk = null;
            }

            try
            {
                _excel.Quit();
                Marshal.ReleaseComObject(_excel);
            }
            catch
            { }
            finally
            {
                _excel = null;
            }
        }

        void CheckForExistingExcellProcesses()
        {
            var allProcesses = Process.GetProcessesByName("excel");
            _myHashtable = new Hashtable();
            var iCount = 0;

            foreach (var excelProcess in allProcesses)
            {
                _myHashtable.Add(excelProcess.Id, iCount);
                iCount = iCount + 1;
            }
        }

        void GetTheExcelProcessIdThatUsedByThisInstance()
        {
            var allProcesses = Process.GetProcessesByName("excel");

            // Search For the Right Excel
            foreach (var excelProcess in allProcesses)
            {
                if (_myHashtable == null)
                    return;
                if (_myHashtable.ContainsKey(excelProcess.Id) == false)
                {
                    // Get the process ID
                    _myExcelProcessId = excelProcess.Id;
                }
            }
            allProcesses = null;
        }

        void KillExcelProcessThatUsedByThisInstance()
        {
            var allProcesses = Process.GetProcessesByName("excel");

            foreach (var excelProcess in allProcesses)
            {
                if (_myHashtable == null)
                    return;

                if (excelProcess.Id == _myExcelProcessId)
                    excelProcess.Kill();
            }

            allProcesses = null;
        }
    }
}
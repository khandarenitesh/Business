using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using CNF.Business.Model.OrderDispatch;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace CNF.Business.BusinessConstant
{
    public class PDFHelper
    {
        /// <summary>
        /// Generate PDF and QR Code
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="imgLocFile"></param>
        /// <returns></returns>
        public static string GeneratePDF(string pdfTitle, string imgLocFile, List<GetInvoiceDetailsForStickerModel> modelList,
                                         string stickerIpAddress, int stickerPortNumber)
        {
            string msgPDF = string.Empty, contentText = string.Empty;
            try
            {
                // Create PDF Document
                PdfDocument document = new PdfDocument();
                document.Info.Title = pdfTitle;
                // Pdf Page
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;

                for (int i = 0; i < modelList.Count; i++)
                {
                    contentText = modelList[i].InvNo + modelList[i].StockistNo + modelList[i].StockistName + modelList[i].CityName
                                + modelList[i].InvAmount + modelList[i].NoOfBox + modelList[i].TransporterNo + modelList[i].TransporterName;

                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    // Draw background
                    gfx.DrawImage(XImage.FromFile(imgLocFile), 0, 0);
                    XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
                    XTextFormatter tf = new XTextFormatter(gfx);
                    XRect rect = new XRect();

                    if (i < 3)
                    {
                        rect = new XRect(15, 15 + i * 180, 400, 170);
                    }
                    else
                    {
                        rect = new XRect(430, 15 + (i - 3) * 180, 400, 170);
                    }

                    gfx.DrawRectangle(XBrushes.SeaShell, rect);
                    tf.DrawString(contentText, font, XBrushes.Black, rect, XStringFormats.TopLeft);
                    gfx.Dispose();

                    #region Start - Generate Print
                    msgPDF = PDFHelper.GeneratePrint(stickerIpAddress, stickerPortNumber, contentText);
                    #endregion End - Generate Print

                    msgPDF = BusinessCont.SuccessStatus;
                }
            }
            catch(Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GeneratePDF", "Generate PDF and QR Code", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            return msgPDF;
        }

        /// <summary>
        /// Generate Print
        /// </summary>
        /// <param name="stickerIpAddress"></param>
        /// <param name="stickerPortNumber"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GeneratePrint(string stickerIpAddress, int stickerPortNumber, string filePath)
        {
            string msgPrint = string.Empty;
            Socket clientSocket;
            PrintDocument printDoc = new PrintDocument();

            try
            {
                BusinessCont.SaveLog(0, 0, 0, "GeneratePrint", "Generate Print", "START", "");

                // Socket
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.NoDelay = true;
                IPAddress IpAddress = IPAddress.Parse(stickerIpAddress);
                IPEndPoint iPEP = new IPEndPoint(IpAddress, stickerPortNumber);
                clientSocket.Connect(iPEP);
                if (!clientSocket.Connected)
                {
                    BusinessCont.SaveLog(0, 0, 0, "GeneratePrint", "Generate Print", "clientSocket.Connected", "Printer is not connected");
                }

                // Print Document
                printDoc.DocumentName = filePath;
                printDoc.DefaultPageSettings.Landscape = true;
                printDoc.PrintController = new StandardPrintController();
                printDoc.DefaultPageSettings.PaperSize = new PaperSize("StickerSize", 100, 50);
                printDoc.Print();
                printDoc.Dispose();
                // Socket Close
                clientSocket.Close();

                // To Msg Print Status - Success
                msgPrint = BusinessCont.SuccessStatus;
                BusinessCont.SaveLog(0, 0, 0, "GeneratePrint", "Generate Print", msgPrint, "");
            }
            catch (Exception ex)
            {
                BusinessCont.SaveLog(0, 0, 0, "GeneratePrint", "Generate Print", BusinessCont.FailStatus, BusinessCont.ExceptionMsg(ex.InnerException));
            }
            finally
            {
                printDoc = null;
                clientSocket = null;
            }
            return msgPrint;
        }

    }

}

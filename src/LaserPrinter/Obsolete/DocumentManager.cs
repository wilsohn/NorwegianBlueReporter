﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FSharp.Markdown.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using StatsReader;
using iTextSharp.text.pdf;

namespace LaserPrinter.Obsolete
{
    public class DocumentManager : IDocumentManager
    {
        private readonly Document _document;

        public DocumentManager(Document document)
        {
            _document = document;
            _document = new Document();
            _graphManager = new GraphManager();
            _tableManager = new TableManager();
        }

        public void CreateGraphSection(GraphData graphData)
        {
            GraphFactory.CreateGraph(graphData);
        }

        public void AppendMarkdown(string markdown, Section section = null)
        {
            if (section == null)
            {
                section = _document.LastSection;
            }
            MarkdownPdf.AddMarkdown(_document, section, markdown);   
        }

        public void AddMarkdown(string markdown)
        {
            AppendMarkdown( markdown, _document.AddSection());
        }

        public void AppendAnalysisNote(AnalysisNote analysisNote, Section section = null)
        {
            bool summaryPresent = !string.IsNullOrEmpty(analysisNote.Summary);
            bool graphPresent = (analysisNote.GraphData != null);

            if (section == null)
            {
                section = _document.LastSection;
            }

            if (summaryPresent)
            {
                AppendMarkdown(analysisNote.Summary, section);
            }

            if (graphPresent)
            {
                var graph = GraphFactory.CreateGraph(analysisNote.GraphData);
                graph.Draw(_document);
            }
        }

        public void SaveAsPdf(string fileName)
        {
            var renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always)
            {
                Document = _document
            };

            renderer.RenderDocument();
            renderer.PdfDocument.Save(fileName);
        }

        // Need to specify existing created by SaveAsPdf() and a new PDF file to write to. Due to the nature of PDFs, it seems like iTextSharp cannot
        // just insert an attachment to an already created PDF file. Instead, they need to generate a completely new one and copy the contents while
        // adding any new additions.
        public void AttachFileToDocument(string existingPdfFile, string updatedPdfFile, string attachmentFile)
        {
            var pdfReader = new PdfReader(existingPdfFile);
            var outStream = new FileStream(updatedPdfFile, FileMode.Create);

            var pdfStamper = new PdfStamper(pdfReader, outStream);
            var pdfWriter = pdfStamper.Writer;

            var pdfAttachment = PdfFileSpecification.FileEmbedded(pdfWriter, attachmentFile, attachmentFile, null);
            pdfStamper.AddFileAttachment(attachmentFile, pdfAttachment);
            pdfStamper.Close();
        }



        //
        //
        // TODO: Code below this are works in progress...
        public void EmbedFile(string pdfFile, string embeddedFile)
        {
            ImplantData(pdfFile, embeddedFile, 52, 0, "/Type /EmbeddedFile", Decode.Flate);
        }

        private void ImplantData(string pdfFile, string embeddedFile, int index, int version, string entryType, Decode decodeType)
        {
            bool dataAdded = false;
            const string xrefPattern = @"\d{10} \d{5} [nf]";

            var pdf = File.ReadAllText(pdfFile, Encoding.Default);
            var matches = Regex.Matches(pdf, xrefPattern);

            var offsets = new List<int>();
            foreach (Match match in matches)
            {
                var byteOffset = match.Value.Substring(0, 10);
                offsets.Add(Convert.ToInt32(byteOffset));
            }

            var maxOffset = offsets.Max(x => x);

            var gibberish = new FileStream("gibberish.pdf", FileMode.Open);
            var asdf = new BinaryReader(gibberish, Encoding.Default);
            var size = asdf.ReadBytes((int) gibberish.Length);

            //................................................................................................................

            var fsIn = new FileStream("Experiment Alpha.pdf", FileMode.Open);
            var br = new BinaryReader(fsIn, Encoding.Default);

            var bytes = new byte[fsIn.Length];
            br.Read(bytes, 0, (int) fsIn.Length - 6);

            var eof = new byte[6];
            fsIn.Seek(fsIn.Length - 6, SeekOrigin.Begin);
            br.Read(eof, 0, 6);

            var fsOut = new FileStream("Experiment Nu.pdf", FileMode.Create);
            var bw = new BinaryWriter(fsOut, Encoding.Default);

            //................................................................................................................

            var encoding = new UTF8Encoding();
            var builder = new StringBuilder();
            builder.Append("\n");
            builder.Append(embeddedFile.Length);
            builder.Append(string.Format("{0} {1} obj\n<<\n /Length {2}\n", index, version, embeddedFile.Length));
            builder.Append(string.Format(" /Filter {0}\n", decodeType));

            if (!string.IsNullOrEmpty(entryType))
            {
                builder.Append(string.Format(" {0}\n", entryType));
            }

            builder.Append(">>\nstream\n");

            byte[] startBuffer = encoding.GetBytes(builder.ToString());
            var embedStartBuffer = Encoding.Convert(Encoding.UTF8, Encoding.Default, startBuffer);

            //................................................................................................................

            var csvIn = new FileStream(embeddedFile, FileMode.Open);
            var csvReader = new BinaryReader(csvIn, Encoding.Default);
            var csvBytes = csvReader.ReadBytes((int)csvIn.Length);

            //................................................................................................................

            const string endObj = "\nendstream\nendobj\n";
            byte[] endBuffer = encoding.GetBytes(endObj);
            var embedEndBuffer = Encoding.Convert(Encoding.UTF8, Encoding.Default, endBuffer);

            //................................................................................................................

            bw.Write(bytes);
            bw.Write(embedStartBuffer);
            bw.Write(csvBytes);
            bw.Write(embedEndBuffer);
            bw.Write(eof);



            //var fs = new FileStream("SomethingNew", FileMode.Create);
            //var bw = new BinaryWriter(fs, Encoding.Default);

            //foreach (string t in result)
            //{
            //    var matchFound = regex.IsMatch(t);

            //    if (matchFound && !dataAdded)
            //    {
            //        dataAdded = true;
            //        var builder = new StringBuilder();
            //        builder.Append("\n");
            //        builder.Append(embeddedFile.Length);
            //        builder.Append(string.Format("{0} {1} obj\n<<\n /Length {2}\n", index, version, embeddedFile.Length));
            //        builder.Append(string.Format(" /Filter {0}\n", decodeType));

            //        if (!string.IsNullOrEmpty(entryType))
            //        {
            //            builder.Append(string.Format(" {0}\n", entryType));
            //        }

            //        builder.Append(">>\nstream\n");

            //        byte[] startBuffer = encoding.GetBytes(builder.ToString());
            //        bw.Write(startBuffer);

            //        var gZipCompressed = ConstructRawBinaryData(embeddedFile, decodeType);
            //        int length = (int)gZipCompressed.BaseStream.Length;
            //        var attachmentBytes = new byte[length];
            //        //gZipCompressed.Seek(0, SeekOrigin.Begin);
            //        //gZipCompressed.CopyTo(fs);
            //        //gZipCompressed.Write(attachmentBytes, 0, attachmentBytes.Length);

            //        const string endObj = "\nendstream\nendobj\n";
            //        byte[] endBuffer = encoding.GetBytes(endObj);
            //        bw.Write(endBuffer);
            //    }

            //    byte[] buffer = encoding.GetBytes(t);
            //    bw.Write(buffer);
            //}

            //bw.Close();
        }

        private GZipStream ConstructRawBinaryData(FileStream embeddedFile, Decode decodeType)
        {
            var length = (int)embeddedFile.Length;
            var buffer = new byte[length];
            DeflateStream encodedData;
            GZipStream gZipStream = null;

            switch (decodeType)
            {
                case Decode.AsciiHex:
                    //encodedData = null;
                    throw new NotImplementedException("This decode type is currently not supported ...");
                case Decode.Flate:
                    //encodedData = new DeflateStream(embeddedFile, CompressionMode.Compress);
                    //encodedData.Read(buffer, 0, length);
                    
                    gZipStream = new GZipStream(embeddedFile, CompressionMode.Compress);
                    //gZipStream.Read(buffer, 0, length);

                    //embeddedFile.Read(buffer, 0, length);
                    
                    break;
            }

            return gZipStream;
        }
    }
}
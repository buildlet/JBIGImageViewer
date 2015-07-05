/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015 Daiki Sakamoto

 Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using BUILDLet.Imaging;


namespace BUILDLet.JbigViewer
{
    public class JbigFragments : IDisposable
    {
        private readonly string fileHeaderText = "\x1B%-12345X@PJL\r\n";
        private readonly string fileFooterText = "\x1B%-12345X\r\n";
        private readonly string lineHeaderText = "@PJL ";

        private readonly int maxHeaderLength = 1024 * 2;
        private readonly int bufsize = (int)Math.Pow(1024, 2);

        private string inputFileName = string.Empty;
        private List<string> outputFileNames = new List<string>();


        public string SourceFileName
        {
            get { return this.inputFileName; }
        }


        public string[] FileNames
        {
            get { return this.outputFileNames.ToArray(); }
        }


        public JbigFragments(string filename)
        {
            this.inputFileName = filename;

            try
            {
                using (FileStream inputStream = new FileStream(this.inputFileName, FileMode.Open, FileAccess.Read))
                {
                    // Validate File Header
                    if (!this.readFileHeader(inputStream))
                    {
                        // PJL text data is NOT found at the file header.
                        this.outputFileNames.Add(this.inputFileName);
                        return;
                    }



                    // for each JBIG data fragments
                    while (inputStream.Position < inputStream.Length)
                    {
                        // initialize buffer (to read LINE header)
                        byte[] buffer = new byte[this.lineHeaderText.Length];



                        // Read PJL part
                        // Read anc Check Line Header
                        while ((inputStream.Read(buffer, 0, buffer.Length) == buffer.Length) && (ASCIIEncoding.ASCII.GetString(buffer) == this.lineHeaderText))
                        {
                            // Read to line end (CRLF)
                            for (int i = 0; i < this.maxHeaderLength; i++)
                            {
                                // reach to line end (CRLF)
                                if ((inputStream.ReadByte() == Convert.ToInt32('\r')) && (inputStream.ReadByte() == Convert.ToInt32('\n'))) { break; }

                                // PJL part does not end.
                                if (i >= this.maxHeaderLength - 1) { throw new FileFormatException(); }
                            }
                        }
                        // back current position of input stream
                        inputStream.Seek(-lineHeaderText.Length, SeekOrigin.Current);



                        // Check File Footer or not
                        if (this.checkFileFooter(inputStream)) { return; }


                        
                        // set buffer size (to read JBIG image data)
                        buffer = new byte[this.bufsize];

                        // get and set temporary file name
                        this.outputFileNames.Add(Path.GetTempFileName());

                        // Open output stream
                        using (FileStream outputStream =
                            new FileStream(this.outputFileNames[this.outputFileNames.Count - 1], FileMode.Append, FileAccess.Write, FileShare.Delete))
                        {
                            int readCount = 0;


                            // Read JBIG part
                            while ((readCount = inputStream.Read(buffer, 0, bufsize)) > 0)
                            {
                                int backCount = 0;
                                for (int i = 0; i < buffer.Length; i++)
                                {
                                    // Check only 1st byte
                                    if (buffer[i] == this.lineHeaderText[0])
                                    {
                                        if (Encoding.ASCII.GetString(buffer, i, this.lineHeaderText.Length) == this.lineHeaderText)
                                        {
                                            backCount = readCount - i;
                                            break;
                                        }
                                    }
                                }

                                // write to output stream
                                outputStream.Write(buffer, 0, readCount - backCount);

                                // Next Fragment
                                if (backCount > 0)
                                {
                                    // back current position of input stream
                                    inputStream.Seek(-backCount, SeekOrigin.Current);

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) { throw e; }
        }


        public void Dispose()
        {
            try
            {
                foreach (var filename in this.outputFileNames)
                {
                    if ((filename != this.SourceFileName) && (File.Exists(filename))) { File.Delete(filename); }
                }

                this.outputFileNames.Clear();
            }
            catch (Exception e) { throw e; }
        }


        private bool readFileHeader(Stream stream)
        {
            // set buffer size (to read FILE Header)
            byte[] buffer = new byte[this.fileHeaderText.Length];

            // (Validate) Read and Check the file header
            return (
                (stream.Read(buffer, 0, this.fileHeaderText.Length) == this.fileHeaderText.Length) &&
                (ASCIIEncoding.ASCII.GetString(buffer) == this.fileHeaderText));
        }


        private bool checkFileFooter(Stream stream)
        {
            // set buffer size (to read FILE Footer)
            byte[] buffer = new byte[this.fileFooterText.Length];

            // (Validate) Read and Check the file header
            bool result = 
                (stream.Read(buffer, 0, this.fileFooterText.Length) == this.fileFooterText.Length) &&
                (ASCIIEncoding.ASCII.GetString(buffer) == this.fileFooterText);

            stream.Seek(-this.fileFooterText.Length, SeekOrigin.Current);

            return result;
        }
    }
}

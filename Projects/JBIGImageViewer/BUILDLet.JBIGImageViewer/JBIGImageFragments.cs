/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2015-2017 Daiki Sakamoto

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


namespace BUILDLet.JBIGImageViewer
{
    public class JBIGImageFragments : IDisposable
    {
        // Unexpected text data
        private readonly string fileHeaderText = "\x1B%-12345X@PJL\r\n";
        private readonly string fileFooterText = "\x1B%-12345X\r\n";
        private readonly string lineHeaderText = "@PJL";

        // Number of Unexpected Text Line(s)
        private readonly int removableLineCount = 3;


        // File name of page(s)
        private List<string> filenames = new List<string>();


        // Source JBIG Image File Name
        public string SourceFileName { get; protected set; } = string.Empty;


        // JBIG Image File Name(s)
        public string[] FileNames
        {
            get { return this.filenames.ToArray(); }
        }


        // Number of Pages of JBIG Image Fragments
        public int Pages
        {
            get { return this.FileNames.Length; }
        }


        // Constructor
        public JBIGImageFragments(string filename)
        {
            // store Source JBIG Image File Name
            this.SourceFileName = filename;

            try
            {
                // open Input Stream
                using (FileStream inputStream = new FileStream(this.SourceFileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[this.fileHeaderText.Length];

                    // Validate File Header
                    if ((inputStream.Read(buffer, 0, this.fileHeaderText.Length) != this.fileHeaderText.Length) ||
                        (ASCIIEncoding.ASCII.GetString(buffer) != this.fileHeaderText))
                    {
                        // PJL lineText data is NOT found at the file header.
                        // JBIG Image Fragments consist of only source file.
                        this.filenames.Add(this.SourceFileName);
                        return;
                    }
                    else
                    {
                        // for JBIG data fragments Roop
                        while (inputStream.Position < inputStream.Length)
                        {
                            int data;
                            int lineCount = 0;
                            bool pageHead = true;
                            List<byte> textPart = new List<byte>();
                            List<byte> footPart = new List<byte>();


                            // get and set temporary file name
                            this.filenames.Add(Path.GetTempFileName());

                            // open Output Stream
                            using (FileStream outputStream = 
                                new FileStream(this.filenames[this.filenames.Count - 1], FileMode.Append, FileAccess.Write, FileShare.Delete))
                            {
                                // Read Byte
                                while ((data = inputStream.ReadByte()) >= 0)
                                {
                                    // Check File Footer
                                    if (footPart.Count <= 0)
                                    {
                                        // NOT IN Footer Part

                                        if (data == this.fileFooterText[0])
                                        {
                                            // Enter Footer Part
                                            footPart.Add((byte)data);
                                        }
                                    }
                                    else
                                    {
                                        // IN File Footer Part, or END of File Footer Part

                                        // Length check
                                        if (footPart.Count < this.fileFooterText.Length)
                                        {
                                            if (data == this.fileFooterText[footPart.Count])
                                            {
                                                // Continue Footer Part
                                                footPart.Add((byte)data);

                                                // Reached to Footer
                                                if (footPart.Count == this.fileFooterText.Length) { return; }
                                            }
                                            else
                                            {
                                                // Write buffer to Output Stream
                                                outputStream.Write(footPart.ToArray(), 0, footPart.Count);

                                                // Exit Footer Part
                                                footPart.Clear();
                                            }
                                        }
                                        else { throw new ApplicationException(); }
                                    }


                                    // Check Text Line Part
                                    if (textPart.Count <= 0)
                                    {
                                        // NOT IN Text Line Part

                                        if (data == this.lineHeaderText[0])
                                        {
                                            // Enter Text Line Part
                                            textPart.Add((byte)data);
                                        }
                                    }
                                    else
                                    {
                                        // IN Text Line Part, or END of Text Line Part

                                        // Length check
                                        if (textPart.Count < this.lineHeaderText.Length)
                                        {
                                            // IN Line Header Part, or END of Line Header Part

                                            if (data == this.lineHeaderText[textPart.Count])
                                            {
                                                // Continue Text Line Part
                                                textPart.Add((byte)data);
                                            }
                                            else
                                            {
                                                // Write buffer to Output Stream
                                                outputStream.Write(textPart.ToArray(), 0, textPart.Count);

                                                // Exit Text Line Part
                                                textPart.Clear();
                                            }
                                        }
                                        else
                                        {
                                            // NOT IN Line Header Part

                                            if ((data == '\r') || (data == '\n') || ((data >= 0x20) || (data <= 0x7e)))
                                            {
                                                // Continue Text Line Part
                                                textPart.Add((byte)data);

                                                // Check Line Break
                                                if ((textPart.Count > 2) && (textPart[textPart.Count - 2] == '\r') && (textPart[textPart.Count - 1] == '\n'))
                                                {
                                                    // Increment Text Line Count, and Check Page Break
                                                    if (!pageHead && ((++lineCount) > this.removableLineCount)) { break; }

                                                    // Clear buffer and Continue
                                                    textPart.Clear();
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                // Write buffer to Output Stream
                                                outputStream.Write(textPart.ToArray(), 0, textPart.Count);

                                                // Exit Text Line Part
                                                textPart.Clear();
                                            }
                                        }
                                    }


                                    // Check Binary Data
                                    if ((footPart.Count <= 0) && (textPart.Count <= 0))
                                    {
                                        // write data to Output Stream
                                        outputStream.WriteByte((byte)data);

                                        // Exit Header Part
                                        pageHead = false;

                                        // Clear Text Line Count
                                        lineCount = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { throw; }
        }


        public void Dispose()
        {
            try
            {
                foreach (var filename in this.filenames)
                {
                    if ((filename != this.SourceFileName) && (File.Exists(filename))) { File.Delete(filename); }
                }

                this.filenames.Clear();
            }
            catch (Exception) { throw; }
        }
    }
}

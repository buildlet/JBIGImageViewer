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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

using BUILDLet.JBIG.Tests.Properties;


namespace BUILDLet.JBIG.Tests
{
    [TestClass()]
    public class JBIGImageConverterTests
    {
        [TestMethod()]
        public void ConvertTest()
        {
            string[] input_files = null;
            JBIGImageConverterInputImageFormat input_format;


            // Input Format = JBIG
            foreach (JBIGImageConverterOutputImageFormat output_format in Enum.GetValues(typeof(JBIGImageConverterOutputImageFormat)))
            {
                // Set Input File and Format (JBIG)
                input_files = Directory.GetFiles(Resources.JBIGTestImageDirectoryPath, "*.jbg", SearchOption.TopDirectoryOnly);
                input_format = JBIGImageConverterInputImageFormat.Jbig;

                // Convert (JBIG -> Other Format)
                this.convert(input_files, input_format, output_format);
            }


            // Set Input File and Format (PBM)
            input_files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pbm", SearchOption.TopDirectoryOnly);
            input_format = JBIGImageConverterInputImageFormat.Pbm;

            // Convert (PBM -> BMP)
            this.convert(input_files, input_format, JBIGImageConverterOutputImageFormat.Jpeg);
        }


        private void convert(string[] input_files, JBIGImageConverterInputImageFormat input_format, JBIGImageConverterOutputImageFormat output_format)
        {
            string extension = null;

            switch (output_format)
            {
                case JBIGImageConverterOutputImageFormat.Pbm:
                    extension = ".pbm";
                    break;

                case JBIGImageConverterOutputImageFormat.Bmp:
                    extension = ".bmp";
                    break;

                case JBIGImageConverterOutputImageFormat.Png:
                    extension = ".png";
                    break;

                case JBIGImageConverterOutputImageFormat.Gif:
                    extension = ".gif";
                    break;

                case JBIGImageConverterOutputImageFormat.Jpeg:
                    extension = ".jpg";
                    break;

                case JBIGImageConverterOutputImageFormat.Tiff:
                    extension = ".tif";
                    break;

                default:
                    break;
            }

            // Overwrite Extension
            if ((input_format == JBIGImageConverterInputImageFormat.Pbm) && (output_format == JBIGImageConverterOutputImageFormat.Jpeg))
            {
                extension = ".jpeg";
            }


            string conversion =
                "Convert \"" + Enum.GetName(typeof(JBIGImageConverterInputImageFormat), input_format) + "\" " +
                "To \"" + Enum.GetName(typeof(JBIGImageConverterOutputImageFormat), output_format) + "\"";

            // Show Message (Start)
            MessageBox.Show(conversion + ": Start!", "JBIGImageConverterTests.ConvertTest");


            // Convert Each Files
            foreach (var input_filename in input_files)
            {
                string output_filename = Path.GetFileNameWithoutExtension(input_filename) + extension;

                // Convert and Assert
                if (!JBIGImageConverter.Convert(input_filename, ref output_filename, input_format, output_format)) { Assert.Fail(); }

                // Show Image (Except PBM)
                if (output_format != JBIGImageConverterOutputImageFormat.Pbm)
                {
                    Process proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(output_filename);
                    proc.Start();
                }
            }


            if (output_format != JBIGImageConverterOutputImageFormat.Pbm)
            {
                // Show Message
                MessageBox.Show("Please close all window(s).", "JBIGImageConverterTests.ConvertTest");
            }

            // Show Message (Completed)
            MessageBox.Show(conversion + ": Completed!", "JBIGImageConverterTests.ConvertTest");
        }
    }
}

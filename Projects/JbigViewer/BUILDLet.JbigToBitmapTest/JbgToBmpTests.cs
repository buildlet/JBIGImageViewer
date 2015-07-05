﻿/*******************************************************************************
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Diagnostics;
using System.Windows.Forms;


namespace BUILDLet.Imaging.Tests
{
    [TestClass()]
    public class JbgToBmpTests
    {
        [TestMethod()]
        public void JbgToBmpTest()
        {
            // Show Message
            MessageBox.Show("Please delete the temporary bitmap image file(s).", "JbgToBmpTest");

            foreach (var jbigfile in Directory.GetFiles(@"..\..\..\..\Common\TestData\jbigkit-1.6\examples", "*.jbg", SearchOption.TopDirectoryOnly))
            {
                string bitmap = JbigToBitmap.Convert(jbigfile);
                File.Move(bitmap, (bitmap = bitmap + ".bmp"));

                Process proc = new Process();
                proc.StartInfo = new ProcessStartInfo(@bitmap);
                proc.Start();
            }
        }
    }
}
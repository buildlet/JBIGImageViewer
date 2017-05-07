/*******************************************************************************
 The MIT License (MIT)

 Copyright (c) 2017 Daiki Sakamoto

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

using BUILDLet.JBIG;


namespace BUILDLet.JBIGImageViewer
{
    public class SaveFileDialogFilter
    {
        public static JBIGImageConverterOutputImageFormat GetOutputImageFormat(int Index)
        {
            switch (Index)
            {
                case 1:
                    return JBIGImageConverterOutputImageFormat.Png;

                case 2:
                    return JBIGImageConverterOutputImageFormat.Bmp;

                case 3:
                    return JBIGImageConverterOutputImageFormat.Gif;

                case 4:
                    return JBIGImageConverterOutputImageFormat.Jpeg;

                case 5:
                    return JBIGImageConverterOutputImageFormat.Tiff;

                default:
                    throw new NotSupportedException();
            }
        }

        public new static string ToString()
        {
            return
                "Portable Network Graphics (PNG) Image Files (*.png)|*.png|" +
                "Bitmap (BMP) Image Files (*.bmp)|*.bmp|" +
                "Graphics Interchange Format (GIF) Image Files (*.gif)|*.gif|" +
                "Joint Photographic Experts Group (JPEG) Image Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Tagged Image File Format (TIFF) Image Files (*.tif;*.tiff)|*.tif;*.tiff|" +
                "All Files (*.*)|*.*";
        }
    }
}

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


namespace BUILDLet.JBIG
{
    /// <summary>
    /// <see cref="JBIGImageConverter"/> クラスがサポートしている変換先の画像フォーマットを表します。
    /// </summary>
    public enum JBIGImageConverterOutputImageFormat
    {
        /// <summary>
        /// PBM (Portable Bitmap), PNM (Portable Any Map) or PPM (Portable Pixel Map) image format
        /// </summary>
        Pbm,

        /// <summary>
        /// Bitmap (BMP) image format
        /// </summary>
        Bmp,

        /// <summary>
        /// W3C Portable Network Graphics (PNG) image format
        /// </summary>
        Png,

        /// <summary>
        /// Graphics Interchange Format (GIF) image format
        /// </summary>
        Gif,

        /// <summary>
        ///  Joint Photographic Experts Group (JPEG) image format
        /// </summary>
        Jpeg,

        /// <summary>
        /// Tagged Image File Format (TIFF) image format
        /// </summary>
        Tiff
    }
}

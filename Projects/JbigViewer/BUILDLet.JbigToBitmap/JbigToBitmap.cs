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
using System.Diagnostics;
using System.Reflection;


namespace BUILDLet.Imaging
{

    /// <summary>
    /// JIBG1 フォーマットの画像ファイルをビットマップ画像ファイルへの変換を実装します。
    /// </summary>
    public class JbigToBitmap
    {
        /// <summary>
        /// <see cref="JbigToBitmap"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected JbigToBitmap() { }


        /// <summary>
        /// JIBG1 フォーマットの画像ファイルをビットマップ画像ファイルに変換します。
        /// </summary>
        /// <param name="filename">JIBG1 フォーマットの画像ファイルのパス</param>
        /// <returns></returns>
        public static string Convert(string filename)
        {
            try
            {
                string pbmFileName = Path.GetTempFileName();
                string bmpFileName = Path.GetTempFileName();

                try
                {
                    // JBIG -> PBM (PPM)
                    JbigToBitmap.JbigToPbm(filename, ref pbmFileName);
                    Console.WriteLine("[JbgToBmp.Convert] \"{0}\" is created.", pbmFileName);

                    // PPM -> Bitmap
                    JbigToBitmap.PpmToBitmap(pbmFileName, ref bmpFileName);
                    Console.WriteLine("[JbgToBmp.Convert] \"{0}\" is created.", bmpFileName);

                    return bmpFileName;
                }
                catch (Exception e)
                {
                    // Clean temporary Bitmap file
                    if (File.Exists(bmpFileName))
                    {
                        File.Delete(bmpFileName);
                        Console.WriteLine("[JbgToBmp.Convert] \"{0}\" is deleted.", bmpFileName);
                    }

                    throw e;
                }
                finally
                {
                    // Clean temporary PBM file
                    if (File.Exists(pbmFileName))
                    {
                        File.Delete(pbmFileName);
                        Console.WriteLine("[JbgToBmp.Convert] \"{0}\" is deleted.", pbmFileName);
                    }
                }
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// JIBG1 フォーマットの画像ファイルを PBM (Portable Bitmap) フォーマットの画像ファイルに変換します。
        /// </summary>
        /// <param name="input">JIBG1 フォーマットの入力画像ファイルのパス</param>
        /// <param name="output">PBM フォーマットの出力画像ファイルのパス</param>
        /// <remarks>
        /// jbgtopbm.exe を使用しているため、jbgtopbm.exe および jbig1.dll が必要です。
        /// </remarks>
        public static void JbigToPbm(string input, ref string output)
        {
            try
            {
                cmd_exe(string.Format("\"{0}\\jbgtopbm.exe\" \"{1}\" \"{2}\"", 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), input, output));
            }
            catch (Exception e) { throw new ApplicationException(string.Format(Properties.Resources.ApplicationErrorMessageTemplate, "jbgtopbm.exe"), e); }
        }


        /// <summary>
        /// PPM (Portable Pixel Map) フォーマットの画像ファイルをビットマップ画像ファイルに変換します。
        /// </summary>
        /// <param name="input">PPM フォーマットの出力画像ファイルのパス</param>
        /// <param name="output">ビットマップ出力画像ファイルのパス</param>
        /// <remarks>
        /// ppmtobmp.exe を使用しているため、ppmtobmp.exe および libnetpbm10.dll が必要です。
        /// </remarks>
        public static void PpmToBitmap(string input, ref string output)
        {
            try
            {
                cmd_exe(string.Format("\"{0}\\ppmtobmp.exe\" \"{1}\" -windows > \"{2}\"", 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), input, output));
            }
            catch (Exception e) { throw new ApplicationException(string.Format(Properties.Resources.ApplicationErrorMessageTemplate, "ppmtobmp.exe"), e); }
        }


        // cmd.exe
        private static void cmd_exe(string arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", string.Format("/C \"{0}\"", arguments));
                Process proc = new Process();

                // Initialize Process
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo = startInfo;

                // Process Start
                proc.Start();

                // Wait for exit
                proc.WaitForExit();

                // Validate Exit Code
                if (proc.ExitCode != 0) { throw new ApplicationException(); }
            }
            catch (Exception e) { throw e; }
        }
    }
}

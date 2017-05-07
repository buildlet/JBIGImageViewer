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
using System.Diagnostics;
using System.Reflection;

using BUILDLet.JBIG.Properties;


namespace BUILDLet.JBIG
{
    /// <summary>
    /// JIBG1 フォーマットの画像ファイルの、他のフォーマットの画像ファイルへの変換を実装します。
    /// </summary>
    public class JBIGImageConverter
    {
        private static string get_exe_filename(
            JBIGImageConverterInputImageFormat inputFormat = JBIGImageConverterInputImageFormat.Jbig,
            JBIGImageConverterOutputImageFormat outputFormat = JBIGImageConverterOutputImageFormat.Pbm)
        {
            switch (inputFormat)
            {
                case JBIGImageConverterInputImageFormat.Jbig:

                    // Input Format = JBIG
                    switch (outputFormat)
                    {
                        case JBIGImageConverterOutputImageFormat.Pbm:

                            // jbgtopbm.exe: JBG -> PBM
                            return Resources.FileName_JbgToPbm_EXE;

                        default:
                            throw new NotSupportedException();
                    }

                case JBIGImageConverterInputImageFormat.Pbm:

                    // Input Format = PBM
                    switch (outputFormat)
                    {
                        case JBIGImageConverterOutputImageFormat.Bmp:

                            // PPM -> BMP
                            return Resources.FileName_PpmToBmp_EXE;

                        case JBIGImageConverterOutputImageFormat.Png:

                            // PPM -> PNG
                            return Resources.FileName_PnmToPng_EXE;

                        case JBIGImageConverterOutputImageFormat.Gif:

                            // PPM -> GIF
                            return Resources.FileName_PpmToGif_EXE;

                        case JBIGImageConverterOutputImageFormat.Jpeg:

                            // PPM -> JPEG
                            return Resources.FileName_PpmToJpeg_EXE;

                        case JBIGImageConverterOutputImageFormat.Tiff:

                            // PPM -> TIFF
                            return Resources.FileName_PnmToTiff_EXE;

                        default:
                            throw new NotSupportedException();
                    }

                default:
                    throw new NotSupportedException();
            }
        }


        /// <summary>
        /// <see cref="JBIGImageConverter"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        protected JBIGImageConverter() { }


        /// <summary>
        /// JIBG1 フォーマットの画像ファイルをビットマップ画像ファイルに変換します。
        /// </summary>
        /// <param name="inputFileName">入力画像ファイルのファイル名</param>
        /// <param name="outputFileName">出力画像ファイルのファイル名</param>
        /// <param name="inputFormat">入力画像ファイルのイメージ フォーマット</param>
        /// <param name="outputFormat">出力画像ファイルのイメージ フォーマット</param>
        /// <returns>出力画像ファイルの作成に成功した場合に true を返します。</returns>
        public static bool Convert(string inputFileName, ref string outputFileName,
            JBIGImageConverterInputImageFormat inputFormat = JBIGImageConverterInputImageFormat.Jbig,
            JBIGImageConverterOutputImageFormat outputFormat = JBIGImageConverterOutputImageFormat.Pbm)
        {
            // Initialize PBM Filename
            string pbm_filename = null;

            // Initialize Current Input/Output Image Format
            JBIGImageConverterInputImageFormat currentInputFormat = inputFormat;
            JBIGImageConverterOutputImageFormat currentOutputFormat = outputFormat;

            try
            {
                // Input Format = JBIG
                if (inputFormat == JBIGImageConverterInputImageFormat.Jbig)
                {
                    // Set PBM Filename
                    pbm_filename = ((outputFormat == JBIGImageConverterOutputImageFormat.Pbm) ? outputFileName : Path.GetTempFileName());

                    // Update Current Input/Output Image Format (JBIG -> PBM)
                    currentInputFormat = inputFormat;
                    currentOutputFormat = JBIGImageConverterOutputImageFormat.Pbm;

                    // Run jbgtopbm.exe (JBIG -> PBM)
                    cmd_exe(commandline(inputFileName, pbm_filename));
                    Debug.WriteLine(string.Format("[JBIGImageConverter.Convert] \"{0}\" is created.", pbm_filename));
                }


                switch (inputFormat)
                {
                    // Input Format = JBIG
                    case JBIGImageConverterInputImageFormat.Jbig:

                        switch (outputFormat)
                        {
                            // JBIG -> PBM (PPM)
                            case JBIGImageConverterOutputImageFormat.Pbm:

                                // JBIG -> PBM
                                // Do Nothing
                                break;

                            // JBIG -> PBM -> Other Format
                            default:

                                // Update Current Input/Output Image Format (PBM -> Other Format)
                                currentInputFormat = JBIGImageConverterInputImageFormat.Pbm;
                                currentOutputFormat = outputFormat;

                                // Ron Command (PBM -> Other Format)
                                cmd_exe(commandline(pbm_filename, outputFileName, currentInputFormat, currentOutputFormat));
                                Debug.WriteLine(string.Format("[JBIGImageConverter.Convert] \"{0}\" is created.", outputFileName));

                                break;
                        }
                        break;

                    // Input Format = PBM
                    case JBIGImageConverterInputImageFormat.Pbm:

                        switch (outputFormat)
                        {
                            case JBIGImageConverterOutputImageFormat.Pbm:

                                // PBM -> PBM
                                // Do Nothing
                                break;

                            default:

                                // Update Current Input/Output Image Format (PBM -> Other Format)
                                currentInputFormat = inputFormat;
                                currentOutputFormat = outputFormat;

                                // Run Command (PBM -> Other Format)
                                cmd_exe(commandline(inputFileName, outputFileName, currentInputFormat, currentOutputFormat));
                                Debug.WriteLine(string.Format("[JBIGImageConverter.Convert] \"{0}\" is created.", outputFileName));

                                break;
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }


                // RETURN
                return File.Exists(outputFileName);
            }
            catch (Exception e)
            {
                // Clean output file
                if (File.Exists(outputFileName))
                {
                    File.Delete(outputFileName);
                    Debug.WriteLine(string.Format("[JBIGImageConverter.Convert] \"{0}\" is deleted.", outputFileName));
                }

                throw new ApplicationException(string.Format(
                    Resources.ErrorMessageTemplate, JBIGImageConverter.get_exe_filename(currentInputFormat, currentOutputFormat)), e);
            }
            finally
            {
                // Clean temporary PBM file
                if ((pbm_filename != outputFileName) && (File.Exists(pbm_filename)))
                {
                    File.Delete(pbm_filename);
                    Debug.WriteLine(string.Format("[JBIGImageConverter.Convert] \"{0}\" is deleted.", pbm_filename));
                }
            }
        }


        // command line
        private static string commandline(string inputFilePath, string outputFilePath,
            JBIGImageConverterInputImageFormat inputFormat = JBIGImageConverterInputImageFormat.Jbig,
            JBIGImageConverterOutputImageFormat outputFormat = JBIGImageConverterOutputImageFormat.Pbm)
        {
            string commandline_base_JbgToPbm = "\"{0}\" \"{1}\" \"{2}\"";
            string commandline_base_PpmToBmp = "\"{0}\" \"{1}\" -windows > \"{2}\"";
            string commandline_base = "\"{0}\" \"{1}\" > \"{2}\"";

            string filename = JBIGImageConverter.get_exe_filename(inputFormat, outputFormat);
            string filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);


            switch (inputFormat)
            {
                case JBIGImageConverterInputImageFormat.Jbig:

                    // Input Format = JBIG
                    switch (outputFormat)
                    {
                        case JBIGImageConverterOutputImageFormat.Pbm:

                            // JBIG -> PBM
                            return string.Format(commandline_base_JbgToPbm, filepath, inputFilePath, outputFilePath);

                        default:
                            throw new NotSupportedException();
                    }

                case JBIGImageConverterInputImageFormat.Pbm:

                    // Input Format = PBM
                    switch (outputFormat)
                    {
                        case JBIGImageConverterOutputImageFormat.Pbm:

                            // PBM -> PBM
                            throw new ArgumentException();

                        case JBIGImageConverterOutputImageFormat.Bmp:

                            // PPM -> BMP
                            return string.Format(commandline_base_PpmToBmp, filepath, inputFilePath, outputFilePath);

                        default:

                            // PPM -> Other
                            return string.Format(commandline_base, filepath, inputFilePath, outputFilePath);
                    }

                default:
                    throw new NotImplementedException();
            }
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
            catch (Exception) { throw; }
        }
    }
}

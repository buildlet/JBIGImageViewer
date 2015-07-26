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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;

using BUILDLet.Utilities.WPF;
using BUILDLet.Imaging;


namespace BUILDLet.JbigViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private JbigFragments innerJbigData = null;
        private Dictionary<int, string> bitmapFileNames = new Dictionary<int, string>();

        private string sourceFileName = string.Empty;
        private int currentPage = -1;


        // Show Image
        private void showImage(int page)
        {
            // Validate
            if (this.innerJbigData != null)
            {
                if ((page < 1) || (page > this.innerJbigData.FileNames.Length)) { throw new ApplicationException(); }
            }

            // Disable Button(s)
            this.LeftButton.IsEnabled = false;
            this.LeftArrowPolygon.Fill = new SolidColorBrush(Colors.LightGray);
            this.RightButton.IsEnabled = false;
            this.RightArrowPolygon.Fill = new SolidColorBrush(Colors.LightGray);

            // Change Cursor
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                string filename;
                if (!this.bitmapFileNames.TryGetValue(page - 1, out filename))
                {
                    if (this.innerJbigData == null)
                    {
                        filename = this.sourceFileName;
                    }
                    else
                    {
                        filename = this.innerJbigData.FileNames[page - 1];
                    }

                    // JBIG -> Bitmap
                    this.bitmapFileNames.Add(page - 1, JbigToBitmap.Convert(filename));
                }


                // new Btimap Image
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(this.bitmapFileNames[page - 1]);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                // Set ImageSource
                this.MainImage.Source = image;

                // set current page number
                this.currentPage = page;

                // update status
                this.updateStatus();
            }
            catch (Exception e) { throw e; }
            finally
            {
                // Restore Cursor
                Mouse.OverrideCursor = null;
            }
        }


        // Open Bitmap Image File
        private void openImage(string filename)
        {
            try
            {
                // Close Image if already opened.
                if (this.currentPage > 0) { this.closeImage(); }

                // set source file name
                this.sourceFileName = filename;


                // create inner JBIG Image Data
                if (this.RemoveUnexpectedDataMenuItem.IsChecked)
                {
                    this.innerJbigData = new JbigFragments(this.sourceFileName);
                }


                // show Bitmap Image
                this.showImage(1);
            }
            catch (Exception e) { throw e; }
        }


        // Show Page
        private void showPage(int page)
        {
            if ((page != this.currentPage) && (page > 0) && (this.innerJbigData != null) && (page <= this.innerJbigData.FileNames.Length))
            {
                // Clear ImageSource
                this.MainImage.Source = null;

                try
                {
                    // Show Bitmap Image
                    // and Increment current page number
                    this.showImage(page);
                }
                catch (Exception e) { throw e; }
            }
        }

        
        // Show Next Page
        private void showNextPage()
        {
            if ((this.currentPage > 0) && (this.currentPage < this.innerJbigData.FileNames.Length))
            {
                // Clear ImageSource
                this.MainImage.Source = null;

                try
                {
                    // Show Bitmap Image
                    // and Increment current page number
                    this.showImage(++this.currentPage);
                }
                catch (Exception e) { throw e; }
            }
        }


        // Show Previous Page
        private void showPreviousPage()
        {
            if ((this.currentPage > 1) && (this.currentPage <= this.innerJbigData.FileNames.Length))
            {
                // Clear ImageSource
                this.MainImage.Source = null;

                try
                {
                    // Show Bitmap Image
                    // and Increment current page number
                    this.showImage(--this.currentPage);
                }
                catch (Exception e) { throw e; }
            }
        }


        // Close Image
        private void closeImage()
        {
            // only if the image is already opened.
            if (this.currentPage > 0)
            {
                // Clear ImageSource
                this.MainImage.Source = null;


                try
                {
                    // Clear JBIG Fragments
                    if (this.innerJbigData != null)
                    {
                        this.innerJbigData.Dispose();
                        this.innerJbigData = null;
                    }

                    // Delete Bitmap Image Page File(s)
                    if (this.bitmapFileNames != null)
                    {
                        foreach (var filename in this.bitmapFileNames.Values)
                        {
                            // Delete Bitmap Image File (if exists)
                            if (File.Exists(filename)) { File.Delete(filename); }
                        }

                        // Clear
                        this.bitmapFileNames.Clear();
                    }

                }
                catch (Exception e) { throw e; }


                // Clear current page number
                this.currentPage = -1;

                // update status
                this.updateStatus();
            }
        }


        // Save as Bitmap Image
        private void saveImage(string filename)
        {
            try
            {
                // Save (Copy) Image as Bitmap File
                File.Copy(this.bitmapFileNames[this.currentPage - 1], filename, true);
            }
            catch (Exception e) { throw e; }
        }


        // Update Page Number TextBox
        private void updatePageNumberTextBox()
        {
            try
            {
                int page;
                if (int.TryParse(this.PageNumberTextBox.Text, out page) && (this.PageNumberTextBox.Text != this.currentPage.ToString()))
                {
                    // Show Page
                    this.showPage(page);
                }

                // Update (Modify) Page Number
                this.PageNumberTextBox.Text = this.currentPage.ToString();
            }
            catch (Exception e) { throw e; }
        }



        // Update Status
        private void updateStatus()
        {
            // Page Number
            if (this.currentPage > 0)
            {
                this.PageNumberTextBox.Text = this.currentPage.ToString();
            }
            else
            {
                this.PageNumberTextBox.Text = string.Empty;
            }


            // Menu
            if (this.currentPage > 0)
            {
                this.SaveAsMenuItem.IsEnabled = true;
                this.CloseMenuItem.IsEnabled = true;
                this.SameAsOriginalImageSizeMenuItem.IsEnabled = true;
                this.RemoveUnexpectedDataMenuItem.IsEnabled = false;
            }
            else
            {
                this.SaveAsMenuItem.IsEnabled = false;
                this.CloseMenuItem.IsEnabled = false;
                this.SameAsOriginalImageSizeMenuItem.IsEnabled = false;
                this.RemoveUnexpectedDataMenuItem.IsEnabled = true;
            }


            // Previous Page
            if (this.currentPage > 1)
            {
                this.PreviousPageMenuItem.IsEnabled = true;
                this.LeftButton.IsEnabled = true;
                this.LeftArrowPolygon.Fill = new SolidColorBrush(Colors.DimGray);
            }
            else
            {
                this.PreviousPageMenuItem.IsEnabled = false;
                this.LeftButton.IsEnabled = false;
                this.LeftArrowPolygon.Fill = new SolidColorBrush(Colors.LightGray);
            }


            // Next Page
            if ((this.currentPage > 0) && (this.innerJbigData != null) && (this.currentPage < this.innerJbigData.FileNames.Length))
            {
                this.NextPageMenuItem.IsEnabled = true;
                this.RightButton.IsEnabled = true;
                this.RightArrowPolygon.Fill = new SolidColorBrush(Colors.DimGray);
            }
            else
            {
                this.NextPageMenuItem.IsEnabled = false;
                this.RightButton.IsEnabled = false;
                this.RightArrowPolygon.Fill = new SolidColorBrush(Colors.LightGray);
            }
        }
    }
}

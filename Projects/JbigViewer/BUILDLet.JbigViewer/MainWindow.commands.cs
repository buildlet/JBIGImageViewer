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
        private int currentPage = 0;

        private bool sameAsOriginalImageSize = false;

        private double minImageWidth = 100;
        private double maxImageWidth = 200 * 500;
        private double defaultImageWidth = 390;


        // Show Image
        private void showImage(int page)
        {
            // Validate
            if (this.innerJbigData != null)
            {
                if ((page < 1) || (page > this.innerJbigData.Pages)) { throw new ApplicationException(); }
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
                this.MainImage.MinWidth = this.minImageWidth;
                this.MainImage.MaxWidth = this.maxImageWidth;

                // Set default Image Width
                this.MainImage.Width = this.defaultImageWidth;
                this.MainImage.Height = this.defaultImageWidth * (this.MainImage.Source.Height / this.MainImage.Source.Width);

                // Unlock SameAsOriginalImageSize
                this.unlockSameAsOriginalImageSize();


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
            if ((page != this.currentPage) && (page > 0) && (this.innerJbigData != null) && (page <= this.innerJbigData.Pages))
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
            if ((this.currentPage > 0) && (this.innerJbigData != null) && (this.currentPage < this.innerJbigData.Pages))
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
            if ((this.currentPage > 1) && (this.innerJbigData != null) && (this.currentPage <= this.innerJbigData.Pages))
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


        // Unlock SameAsOriginalImageSize
        private void unlockSameAsOriginalImageSize()
        {
            // Set current status of this.sameAsOriginalImageSize
            this.sameAsOriginalImageSize = false;

            // Set Image Stretch
            this.MainImage.Stretch = Stretch.Uniform;

            // update status
            this.updateStatus();
        }


        // Zoom In/Out function
        private void zoom(double zoomRatio) { this.zoom(zoomRatio, 0, 0); }

        private void zoom(double zoomRatio, double x, double y)
        {
            if (((this.MainImage.ActualWidth * zoomRatio) > this.minImageWidth) && ((this.MainImage.ActualWidth * zoomRatio) < this.maxImageWidth))
            {
                // Unlock this.sameAsOriginalImageSize
                this.unlockSameAsOriginalImageSize();


                // Store start position
                double offsetX = this.MainView.HorizontalOffset;
                double offsetY = this.MainView.VerticalOffset;


                // Zoom In/Out
                this.MainImage.Width = this.MainImage.ActualWidth * zoomRatio;
                this.MainImage.Height = this.MainImage.ActualHeight * zoomRatio;


                // Restore position
                this.MainView.ScrollToHorizontalOffset(offsetX * zoomRatio);
                this.MainView.ScrollToVerticalOffset(offsetY * zoomRatio);
            }


            // update status
            this.updateStatus();
        }


        // Zoom In function (by MenuItem Click)
        private void zoomIn()
        {
            this.zoom(1.2);
        }

        // Zoom Out function (by MenuItem Click)
        private void zoomOut()
        {
            this.zoom(0.8);
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


            // Previous Page
            if (this.currentPage > 1)
            {
                this.PreviousPageMenuItem.IsEnabled = true;
                this.PreviousPageContextMenuItem.IsEnabled = true;

                this.LeftButton.IsEnabled = true;
                this.LeftArrowPolygon.Fill = new SolidColorBrush(Colors.DimGray);
            }
            else
            {
                this.PreviousPageMenuItem.IsEnabled = false;
                this.PreviousPageContextMenuItem.IsEnabled = false;

                this.LeftButton.IsEnabled = false;
                this.LeftArrowPolygon.Fill = new SolidColorBrush(Colors.LightGray);
            }

            // Next Page
            if ((this.currentPage > 0) && (this.innerJbigData != null) && (this.currentPage < this.innerJbigData.Pages))
            {
                this.NextPageMenuItem.IsEnabled = true;
                this.NextPageContextMenuItem.IsEnabled = true;

                this.RightButton.IsEnabled = true;
                this.RightArrowPolygon.Fill = new SolidColorBrush(Colors.DimGray);
            }
            else
            {
                this.NextPageMenuItem.IsEnabled = false;
                this.NextPageContextMenuItem.IsEnabled = false;

                this.RightButton.IsEnabled = false;
                this.RightArrowPolygon.Fill = new SolidColorBrush(Colors.LightGray);
            }


            // Menu & Context Menu
            if (this.currentPage > 0)
            {
                // Menu
                this.SaveAsMenuItem.IsEnabled = true;
                this.CloseMenuItem.IsEnabled = true;
                this.ZoomInMenuItem.IsEnabled = true;
                this.ZoomOutMenuItem.IsEnabled = true;
                this.SameAsOriginalImageSizeMenuItem.IsEnabled = true;
                this.RemoveUnexpectedDataMenuItem.IsEnabled = false;

                // Context Menu
                this.SaveAsContextMenuItem.IsEnabled = true;
                this.CloseContextMenuItem.IsEnabled = true;
                this.ZoomInContextMenuItem.IsEnabled = true;
                this.ZoomOutContextMenuItem.IsEnabled = true;
                this.SameAsOriginalImageSizeContextMenuItem.IsEnabled = true;
                this.RemoveUnexpectedDataContextMenuItem.IsEnabled = false;
            }
            else
            {
                // Menu
                this.SaveAsMenuItem.IsEnabled = false;
                this.CloseMenuItem.IsEnabled = false;
                this.ZoomInMenuItem.IsEnabled = false;
                this.ZoomOutMenuItem.IsEnabled = false;
                this.SameAsOriginalImageSizeMenuItem.IsEnabled = false;
                this.RemoveUnexpectedDataMenuItem.IsEnabled = true;

                // Context Menu
                this.SaveAsContextMenuItem.IsEnabled = false;
                this.CloseContextMenuItem.IsEnabled = false;
                this.ZoomInContextMenuItem.IsEnabled = false;
                this.ZoomOutContextMenuItem.IsEnabled = false;
                this.SameAsOriginalImageSizeContextMenuItem.IsEnabled = false;
                this.RemoveUnexpectedDataContextMenuItem.IsEnabled = true;
            }


            // RemoveUnexpectedData (Menu and ContextMenu)
            this.RemoveUnexpectedDataMenuItem.IsChecked = Properties.Settings.Default.RemoveUnexpectedDataStatus;
            this.RemoveUnexpectedDataContextMenuItem.IsChecked = Properties.Settings.Default.RemoveUnexpectedDataStatus;

            // this.sameAsOriginalImageSize
            this.SameAsOriginalImageSizeMenuItem.IsChecked = this.sameAsOriginalImageSize;
            this.SameAsOriginalImageSizeContextMenuItem.IsChecked = this.sameAsOriginalImageSize;
        }
    }
}

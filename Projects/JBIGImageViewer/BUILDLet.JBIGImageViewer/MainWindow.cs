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

using BUILDLet.JBIG;


namespace BUILDLet.JBIGImageViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double minImageWidth = 100;
        private readonly double maxImageWidth = 200 * 500;
        private readonly double defaultImageWidth = 390;
        private readonly double defaultZoomInRatio = 1.2;
        private readonly double defaultZoomOutRatio = 0.8;

        private bool image_is_handling = false;
        private bool same_as_original = false;


        // Source JBIG Image File Name
        protected string SourceJBIGImageFileName { get; set; } = string.Empty;

        // PBM Image File Name(s)
        protected Dictionary<int, string> PBMImageFileNames { get; set; } = new Dictionary<int, string>();

        // Bitmap Image File Name(s)
        protected Dictionary<int, string> BitmapImageFileNames { get; set; } = new Dictionary<int, string>();

        // JBIG Image Fragments
        protected JBIGImageFragments JBIGImageFragments { get; set; } = null;

        // Current Page Number
        protected int CurrentPage { get; set; } = 0;

        // Image is already open.
        protected bool ImageIsOpen
        {
            get { return (this.CurrentPage > 0); }
        }

        // Image is handling
        protected bool ImageIsHandling
        {
            get { return this.image_is_handling; }
            set
            {
                // Set Image Handling Mode and Change Mouse Cursor
                Mouse.OverrideCursor = (this.image_is_handling = value) ? Cursors.Hand : null;
            }
        }

        // Current Status of "Same As Original Image Size"
        protected bool SameAsOriginalImageSizeStatus
        {
            get { return this.same_as_original; }
            set
            {
                // Update Image Stretch
                this.MainImage.Stretch = (this.same_as_original = value) ? Stretch.None : Stretch.Uniform;

                // Same As Original Image Size (= TRUE)
                if (this.same_as_original)
                {
                    this.MainImage.Width = this.MainImage.Source.Width;
                    this.MainImage.Height = this.MainImage.Source.Height;
                }
            }
        }

        // Zoom In Function (Menu & Context Menu) is Enable, OR NOT
        protected bool ZoomInFunctionIsAvailable
        {
            get { return ((this.MainImage.Width * defaultZoomInRatio) < this.maxImageWidth); }
        }

        // Zoom Out Function (Menu & Context Menu) is Enable, OR NOT
        protected bool ZoomOutFunctionIsAvailable
        {
            get { return ((this.MainImage.Width * defaultZoomOutRatio) > this.minImageWidth); }
        }


        // Show Image
        private void showImage(int page)
        {
            // Validate
            if (this.JBIGImageFragments != null)
            {
                if ((page < 1) || (page > this.JBIGImageFragments.Pages)) { throw new ArgumentException(); }
            }

            // Change Cursor
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                // Check Page Number of Bitmap File
                if (!this.BitmapImageFileNames.Keys.Contains(page - 1))
                {
                    // Bitmap File is NOT yet Converted from JBIG.

                    // set source JBIG Image file name of RAW JBIG Image or JBIG Image Fragments
                    string src_jbg_filename = (this.JBIGImageFragments == null) ? this.SourceJBIGImageFileName : this.JBIGImageFragments.FileNames[page - 1];

                    // Temporary PBM File Name
                    string pbm_filename = System.IO.Path.GetTempFileName();

                    // Convert JBIG Image File -> PBM Image File
                    if (JBIGImageConverter.Convert(src_jbg_filename, ref pbm_filename))
                    {
                        // Add File Name of created Bitmap File
                        this.PBMImageFileNames.Add(page - 1, pbm_filename);
                    }
                    else { throw new ApplicationException(); }


                    // Temporary Bitmap File Name
                    string bmp_filename = System.IO.Path.GetTempFileName();

                    // Convert PBM Image File -> Bitmap Image File
                    if (JBIGImageConverter.Convert(pbm_filename, ref bmp_filename, JBIGImageConverterInputImageFormat.Pbm, JBIGImageConverterOutputImageFormat.Bmp))
                    {
                        // Add File Name of created Bitmap File
                        this.BitmapImageFileNames.Add(page - 1, bmp_filename);
                    }
                    else { throw new ApplicationException(); }
                }


                // new Btimap Image
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(this.BitmapImageFileNames[page - 1]);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                // Set ImageSource
                this.MainImage.Source = image;
                this.MainImage.MinWidth = this.minImageWidth;
                this.MainImage.MaxWidth = this.maxImageWidth;

                // Set default Image Width
                this.MainImage.Width = this.defaultImageWidth;
                this.MainImage.Height = this.defaultImageWidth * (this.MainImage.Source.Height / this.MainImage.Source.Width);


                // Clear this.SameAsOriginalImageSizeStatus
                this.SameAsOriginalImageSizeStatus = false;

                // set current page number
                this.CurrentPage = page;

                // update status
                this.updateStatus();
            }
            catch (Exception) { throw; }
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
                // close Image if already opened.
                if (this.ImageIsOpen) { this.closeImage(); }

                // store Source JBIG Image File Name
                this.SourceJBIGImageFileName = filename;


                // create JBIG Image Fragments
                if (this.RemoveUnexpectedDataMenuItem.IsChecked)
                {
                    this.JBIGImageFragments = new JBIGImageFragments(this.SourceJBIGImageFileName);
                }


                // show Bitmap Image
                this.showImage(1);
            }
            catch (Exception) { throw; }
        }

        
        // Show Next Page
        private void showNextPage()
        {
            if (this.ImageIsOpen && (this.JBIGImageFragments != null) && (this.CurrentPage < this.JBIGImageFragments.Pages))
            {
                // Clear ImageSource
                this.MainImage.Source = null;

                try
                {
                    // Show Bitmap Image
                    // and Increment current page number
                    this.showImage(++this.CurrentPage);
                }
                catch (Exception) { throw; }
            }
        }


        // Show Previous Page
        private void showPreviousPage()
        {
            if (this.ImageIsOpen && (this.JBIGImageFragments != null) && (this.CurrentPage > 1) && (this.CurrentPage <= this.JBIGImageFragments.Pages))
            {
                // Clear ImageSource
                this.MainImage.Source = null;

                try
                {
                    // Show Bitmap Image
                    // and Increment current page number
                    this.showImage(--this.CurrentPage);
                }
                catch (Exception) { throw; }
            }
        }


        // Close Image
        private void closeImage()
        {
            // only if the image is already opened.
            if (this.ImageIsOpen)
            {
                // Clear ImageSource
                this.MainImage.Source = null;


                try
                {
                    // Clear JBIG Fragments
                    if (this.JBIGImageFragments != null)
                    {
                        this.JBIGImageFragments.Dispose();
                        this.JBIGImageFragments = null;
                    }

                    // Delete PBM & Bitmap Image File(s)
                    // (Clear PBMImageFileNames & BitmapImageFileNames)
                    foreach (var filenames in new Dictionary<int, string>[] { PBMImageFileNames, BitmapImageFileNames })
                    {
                        if (filenames != null)
                        {
                            foreach (var filename in filenames.Values)
                            {
                                if (File.Exists(filename))
                                {
                                    // Delete PBM or Bitmap Image File
                                    File.Delete(filename);
                                }
                            }

                            // Clear PBM or Bitmap Image File Name(s)
                            filenames.Clear();
                        }
                    }
                }
                catch (Exception) { throw; }


                // Clear current page number
                this.CurrentPage = -1;

                // update status
                this.updateStatus();
            }
        }


        // Save as Bitmap Image
        private void saveImage(string filename, int filterIndex)
        {
            try
            {
                JBIGImageConverterOutputImageFormat outputFormat = SaveFileDialogFilter.GetOutputImageFormat(filterIndex);

                switch (outputFormat)
                {
                    case JBIGImageConverterOutputImageFormat.Pbm:
                        throw new ApplicationException();

                    case JBIGImageConverterOutputImageFormat.Bmp:

                        // Copy Bitmap Image File
                        File.Copy(this.BitmapImageFileNames[this.CurrentPage - 1], filename, true);
                        break;

                    default:

                        // Save PBM Image File -> Other Image Format File
                        JBIGImageConverter.Convert(this.PBMImageFileNames[this.CurrentPage - 1], ref filename, JBIGImageConverterInputImageFormat.Pbm, outputFormat);
                        break;
                }
            }
            catch (Exception) { throw; }
        }


        // Update Page Number TextBox
        private void updatePageNumberTextBox()
        {
            try
            {
                int page;
                if (int.TryParse(this.PageNumberTextBox.Text, out page) && (this.PageNumberTextBox.Text != this.CurrentPage.ToString()))
                {
                    if ((this.JBIGImageFragments != null) && (page != this.CurrentPage) && (page > 0) && (page <= this.JBIGImageFragments.Pages))
                    {
                        // Clear ImageSource
                        this.MainImage.Source = null;

                        // Show Bitmap Image
                        // and Increment current page number
                        this.showImage(page);
                    }
                }

                // Update (Modify) Page Number
                this.PageNumberTextBox.Text = this.CurrentPage.ToString();
            }
            catch (Exception) { throw; }
        }


        // Zoom In function (by MenuItem Click)
        private void zoomIn() { this.zoom(defaultZoomInRatio, -1, -1); }

        // Zoom Out function (by MenuItem Click)
        private void zoomOut() { this.zoom(defaultZoomOutRatio, -1, -1); }

        // Zoom In/Out function
        private void zoom(double zoomRatio, double x, double y)
        {
            if (((this.MainImage.Width * zoomRatio) > this.minImageWidth) && ((this.MainImage.Width * zoomRatio) < this.maxImageWidth))
            {
                // Clear this.SameAsOriginalImageSizeStatus
                this.SameAsOriginalImageSizeStatus = false;


                // Store start position
                double startOffsetX = this.MainView.HorizontalOffset;
                double startOffsetY = this.MainView.VerticalOffset;
                double offsetX = ((x >= 0) ? x : (this.MainView.ViewportWidth / 2));
                double offsetY = ((y >= 0) ? y : (this.MainView.ViewportHeight / 2));


                // Zoom In/Out
                this.MainImage.Width = this.MainImage.Width * zoomRatio;
                this.MainImage.Height = this.MainImage.Height * zoomRatio;


                double targetOffsetX = ((startOffsetX + offsetX) * zoomRatio) - offsetX;
                double targetOffsetY = ((startOffsetY + offsetY) * zoomRatio) - offsetY;
                if (targetOffsetX < 0) { targetOffsetX = 0; }
                if (targetOffsetY < 0) { targetOffsetY = 0; }

                // Restore position
                this.MainView.ScrollToHorizontalOffset(targetOffsetX);
                this.MainView.ScrollToVerticalOffset(targetOffsetY);
            }

            // update status
            this.updateStatus();
        }


        // Update Status
        private void updateStatus()
        {
            // Page Number
            if (this.ImageIsOpen)
            {
                this.PageNumberTextBox.Text = this.CurrentPage.ToString();
            }
            else
            {
                this.PageNumberTextBox.Text = string.Empty;
            }


            // Previous Page
            if (this.CurrentPage > 1)
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
            if (this.ImageIsOpen && (this.JBIGImageFragments != null) && (this.CurrentPage < this.JBIGImageFragments.Pages))
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


            // Image is Open, OR NOT (Menu & Context Menu)
            if (this.ImageIsOpen)
            {
                // Menu
                this.SaveAsMenuItem.IsEnabled = true;
                this.CloseMenuItem.IsEnabled = true;
                this.SameAsOriginalImageSizeMenuItem.IsEnabled = true;
                this.RemoveUnexpectedDataMenuItem.IsEnabled = false;

                this.ZoomInMenuItem.IsEnabled = this.ZoomInFunctionIsAvailable;
                this.ZoomOutMenuItem.IsEnabled = this.ZoomOutFunctionIsAvailable;

                // Context Menu
                this.SaveAsContextMenuItem.IsEnabled = true;
                this.CloseContextMenuItem.IsEnabled = true;
                this.SameAsOriginalImageSizeContextMenuItem.IsEnabled = true;
                this.RemoveUnexpectedDataContextMenuItem.IsEnabled = false;

                this.ZoomInContextMenuItem.IsEnabled = this.ZoomInFunctionIsAvailable;
                this.ZoomOutContextMenuItem.IsEnabled = this.ZoomOutFunctionIsAvailable;
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

            // SameAsOriginalImageSizeStatus
            this.SameAsOriginalImageSizeMenuItem.IsChecked = this.SameAsOriginalImageSizeStatus;
            this.SameAsOriginalImageSizeContextMenuItem.IsChecked = this.SameAsOriginalImageSizeStatus;
        }
    }
}

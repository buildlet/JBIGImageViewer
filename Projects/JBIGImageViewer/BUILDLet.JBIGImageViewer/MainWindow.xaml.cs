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
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;

using BUILDLet.Utilities.WPF;
using BUILDLet.JBIG;


namespace BUILDLet.JBIGImageViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // Scroll Start Position
        private double scrollStartPositionX = 0;
        private double scrollStartPositionY = 0;


        // Constructor
        public MainWindow()
        {
            InitializeComponent();


            if (!string.IsNullOrEmpty(App.inputJBIGImageFileName))
            {
                try
                {
                    // load Input JBIG Image File
                    this.openImage(App.inputJBIGImageFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


            // Load status of RemoveUnexpectedDataMenuItem.IsChecked
            this.RemoveUnexpectedDataMenuItem.IsChecked = Properties.Settings.Default.RemoveUnexpectedDataStatus;

            // update status
            this.updateStatus();
        }


        // (Window) KeyDown Event Handler
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // [Esc]: Close
            // V1.1.2.0  Commented out
            // if (e.Key == Key.Escape) { this.Close(); }


            // [Ctrl] + any key
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // [Ctrl] + [O]: Open
                if (e.Key == Key.O) { this.OpenMenuItem_Click(this, null); }

                // Image is Open.
                if (this.CurrentPage > 0)
                {
                    // [Ctrl] + [+]: Zoom In
                    if (e.Key == Key.Add) { this.zoomIn(); }

                    // [Ctrl] + [-]: Zoom Out
                    if (e.Key == Key.Subtract) { this.zoomOut(); }

                    // [Ctrl] + [S]: SaveAs
                    if (e.Key == Key.S) { this.SaveAsMenuItem_Click(this, null); }

                    // [Ctrl] + [C]: Close
                    if (e.Key == Key.C) { this.CloseMenuItem_Click(this, null); }

                    // [Ctrl] + [Right]: Next Page
                    if (e.Key == Key.Right) { this.NextPageMenuItem_Click(this, null); }

                    // [Ctrl] + [Left]: Previous Page
                    if (e.Key == Key.Left) { this.PreviousPageMenuItem_Click(this, null); }
                }
            }
        }


        // (Image) MouseLeftButtonDown Event Handler
        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set Image Handling Mode
            this.ImageIsHandling = true;

            // Store Scroll Start Position
            this.scrollStartPositionX =e.GetPosition(this.MainView).X;
            this.scrollStartPositionY = e.GetPosition(this.MainView).Y;
        }


        // (MainView) MouseLeftButtonUp Event Handler
        private void MainView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Clear Image Handling Mode
                this.ImageIsHandling = false;
            }
        }


        // (MainView) MouseLeave Event Handler
        private void MainView_MouseLeave(object sender, MouseEventArgs e)
        {
            // Clear Image Handling Mode
            this.ImageIsHandling = false;
        }


        // (MainView) MouseMove Event Handler
        private void MainView_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ImageIsHandling)
            {
                // Scroll
                this.MainView.ScrollToHorizontalOffset(this.MainView.ContentHorizontalOffset + (this.scrollStartPositionX - e.GetPosition(this.MainView).X));
                this.MainView.ScrollToVerticalOffset(this.MainView.ContentVerticalOffset + (this.scrollStartPositionY - e.GetPosition(this.MainView).Y));

                // Update Scroll Start Position
                this.scrollStartPositionX = e.GetPosition(this.MainView).X;
                this.scrollStartPositionY = e.GetPosition(this.MainView).Y;
            }
        }


        // (Image) MouseWheel Event Handler
        private void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && this.ImageIsOpen)
            {
                // Set Zoom Ratio and Current Mouse Position
                double ratio = (e.Delta + this.MainView.ViewportHeight) / this.MainView.ViewportHeight;
                double x = e.GetPosition(this.MainView).X;
                double y = e.GetPosition(this.MainView).Y;

                // Zoom In/Out
                this.zoom(ratio, x, y);
            }
        }


        // (Menu) File Open
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter =
                    "JBIG Image Files (*.jbg;*.jbig)|*.jbg;*.jbig|" +
                    "All Files (*.*)|*.*";
                dialog.Multiselect = false;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.LastOpenDirectory) ?
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) : Properties.Settings.Default.LastOpenDirectory;


                // Show Dialog
                if (dialog.ShowDialog() == true)
                {
                    // Open Bitmap Image File
                    this.openImage(dialog.FileName);

                    // Set Last Open Directory
                    Properties.Settings.Default.LastOpenDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) Save As
        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();

                dialog.Filter = SaveFileDialogFilter.ToString();
                dialog.FileName = System.IO.Path.GetFileNameWithoutExtension(this.SourceJBIGImageFileName);
                dialog.OverwritePrompt = true;
                dialog.InitialDirectory = string.IsNullOrEmpty(Properties.Settings.Default.LastSaveDirectory) ?
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) :
                    dialog.InitialDirectory = Properties.Settings.Default.LastSaveDirectory;


                // Show Dialog
                if (dialog.ShowDialog() == true)
                {
                    // Save (Copy) Image as Bitmap File
                    this.saveImage(dialog.FileName, dialog.FilterIndex);

                    // Set Last Save Directory
                    Properties.Settings.Default.LastSaveDirectory = System.IO.Path.GetDirectoryName(dialog.FileName);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) Close
        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Close Image
                this.closeImage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) Exit
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Exit
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) Show Version Information
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bitmap logo = new System.Drawing.Icon(
                    Properties.Resources.BUILDLet_JBIGImageViewer, 
                    VersionInformationWindow.LogoImageWidth, 
                    VersionInformationWindow.LogoImageHeight).ToBitmap();

                // Show Dialog
                new VersionInformationWindow(logo, ImageFormat.Png, this).ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) Show Next Page
        private void NextPageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show Next Page
                this.showNextPage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) Show Previous Page
        private void PreviousPageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show Previous Page
                this.showPreviousPage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Menu) RemoveUnexpectedData Menu is Checked or Unchecked.
        private void RemoveUnexpectedDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Store current status of RemoveUnexpectedDataMenuItem.IsChecked or RemoveUnexpectedDataContextMenuItem.IsChecked
            Properties.Settings.Default.RemoveUnexpectedDataStatus = ((MenuItem)e.OriginalSource).IsChecked;

            // update status (for synchronize Context Menu)
            this.updateStatus();
        }


        // (Menu) SameAsOriginalImageSize Menu is Checked or Unchecked.
        private void SameAsOriginalImageSizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Update current status of this.SameAsOriginalImageSizeStatus
            this.SameAsOriginalImageSizeStatus = ((MenuItem)e.OriginalSource).IsChecked;

            // update status
            this.updateStatus();
        }


        // (Menu) ZoomInMenuItem Click Event Handler
        private void ZoomInMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.zoomIn();
        }


        // (Menu) ZoomOutMenuItem Click Event Handler
        private void ZoomOutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.zoomOut();
        }


        // (Button) Left Button Click
        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show Previous Page
                this.showPreviousPage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Button) Right Button Click
        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show Next Page
                this.showNextPage();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (TextBox) Page Number is updated.
        private void PageNumberTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    // Update Page Number TextBox
                    this.updatePageNumberTextBox();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
            }


            if ((Keyboard.Modifiers == ModifierKeys.Control) && this.ImageIsOpen)
            {
                // Show Previous Page
                if (e.Key == Key.Left) { this.showPreviousPage(); }

                // Show Next Page
                if (e.Key == Key.Right) { this.showNextPage(); }
            }
        }

        // (TextBox) Lost Focus
        private void PageNumberTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update Page Number TextBox
                this.updatePageNumberTextBox();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Window) Drag and Drop
        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

                if ((files != null) && (files.Length > 0))
                {
                    // Open Bitmap Image File
                    this.openImage(files[0]);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }


        // (Window) Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Close Image
                this.closeImage();

                // Save current settings.
                Properties.Settings.Default.Save();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}

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
        public MainWindow()
        {
            InitializeComponent();


            if (!string.IsNullOrEmpty(App.initialInputFileName))
            {
                try
                {
                    // Open Bitmap Image File
                    this.openImage(App.initialInputFileName);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
            }


            // Load status of RemoveUnexpectedDataMenuItem.IsChecked
            this.RemoveUnexpectedDataMenuItem.IsChecked = Properties.Settings.Default.RemoveUnexpectedDataStatus;

            // update status
            this.updateStatus();
        }


        // KeyDown Event Handler
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) { this.Close(); }
        }


        // (Menu) File Open
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "JBIG Image Files (*.jbg, *.jbig)|*.jbg;*.jbig|All Files (*.*)|*.*";
                dialog.Multiselect = false;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;

                if (string.IsNullOrEmpty(Properties.Settings.Default.LastOpenDirectory))
                {
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);    
                }
                else
                {
                    dialog.InitialDirectory = Properties.Settings.Default.LastOpenDirectory;
                }

                
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
                dialog.Filter = "Bitmap Image Files (*.bmp)|*.bmp|All Files (*.*)|*.*";
                dialog.FileName = System.IO.Path.GetFileNameWithoutExtension(this.sourceFileName) + ".bmp";
                dialog.OverwritePrompt = true;

                if (string.IsNullOrEmpty(Properties.Settings.Default.LastSaveDirectory))
                {
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }
                else
                {
                    dialog.InitialDirectory = Properties.Settings.Default.LastSaveDirectory;
                }


                // Show Dialog
                if (dialog.ShowDialog() == true)
                {
                    // Save (Copy) Image as Bitmap File
                    this.saveImage(dialog.FileName);

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
                    Properties.Resources.BUILDLet_JbigViewer, 
                    VersionInformationWindow.LogoImageWidth, 
                    VersionInformationWindow.LogoImageHeight).ToBitmap();

                // Show Dialog
                new VersionInformationWindow(logo, ImageFormat.Bmp).ShowDialog();
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
            // Store current status of RemoveUnexpectedDataMenuItem.IsChecked
            Properties.Settings.Default.RemoveUnexpectedDataStatus = this.RemoveUnexpectedDataMenuItem.IsChecked;
        }


        // (Menu) SameAsOriginalImageSize Menu is Checked or Unchecked.
        private void SameAsOriginalImageSizeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.SameAsOriginalImageSizeMenuItem.IsChecked)
            {
                this.MainView.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                this.MainImage.Stretch = Stretch.None;
            }
            else
            {
                this.MainView.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.MainImage.Stretch = Stretch.UniformToFill;
            }            
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
        private void PageNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int page;
            if (int.TryParse(this.PageNumberTextBox.Text, out page) && (this.PageNumberTextBox.Text != this.currentPage.ToString()))
            {
                try
                {
                    // Show Page
                    this.showPage(page);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error); }
            }
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

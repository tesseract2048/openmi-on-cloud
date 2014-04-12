﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileClient.FileService;
using FileClient.WindowEffect;
using OOC.Util;

namespace FileClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Logger _logger = new Logger("OOC.GUI.FileClient.log");
        private readonly FileServiceClient Client = new FileServiceClient();
        public string CurrentPath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            //init here
            Height = 675;
            Width = 1200;
        }

        public void NavigateTo(string path)
        {
            try
            {
                FileDescription[] fileDescriptions = Client.List(path);
            }
            catch (FaultException e)
            {
                _logger.Warn(e.Message);

            }
        }

        private void menu_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            menu.Visibility = Visibility.Visible;
            //statusDock.Visibility = Visibility.Visible;
        }

        private void menu_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            menu.Visibility = Visibility.Collapsed;
            //statusDock.Visibility = Visibility.Collapsed;
        }

        private void menu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            menu.Visibility = Visibility.Visible;
            //statusDock.Visibility = Visibility.Visible;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                menu.Visibility = Visibility.Visible;
                //statusDock.Visibility = Visibility.Visible;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyGlassEffect(addressBorder);
        }

        private void ApplyGlassEffect(Border border)
        {
            const int borderWidth = 1 + 2;
            const int dpi = 96;

            try
            {
                Application.Current.MainWindow.Background = Brushes.Transparent;
                //menu.Background = Brushes.Transparent;
                // Obtain the window handle for WPF application
                IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                if (mainWindowSrc != null)
                    if (mainWindowSrc.CompositionTarget != null)
                        mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                // Get System Dpi
                System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                float DesktopDpiX = desktop.DpiX;
                float DesktopDpiY = desktop.DpiY;

                // Set Margins
                NonClientRegionAPI.MARGINS margins = new NonClientRegionAPI.MARGINS
                    {
                        cxLeftWidth = Convert.ToInt32(borderWidth * (DesktopDpiX / dpi)),
                        cxRightWidth = Convert.ToInt32(borderWidth * (DesktopDpiX / dpi)),
                        cyTopHeight = Convert.ToInt32(((int)border.ActualHeight + borderWidth + 5) * (DesktopDpiY / dpi)),
                        cyBottomHeight = Convert.ToInt32(borderWidth * (DesktopDpiY / dpi))
                    };

                // Extend glass frame into client area
                // Note that the default desktop Dpi is 96dpi. The  margins are
                // adjusted for the system Dpi.

                if (mainWindowSrc != null)
                {
                    int hr = NonClientRegionAPI.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                    //
                    if (hr < 0)
                    {
                        //DwmExtendFrameIntoClientArea Failed
                    }
                }
            }
            // If not Vista, paint background white.
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }
    }
}

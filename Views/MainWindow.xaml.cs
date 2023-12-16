﻿using HACS.WPF.Behaviors;
using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Media;

namespace HACS.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static class NativeMethods
        {
            // Import SetThreadExecutionState Win32 API and necessary flags
            [DllImport("kernel32.dll")]
            public static extern uint SetThreadExecutionState(uint esFlags);
            public const uint ES_CONTINUOUS = 0x80000000;
            public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        }

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale), typeof(double), typeof(MainWindow));

        public double Scale { get => (double)GetValue(ScaleProperty); set => SetValue(ScaleProperty, value); }

        public bool IsClosed { get; protected set; }

        public ControlPanel ControlPanel { get; protected set; }
        protected Core.HacsBase Hacs => ControlPanel?.Bridge?.GetHacs();

        Window sampleManager;
        Window processSequenceEditor;
        Window deviceSettings;
        Window preferences;

        public MainWindow()
        {
            InitializeComponent();

            var preventSleep = NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED;
            if (0 == NativeMethods.SetThreadExecutionState(preventSleep))
            {
                MessageBox.Show("Call to SetThreadExecutionState failed unexpectedly.",
                    Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public virtual void LoadControlPanel(ControlPanel controlPanel)
        {
            ControlPanel = controlPanel;
            Loaded += controlPanel.UILoaded;
            ContentRendered += controlPanel.UIShown;
            //Closing += controlPanel.UIClosing;

            //MainContent.Children.Add(controlPanel);
            MainContent.Child = controlPanel;

            Title = Hacs?.Name ?? "Aeon Hacs";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Core.Hacs.Stopped)
                base.OnClosing(e);
            else if (!Core.Hacs.Stopping)
            {
                e.Cancel = true;
                Task.Run(() => ControlPanel.UIClosing(this, e)).ContinueWith(t => Dispatcher.Invoke(Close));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
            NativeMethods.SetThreadExecutionState(NativeMethods.ES_CONTINUOUS);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Left)
                FocusManager.SetFocusedElement(this, this);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            var hit = e.Source as Visual;

            string helpText = "";

            while (hit != null)
            {
                if (AutomationProperties.GetHelpText(hit) is string text && !string.IsNullOrWhiteSpace(text))
                {
                    helpText = text;
                    break;
                }
                else
                    hit = VisualTreeHelper.GetParent(hit) as Visual;
            }

            HelpText.Text = helpText;

            base.OnPreviewMouseMove(e);
        }

        //TODO static method in some? class or is there a better way?
        void MoveWindowToMouse(Window window)
        {
            var mouse = PointToScreen(Mouse.GetPosition(this));
            window.Top = mouse.Y;
            window.Left = mouse.X;
        }

        void ShowBackPanel()
        {
        }

        /// <summary>
        /// Show a Sample Manager window, from which a sample can be added, edited, or deleted
        /// </summary>
        void ShowSampleManager()
        {
            if (sampleManager == null)
            {
                var w = sampleManager = new Window();
                MoveWindowToMouse(sampleManager);
                w.Title = Title + " Samples";
                var sm = new SampleManager();
                w.Content = sm;
                w.SizeToContent = SizeToContent.WidthAndHeight;
                sm.SampleList.SizeChanged += (sender, e) =>
                    w.SizeToContent = SizeToContent.WidthAndHeight;
                w.Closed += (sender, e) => 
                    sampleManager = null;
                w.ResizeMode = ResizeMode.NoResize;
                w.Show();
            }
            else
            {
                if (sampleManager.WindowState == WindowState.Minimized)
                    sampleManager.WindowState = WindowState.Normal;
                sampleManager.Activate();
            }
        }


        void ShowProcessSequenceEditor()
        {
            if (processSequenceEditor == null)
            {
                var w = processSequenceEditor = new Window();
                MoveWindowToMouse(processSequenceEditor);
                w.Title = Title + " Process Sequences";
                w.Content = new ProcessSequenceEditor(Hacs as Components.ProcessManager);
                w.SizeToContent = SizeToContent.WidthAndHeight;
                w.ContentRendered += (sender, e) =>
                {
                    w.ClearValue(SizeToContentProperty);
                    w.MinWidth = w.ActualWidth;
                    w.MinHeight = w.ActualHeight;
                };
                w.Closed += (sender, e) => processSequenceEditor = null;
                w.Show();
            }
            else
            {
                if (processSequenceEditor.WindowState == WindowState.Minimized)
                    processSequenceEditor.WindowState = WindowState.Normal;
                processSequenceEditor.Activate();
            }
        }


        void ShowSettings()
        {
            if (Hacs == null) return;
            if (deviceSettings == null)
            {
                var w = deviceSettings = new Window();
                MoveWindowToMouse(deviceSettings);
                w.Title = Title + " Settings";
                w.Content = new SettingsPage(Hacs);
                w.SizeToContent = SizeToContent.WidthAndHeight;
                w.MaxHeight = 0.7 * SystemParameters.PrimaryScreenHeight;
                w.ContentRendered += (sender, e) =>
                {
                    deviceSettings.SizeToContent=SizeToContent.Width;
                    deviceSettings.ClearValue(MaxHeightProperty);
                };
                w.Closed += (sender, e) => deviceSettings = null;
                Interaction.GetBehaviors(w).Add(new SettingsWindowBehavior());
                w.Show();
            }
            else
            {
                if (deviceSettings.WindowState == WindowState.Minimized)
                    deviceSettings.WindowState = WindowState.Normal;
                deviceSettings.Activate();
            }
        }

        void ShowPreferencesWindow()
        {
            if (preferences == null)
            {
                var w = preferences = new Preferences();
                MoveWindowToMouse(w);
                w.Title = Title + " Preferences";
                w.SizeToContent = SizeToContent.WidthAndHeight;
                //w.ContentRendered += (sender, e) =>
                //	w.ClearValue(SizeToContentProperty);
                w.Closed += (sender, e) => preferences = null;
                w.Show();
            }
            else
            {
                if (preferences.WindowState == WindowState.Minimized)
                    preferences.WindowState = WindowState.Normal;
                preferences.Activate();
            }
        }

        private void Samples_Click(object sender, RoutedEventArgs e) =>
            ShowSampleManager();

        private void ProcessSequences_Click(object sender, RoutedEventArgs e) =>
            ShowProcessSequenceEditor();

        private void Settings_Click(object sender, RoutedEventArgs e) =>
            ShowSettings();
        private void Preferences_Click(object sender, RoutedEventArgs e) =>
            ShowPreferencesWindow();


    }
}

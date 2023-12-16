﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HACS.WPF.Views
{
    /// <summary>
    /// Interaction logic for FTC.xaml
    /// </summary>
    public class FTC : View
    {
        public static readonly DependencyProperty FillProperty = Shape.FillProperty.AddOwner(typeof(FTC));

        public Brush Fill { get => GetValue(FillProperty) as Brush; set => SetValue(FillProperty, value); }

        static FTC()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FTC), new FrameworkPropertyMetadata(typeof(FTC)));
        }
    }
}

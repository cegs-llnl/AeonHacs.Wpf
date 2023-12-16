﻿using HACS.Components;
using HACS.WPF.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace HACS.WPF.ViewModels
{
	public class InletPort : LinePort
	{
		[Browsable(false)]
		public new Components.IInletPort Component
		{
			get => base.Component as Components.IInletPort;
			protected set => base.Component = value;
		}

		public HACS.Core.InletPortType PortType { get => Component.PortType; set => Component.PortType = value; }

		public ViewModel QuartzFurnace
		{	
			get => GetFromModel(Component?.QuartzFurnace);
			set { }
		}
		public ViewModel SampleFurnace => GetFromModel(Component?.SampleFurnace);

		public List<ViewModel> PathToFirstTrap
		{
			get => Component?.PathToFirstTrap?.ConvertAll(x => GetFromModel(x));
			set { if (Component != null) Component.PathToFirstTrap = value?.ConvertAll(x => x.Component as Components.IValve); }
		}

		public bool NotifySampleFurnaceNeeded { get => Component.NotifySampleFurnaceNeeded; set => Component.NotifySampleFurnaceNeeded = value; }
		public int WarmTemperature { get => Component.WarmTemperature; set => Component.WarmTemperature = value; }

		// TODO Decide context menu for InletPorts

		//void TurnOffFurnaces();

		protected string SampleCaption { get; set; } = "Edit Sample";
		protected override void StartContext()
		{
			ContextStart.Add(new WPF.Context(SampleCaption, dispatch:false));
			base.StartContext();
		}

		public override void Run(string command = "")
		{
			if (command == SampleCaption)
			{
				EditSample();
			}
			base.Run(command);
		}

		void EditSample()
		{
			var w = new Window();
			var se = new SampleEditor(Component);
			w.Content = se;
			w.SizeToContent = SizeToContent.WidthAndHeight;
			w.Show();
		}
	}
}

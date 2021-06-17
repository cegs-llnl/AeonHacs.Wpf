﻿using System.ComponentModel;
namespace HACS.WPF.ViewModels
{
	public class PneumaticValve : SolenoidValve
	{
		[Browsable(false)]
		public new Components.IPneumaticValve Component
		{
			get => base.Component as Components.IPneumaticValve;
			protected set => base.Component = value;
		}

	}
}

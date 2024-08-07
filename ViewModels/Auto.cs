﻿using System.ComponentModel;
namespace AeonHacs.Wpf.ViewModels
{
    public class Auto : Switch
    {
        [Browsable(false)]
        public new Components.IAuto Component
        {
            get => base.Component as Components.IAuto;
            protected set => base.Component = value;
        }
        public double Setpoint => Component.Setpoint;
        public double TargetSetpoint { get => Component.Config.Setpoint; set => Component.Setpoint = value; }
        public double MinimumSetpoint { get => Component.MinimumSetpoint; set => Component.MinimumSetpoint = value; }
        public double MaximumSetpoint { get => Component.MaximumSetpoint; set => Component.MaximumSetpoint = value; }
        //void TurnOn(double setpoint);
    }
}

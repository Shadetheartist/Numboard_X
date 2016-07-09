using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Numboard
{
	public class ValueMenuItem : MenuItem
	{
		public object Value { get; set; }
		public static readonly DependencyProperty ValueProperty =
		  DependencyProperty.Register("Value", typeof(object), typeof(ValueMenuItem));
	}
}

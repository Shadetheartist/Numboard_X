using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Numboard
{
	public static class Helpers
	{
		public static string[] validFormats = new string[] { ".mp3", ".wav", ".aiff", ".m4a" };

		public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj == null) return null;

			for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);

				var result = child as T ?? GetChildOfType<T>(child);
				if (result != null) return result;
			}
			return null;
		}

		public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
		{
			var logicalCollection = new List<T>();
			GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
			return logicalCollection;
		}

		private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
		{
			var children = LogicalTreeHelper.GetChildren(parent);
			foreach (var child in children)
			{
				if (child is DependencyObject)
				{
					var depChild = child as DependencyObject;
					if (child is T)
					{
						logicalCollection.Add(child as T);
					}
					GetLogicalChildCollection(depChild, logicalCollection);
				}
			}
		}

		public static bool isValidFormat(string file)
		{
			return isValidFormat(file, validFormats);
		}

		private static bool isValidFormat(string file, string[] validformats)
		{
			foreach (var format in validformats)
			{
				if (System.IO.Path.GetExtension(file).Equals(format, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		public static void CopyProperties(this object source, object destination)
		{
			// If any this null throw an exception
			if (source == null || destination == null)
				throw new Exception("Source or/and Destination Objects are null");
			// Getting the Types of the objects
			var typeDest = destination.GetType();
			var typeSrc = source.GetType();
			// Collect all the valid properties to map
			var results = from srcProp in typeSrc.GetProperties()
						  let targetProperty = typeDest.GetProperty(srcProp.Name)
						  where srcProp.CanRead
						        && targetProperty != null && targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true).IsPrivate && (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) == 0 && targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)
						  select new { sourceProperty = srcProp, targetProperty = targetProperty };
			//map the properties
			foreach (var props in results)
			{
				props.targetProperty.SetValue(destination, props.sourceProperty.GetValue(source, null), null);
			}
		}
	}


}

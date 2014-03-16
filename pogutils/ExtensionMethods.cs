using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POG.Utils
{
	/// <summary>
	/// http://www.codemeit.com/linq/c-array-delimited-tostring.html
	/// </summary>
	public static class ExtensionMethods
	{
		public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
		{
			StringBuilder sb = new StringBuilder();

			int previousIndex = 0;
			int index = str.IndexOf(oldValue, comparison);
			while (index != -1)
			{
				sb.Append(str.Substring(previousIndex, index - previousIndex));
				sb.Append(newValue);
				index += oldValue.Length;

				previousIndex = index;
				index = str.IndexOf(oldValue, index, comparison);
			}
			sb.Append(str.Substring(previousIndex));

			return sb.ToString();
		}

		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}
		public static string ToString<T>(this IEnumerable<T> source, string separator)
		{
			if (source == null)
				throw new ArgumentException("Parameter source can not be null.");

			if (string.IsNullOrEmpty(separator))
				throw new ArgumentException("Parameter separator can not be null or empty.");

			string[] array = source.Where(n => n != null).Select(n => n.ToString()).ToArray();

			return string.Join(separator, array);
		}

		// for interface IEnumerable
		public static string ToString(this IEnumerable source, string separator)
		{
			if (source == null)
				throw new ArgumentException("Parameter source can not be null.");

			if (string.IsNullOrEmpty(separator))
				throw new ArgumentException("Parameter separator can not be null or empty.");

			string[] array = source.Cast<object>().Where(n => n != null).Select(n => n.ToString()).ToArray();

			return string.Join(separator, array);
		}
	}
}

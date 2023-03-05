using System;

/* Curiously recurring template pattern checker to ensure that a type recurred a correct type.
 *
 * Usage example:
 *
 * public class Demo<T> : where T : Demo<T>
 * {
 *		protected Demo()
 *		{
 *			CRTPChecker.Check<T>(this);
 *		}
 *	}
 */

public static class CRTPChecker
{
	public static void Check<T>(object instance)
		where T : class
	{
		if (instance as T == null)
		{
			throw new ApplicationException(string.Format("Type {0} must recur itself but recurred {1}",
					instance.GetType(), typeof(T)));
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Pasdecote.Store
{
	public static class CommonExtensions
	{
		public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> src, TKey key,
		                                               TValue defaultValue = default (TValue))
		{
			if (src == null)
				return default(TValue);
			TValue val;
			if (src.TryGetValue(key, out val))
				return val;

			return defaultValue;
		}

		public static string GetDescriptionFromEnumValue(this Enum value)
		{
			DescriptionAttribute attribute = value.GetType()
			                                      .GetField(value.ToString())
			                                      .GetCustomAttributes(typeof (DescriptionAttribute), false)
			                                      .SingleOrDefault() as DescriptionAttribute;
			return attribute == null ? value.ToString() : attribute.Description;
		}



		public static TValue GetEnumAttribute<TEnum, TAttr, TValue>(this TEnum enumValue, Func<TAttr, TValue> getter,
		                                                            TValue defaultValue = default (TValue))
			where TEnum : struct
			where TAttr : Attribute
		{
			var fi = enumValue.GetType().GetField(enumValue.ToString());

			var attributes = (TAttr[]) fi.GetCustomAttributes(typeof (TAttr), false);

			if (attributes.Length > 0)
			{
				var attr = (TAttr) attributes[0];
				return getter(attr);
			}
			else
				return defaultValue;
		}

		public static TRet Maybe<TRet, TSrc>(this TSrc src, Func<TSrc, TRet> func,
		                                     TRet defaultValue = default(TRet))
		{
			if (src == null)
				return defaultValue;
			return func(src);
		}

		public static void Maybe<TSrc>(this TSrc src, Action<TSrc> func)
			where TSrc : class
		{
			if (src == null)
				return;

			func(src);
		}

		public static string GetName<T>(this Object tgt, Expression<Func<T>> expr)
		{
			return ((MemberExpression) expr.Body).Member.Name;
		}

		public static string GetName<T>(Expression<Func<T, Object>> expr)
		{
			return ((MemberExpression) expr.Body).Member.Name;
		}

		public static string GetPropertyName<T>(Expression<Func<T, object>> exp)
		{
			MemberExpression body = exp.Body as MemberExpression;

			if (body == null)
			{
				UnaryExpression ubody = (UnaryExpression) exp.Body;
				body = ubody.Operand as MemberExpression;
			}

			return body.Member.Name;
		}

		public static string GetPropertyName<T>(this T parent, Expression<Func<T, object>> exp)
		{
			return GetPropertyName<T>(exp);
		}

		public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> exp)
		{
			MemberExpression body = exp.Body as MemberExpression;

			if (body == null)
			{
				UnaryExpression ubody = (UnaryExpression) exp.Body;
				body = ubody.Operand as MemberExpression;
			}

			var attribute =
				(DisplayNameAttribute) body.Member.GetCustomAttributes(typeof (DisplayNameAttribute), true).FirstOrDefault();
			if (attribute == null)
			{
				return "";
			}
			else
			{
				return attribute.DisplayName;
			}
		}


		public static IEnumerable<TResult> LeftJoin<TLeft, TRight, TKey, TResult>(
			this IEnumerable<TLeft> left,
			IEnumerable<TRight> right,
			Func<TLeft, TKey> leftKeySelector,
			Func<TRight, TKey> rightKeySelector,
			Func<TLeft, TRight, TResult> resultSelector)
		{
			var leftJoined = left
				.GroupJoin(right, leftKeySelector, rightKeySelector, (a, gj) => new {a, gj})
				.SelectMany(t => t.gj.DefaultIfEmpty(), (t, subB) => resultSelector(t.a, subB));

			return leftJoined;
		}


		/// <summary>
		/// Производит сравнение DateTime с учётом Kind
		/// При стандартном сравнении DateTime не учитывается Kind. 
		/// </summary>
		/// <returns>
		/// Less than zero:	This date/time instance is less than the val object.
		/// Zero:	This date/time instance is equal to the val object.
		/// Greater than zero: This date/time instance is greater than the val object.
		/// </returns>
		public static Int32 SafeComare(this DateTime first, DateTime second)
		{
			if (first.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("DateTimeKind.Unspecified is not supported", "first");
			if (second.Kind == DateTimeKind.Unspecified)
				throw new ArgumentException("DateTimeKind.Unspecified is not supported", "second");

			DateTime secondToUse;
			if (first.Kind == DateTimeKind.Local && second.Kind == DateTimeKind.Utc)
			{
				secondToUse = second.ToLocalTime();
			}
			else if (first.Kind == DateTimeKind.Utc && second.Kind == DateTimeKind.Local)
			{
				secondToUse = second.ToUniversalTime();
			}
			else
				secondToUse = second;


			return first.CompareTo(secondToUse);
		}

		/// <summary>
		/// Представляет коллекцию в виде List, причём если есть возможность, не копирует её, в отличие от ToList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="that"></param>
		/// <returns></returns>
		public static IList<T> AsList<T>(this IEnumerable<T> that)
		{
			var list = that as List<T>;
			if (list != null)
				return list.AsReadOnly(); //редактировать исходный список опасно, так как тот, кто его предоставил в виде Enumerable этого не  ожидает

			return that.ToList();


		}

		public static string[] ReadAllLines(this TextReader that)
		{
			var ret = new List<string>();
			while (that.Peek() > 0)
			{
				ret.Add(that.ReadLine());
			}
			return ret.ToArray();
		}


		
	}
}
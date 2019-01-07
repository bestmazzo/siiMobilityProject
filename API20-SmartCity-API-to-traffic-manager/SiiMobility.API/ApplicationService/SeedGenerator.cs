using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiiMobility.API.ApplicationService
{
	public static class SeedGenerator
	{
		public static List<T> Generate<T>(int numberOfElements = 1, int numberOfSubElements = 1)
		{
			#region GetProperties

			var properties = typeof(T).GetRuntimeProperties().ToList();
			object[] initializedProperties = new object[properties.Count];

			#endregion


			List<T> returnList = new List<T>();
			for (var i = 0; i < numberOfElements; i++)
			{
				T element;
				//if(typeof(T).GetConstructor(Type.EmptyTypes) == null)
				//{
				//	element = (T)Convert.ChangeType("", typeof(T));
				//}
				//else
				element = (T)Activator.CreateInstance(typeof(T));

				foreach (var propertyInfo in properties)
				{
					Type type = propertyInfo.PropertyType;
					Type generatorType = typeof(StaticValueGenerator<>);

					//var testMethod = typeof(StaticValueGenerator<>).GetTypeInfo().GetDeclaredMethod("GenerateValue").MakeGenericMethod(type)
					//	.Invoke(generatorType, null);


					Type genericType = typeof(StaticValueGenerator<>).MakeGenericType(type);
					var testMethod = genericType.GetTypeInfo().GetDeclaredMethod("GenerateValue")
						.Invoke(genericType, new object[] { propertyInfo, numberOfSubElements });
					//var test = Activator.CreateInstance(genericType)
					propertyInfo.SetValue(element, testMethod);
				}

				returnList.Add(element);
			}

			return returnList;
		}

		//private static T ValorizeProperty<T>()
		//{
		//	if (typeof(string) == T)
		//	{
		//		return "ABCDEFG";
		//	}
		//}

		//private static T GenerateValue<T>()
		//{
		//	if ()
		//}

		//private static T GenerateValue
	}

	public class ValueGenerator<T>
	{
		public T Value { get; set; }
		public ValueGenerator()
		{
			var type = typeof(T);
			if (type == typeof(string))
			{
				var result = "Stringa di test";
				Value = (T)Convert.ChangeType(result, type);
			}
		}
	}

	public static class StaticValueGenerator<T>
	{
		public static T GenerateValue(PropertyInfo propertyInfo = null, int numberOfElements = 1)
		{
			if (propertyInfo != null && propertyInfo.Name == "Id")
				return default(T);

			var type = typeof(T);
			object result = new object();
			if (type == typeof(string))
			{
				//var minDate = propertyInfo.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.MaxLengthAttribute))
				result = RandomizedValueGenerator.RandomString(20);
			}

			else if (type == typeof(int))
			{
				result = new Random().Next(0, 100);
			}

			else if (type == typeof(double))
			{
				result = new Random().NextDouble() * (5000000d - 0d) + 0d;
			}

			else if (type == typeof(long))
			{
				result = (long)((new Random().NextDouble() * 2.0 - 1.0) * long.MaxValue);
			}

			else if (type == typeof(DateTime))
			{
				result = RandomizedValueGenerator.RandomDay();
			}

			else if (type == typeof(Guid))
			{
				result = Guid.NewGuid();
			}

			else if (type.IsPrimitive)
			{
				throw new Exception("Unhandled primitive");
			}

			else
			{
				//Type test = Type.GetType("SeedGenerator.Generate<>");
				Type seedGeneratorType = typeof(SeedGenerator);
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
				{
					return ((T)(seedGeneratorType.GetMethod("Generate")?.MakeGenericMethod(type.GenericTypeArguments.First()).Invoke(seedGeneratorType, new object[] { numberOfElements, numberOfElements })));
				}
				return ((List<T>)(seedGeneratorType.GetMethod("Generate")?.MakeGenericMethod(type).Invoke(seedGeneratorType, new object[] { numberOfElements, numberOfElements }))).First(); //TODO: Create instance if null
			}

			if (result == null)
			{
				throw new Exception();
			}

			return (T)Convert.ChangeType(result, type);

		}


	}

	public static class RandomizedValueGenerator
	{
		public static DateTime RandomDay(DateTime? min = null, DateTime? max = null)
		{
			Random gen = new Random();
			var minDate = min ?? DateTime.Today.AddYears(-1);
			var maxDate = max ?? DateTime.Today.AddYears(1);
			int range = (DateTime.Today - minDate).Days;
			return minDate.AddDays(gen.Next(range));
		}

		public static string RandomString(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}

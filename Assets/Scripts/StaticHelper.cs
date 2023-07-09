using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class StaticHelper
{
	public static int currentSimulations;

	[InitializeOnEnterPlayMode]
	static void OnEnterPlaymodeInEditor(EnterPlayModeOptions options)
	{
		ResetStaticsOnEnterPlaymode<MonoBehaviour>();
		SetDefaultStaticValues();
	}

	static void ResetStaticsOnEnterPlaymode<T>() where T : class
	{
		foreach (var item in GetEnumerableOfType<T>())
		{
			foreach (var field in item.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				// Check if field type is a value type
				if (field.FieldType.IsValueType)
				{
					// Set field to default value for value types
					object defaultValue = Activator.CreateInstance(field.FieldType);
					field.SetValue(null, defaultValue);
				}
				else
				{
					// Resetting field to null for reference types
					field.SetValue(null, null);
				}
			}
		}
	}

	public static IEnumerable<Type> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
	{
		List<Type> objects = new List<Type>();
		foreach (Type type in
			Assembly.GetAssembly(typeof(T)).GetTypes()
			.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
		{
			objects.Add(type);
		}
		objects.Sort();
		return objects;
	}

	public static void SetDefaultStaticValues()
	{
		//Set all your statics here
		GameTurn.moveCells = new List<Cell>();
		GameTurn.turn = Turn.White;
		GameTurn.isDragging = false;
		GameTurn.OnTurnChanged = null;
}
}
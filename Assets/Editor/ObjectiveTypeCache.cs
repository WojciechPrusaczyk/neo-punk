using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ObjectiveTypeCache
{
    private static List<Type> _requirementTypes;
    private static string[] _requirementTypeNames;

    public static List<Type> RequirementTypes
    {
        get
        {
            if (_requirementTypes == null)
            {
                FindAllRequirementTypes();
            }
            return _requirementTypes;
        }
    }

    public static string[] RequirementTypeNames
    {
        get
        {
            if (_requirementTypeNames == null)
            {
                FindAllRequirementTypes();
            }
            return _requirementTypeNames;
        }
    }

    private static void FindAllRequirementTypes()
    {
        _requirementTypes = new List<Type>();
        var baseType = typeof(ObjectiveRequirementBase);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        _requirementTypes.Add(type);
                    }
                }
            }
            catch (System.Reflection.ReflectionTypeLoadException)
            {
                // Ignore assemblies that can't be loaded
            }
        }
        _requirementTypeNames = _requirementTypes.Select(type => type.Name).ToArray();
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        FindAllRequirementTypes();
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Xml;
using Models;

namespace TypeEmitAdministrator
{
    internal static class EmitManager
    {
        /// <summary>
        /// Creates a Custom Model assembly with given name and extended type definitions and saves it to assemblyName.dll
        /// </summary>
        /// <param name="assemblyName">The name of the assembly (will be saved as assemblyName.dll)</param>
        /// <param name="extendedTables">The enumeration of extended tables to be implemented</param>
        public static void CreateCustomModelAssembly(string assemblyName, IEnumerable<ExtendedTable> extendedTables)
        {
            string assemblyFileName = assemblyName + ".dll";
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyFileName, assemblyFileName);

            var derivedTypes = new List<Type>();

            var tableMappings = new TableMappings();
            foreach (var extendedTable in extendedTables)
            {
                var tableMapping = new TableMapping(extendedTable.TableName);
                foreach (var extendedType in extendedTable.Types)
                {
                    var derivedType = CreateDerivedType(moduleBuilder, extendedTable, extendedType);

                    derivedTypes.Add(derivedType);

                    tableMapping.Add(new TypeMapping(extendedType.BaseType, derivedType));
                }
                tableMappings.Add(tableMapping);
            }
            SerializeMappings(assemblyName, tableMappings);
            assemblyBuilder.Save(assemblyFileName);
        }

        private static void SerializeMappings(string assemblyName, TableMappings mapper)
        {
            using (var xmlWriter = XmlWriter.Create(assemblyName + ".xml"))
            {
                var serializer = new DataContractSerializer(mapper.GetType());
                serializer.WriteObject(xmlWriter, mapper);
            }
        }

        private static Type CreateDerivedType(ModuleBuilder moduleBuilder, ExtendedTable extendedTable, ExtendedType extendedType)
        {
            // TODO: fix
            string randomString = Guid.NewGuid().ToString().Split(new char[] { '-' })[0];
            var typeBuilder = moduleBuilder.DefineType(extendedType.BaseType.Name + randomString, TypeAttributes.Class | TypeAttributes.Public, extendedType.BaseType);

            foreach (var extendedProperty in extendedTable.ExtendedProperties)
            {
                AddProperty(typeBuilder, extendedType.BaseType, extendedProperty.PropertyName, extendedProperty.PropertyType);
            }

            return typeBuilder.CreateType();
        }
        private static void AddProperty(TypeBuilder typeBuilder, Type baseType, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField(propertyName + Guid.NewGuid(),
                                                       propertyType,
                                                       FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            
            // TODO: fix
            //var customAttributeBuilder = new CustomAttributeBuilder(typeof (RequiredAttribute).GetConstructor(Type.EmptyTypes), new object[0]);
            //propertyBuilder.SetCustomAttribute(customAttributeBuilder);

            const MethodAttributes getSetAttributes = MethodAttributes.Public
                                                    | MethodAttributes.SpecialName
                                                    | MethodAttributes.HideBySig;

            var getPropertyValueMethod =
                typeof (BusinessObject<>).MakeGenericType(baseType).GetMethod("GetProperty",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance)
                                                                   .MakeGenericMethod(propertyType);
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + propertyName, getSetAttributes, propertyType, Type.EmptyTypes);
            ILGenerator getGenerator = getMethodBuilder.GetILGenerator();
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldstr, propertyName);
            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            getGenerator.Emit(OpCodes.Call, getPropertyValueMethod);
            getGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);

            var setPropertyValueMethod =
                typeof (BusinessObject<>).MakeGenericType(baseType).GetMethod("SetProperty",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance)
                                                                   .MakeGenericMethod(propertyType);
            var setMethodBuilder = typeBuilder.DefineMethod("set_" + propertyName, getSetAttributes, null, new[] { propertyType });
            ILGenerator setGenerator = setMethodBuilder.GetILGenerator();
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldstr, propertyName);
            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldflda, fieldBuilder);
            setGenerator.Emit(OpCodes.Ldarg_1);
            setGenerator.Emit(OpCodes.Call, setPropertyValueMethod);
            setGenerator.Emit(OpCodes.Ret);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}

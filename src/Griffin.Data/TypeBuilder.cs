//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Reflection;
//using System.Reflection.Emit;

//namespace Griffin.Data;

//public static class MyTypeBuilder
//{
//    public static void CreateNewObject()
//    {
//        var myType = CompileResultType();
//        var myObject = Activator.CreateInstance(myType);
//    }

//    /*
//     *
//     * var listGenericType = typeof(List<>);
//var list = listGenericType.MakeGenericType(GeneratedType);
//var constructor = list.GetConstructor(new Type[] { });
//var newList = (IList)constructor.Invoke(new object[] { });
//foreach (var value in values)
//    newList.Add(value);
//     */
//    public static Type CompileResultType(Type proxiedType)
//    {
//        var tb = GetTypeBuilder(proxiedType);
//        var constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
//                                                      MethodAttributes.RTSpecialName);

//        foreach (var property in proxiedType.GetProperties())
//        {
//            CreateProxiedProperty(property.Name, property.);
//        }
//        // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
//        //foreach (var field in yourListOfFields)
//        //    CreateProperty(tb, field.FieldName, field.FieldType);

//        var objectType = tb.CreateType();
//        return objectType;
//    }

//    private static TypeBuilder GetTypeBuilder(Type proxiedType)
//    {
//        var typeSignature = proxiedType.Name + "Proxy";
//        var an = new AssemblyName("OrmProxies");

//        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
//        var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
//        var tb = moduleBuilder.DefineType(typeSignature,
//            TypeAttributes.Public |
//            TypeAttributes.Class |
//            TypeAttributes.AutoClass |
//            TypeAttributes.AnsiClass |
//            TypeAttributes.BeforeFieldInit |
//            TypeAttributes.AutoLayout,
//            null);

//        tb.inh
//        return tb;
//    }

//    private static void CreateProxiedProperty(TypeBuilder tb, string propertyName, Type propertyType)
//    {
//        var fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

//        var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
//        var getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
//            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
//            Type.EmptyTypes);
//        var getIl = getPropMthdBldr.GetILGenerator();

//        getIl.Emit(OpCodes.Ldarg_0);
//        getIl.Emit(OpCodes.Ldfld, fieldBuilder);
//        getIl.Emit(OpCodes.Ret);

//        var setPropMthdBldr =
//            tb.DefineMethod("set_" + propertyName,
//                MethodAttributes.Public |
//                MethodAttributes.SpecialName |
//                MethodAttributes.HideBySig,
//                null, new[] { propertyType });

//        var setIl = setPropMthdBldr.GetILGenerator();
//        var modifyProperty = setIl.DefineLabel();
//        var exitSet = setIl.DefineLabel();

//        setIl.MarkLabel(modifyProperty);
//        setIl.Emit(OpCodes.Ldarg_0);
//        setIl.Emit(OpCodes.Ldarg_1);
//        setIl.Emit(OpCodes.Stfld, fieldBuilder);

//        setIl.Emit(OpCodes.Nop);
//        setIl.MarkLabel(exitSet);
//        setIl.Emit(OpCodes.Ret);

//        propertyBuilder.SetGetMethod(getPropMthdBldr);
//        propertyBuilder.SetSetMethod(setPropMthdBldr);
//    }
//}

//public class DynamicClass : DynamicObject
//{
//    private Dictionary<string, KeyValuePair<Type, object>> _fields;

//    public DynamicClass(List<Field> fields)
//    {
//        _fields = new Dictionary<string, KeyValuePair<Type, object>>();
//        fields.ForEach(x => _fields.Add(x.FieldName,
//            new KeyValuePair<Type, object>(x.FieldType, null)));
//    }

//    public override bool TrySetMember(SetMemberBinder binder, object value)
//    {
//        if (_fields.ContainsKey(binder.Name))
//        {
//            var type = _fields[binder.Name].Key;
//            if (value.GetType() == type)
//            {
//                _fields[binder.Name] = new KeyValuePair<Type, object>(type, value);
//                return true;
//            }
//            else throw new Exception("Value " + value + " is not of type " + type.Name);
//        }
//        return false;
//    }

//    public override bool TryGetMember(GetMemberBinder binder, out object result)
//    {
//        result = _fields[binder.Name].Value;
//        return true;
//    }
//}



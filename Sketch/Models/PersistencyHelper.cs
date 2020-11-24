using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RuntimeCheck;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sketch.Models
{
    static class PersistencyHelper
    {
        public static IEnumerable<System.Reflection.FieldInfo> GetAllPersistentFields(object obj, Type baseType)
        {
            if (obj == null) throw new ArgumentNullException("The parameter 'obj' must not be null");
            if (baseType == null) throw new ArgumentNullException("The parameter 'baseType' must not be null");

            var persistentFields = new List<System.Reflection.FieldInfo>();
            var type = obj.GetType();
            while (type != baseType )
            {
                persistentFields.AddRange(type.GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance).Where(
                        (x) => x.GetCustomAttributes(true).OfType<PersistentFieldAttribute>().Any())
                    );
                type = type.BaseType;
            }
            
            return persistentFields;
        }

        public static bool GetPersistentFieldInfo(FieldInfo field, out PersistentFieldAttribute info)
        {
            info = null;
            var persistent = field.GetCustomAttributes(true).OfType<PersistentFieldAttribute>();
            if (persistent.Any())
            {
                info = persistent.First();
                return true;
            }
            return false;
        }

        public static void BackupPersistentFields( object obj, SerializationInfo info, IEnumerable<FieldInfo> fields)
        {
            foreach (var f in fields)
            {
                var persistent = f.GetCustomAttributes(true).OfType<PersistentFieldAttribute>();
                var persistentSpec = persistent.First();
                info.AddValue(persistentSpec.Name, f.GetValue(obj), f.FieldType);
            }
        }

        public static IEnumerable<string> RestorePersistentFields(object obj, SerializationInfo info, IEnumerable<FieldInfo> fields, int version)
        {
            List<string> persistentNames = new List<string>();
            foreach (var f in fields)
            {
                // the version is restored at this point
                if (PersistencyHelper.GetPersistentFieldInfo(f, out PersistentFieldAttribute fieldInfo))
                {

                    if (fieldInfo.AvalailableSince <= version)
                    {
                        var val = info.GetValue(fieldInfo.Name, f.FieldType);
                        f.SetValue(obj, val);
                        persistentNames.Add(fieldInfo.Name);
                    }
                    else
                    {
                        Assert.True(fieldInfo.HasDefault, "The field {0} cannot be restored from the current stream",
                            fieldInfo.Name);
                    }
                }
            }
            return persistentNames;
        }

    }
}

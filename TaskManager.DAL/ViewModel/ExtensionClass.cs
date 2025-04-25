using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DAL.ViewModel
{
    public static class ExtensionClass
    {
        public static TTarget ConvertObject<TSource, TTarget>(TSource source)
            where TTarget : new()
        {
            var target = new TTarget();
            var sourceProps = typeof(TSource).GetProperties();
            var targetProps = typeof(TTarget).GetProperties();

            foreach (var targetProp in targetProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == targetProp.Name
                                                              && p.PropertyType == targetProp.PropertyType);
                if (sourceProp != null)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
        }
        public static List<TTarget> ConvertList<TSource, TTarget>(List<TSource> sourceList)
            where TTarget : new()
        {
            var targetList = new List<TTarget>();
            var sourceProps = typeof(TSource).GetProperties();
            var targetProps = typeof(TTarget).GetProperties();

            foreach (var source in sourceList)
            {
                var target = new TTarget();
                foreach (var targetProp in targetProps)
                {
                    var sourceProp = sourceProps.FirstOrDefault(p =>
                        p.Name == targetProp.Name &&
                        p.PropertyType == targetProp.PropertyType
                    );

                    if (sourceProp != null)
                    {
                        var value = sourceProp.GetValue(source);
                        targetProp.SetValue(target, value);
                    }
                }
                targetList.Add(target);
            }

            return targetList;
        }


        public static List<int> ToIntList(this string? source, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(source))
                return new List<int>();

            return source
                .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var val) ? val : (int?)null)
                .OfType<int>()
                .ToList();
        }

    }

}

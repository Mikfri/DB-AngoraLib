using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services.HelperService
{
    public static class HelperServices
    {
        /// <summary>
        /// Kopierer properties fra en 'source' til et 'target' HVIS properties har samme navn. Ellers ignoreres de.
        /// </summary>
        /// <typeparam name="TSource">Kilde filen</typeparam>
        /// <typeparam name="TTarget">Target filen</typeparam>
        /// <param name="source">Kilde filen</param>
        /// <param name="target">Target filen</param>
        public static void CopyPropertiesTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                var value = sourceProperty.GetValue(source);
                if (value != null)
                {
                    var targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
                    if (targetProperty != null && targetProperty.CanWrite)
                    {
                        targetProperty.SetValue(target, value);
                    }
                }
            }
        }
    }

}

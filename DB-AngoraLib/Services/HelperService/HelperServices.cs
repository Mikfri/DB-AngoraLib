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
        public static void CopyProperties_FromAndTo<TSource, TDestination>(TSource source, TDestination destination, bool allowNulls = false)
        {
            if (source == null || destination == null)
            {
                throw new ArgumentNullException("Source or/and Destination Objects are null");
            }

            var sourceProperties = typeof(TSource).GetProperties();
            var destinationProperties = typeof(TDestination).GetProperties();

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(dp => dp.Name == sourceProperty.Name && dp.PropertyType == sourceProperty.PropertyType);
                if (destinationProperty != null && destinationProperty.CanWrite)
                {
                    var value = sourceProperty.GetValue(source, null);
                    if (allowNulls || value != null)
                    {
                        destinationProperty.SetValue(destination, value, null);
                    }
                }
            }
        }
    }

}

using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Busidex.DataAccess.DotNet
{
    internal static class DataReaderExtension
    {
        public static IEnumerable<T> ConvertTo<T>(this IDataReader reader)
        {
			var results = new List<T>();
			var properties = typeof(T).GetProperties();

			while (reader.Read())
			{
				var item = Activator.CreateInstance<T>();
				foreach (var property in typeof(T).GetProperties())
				{
                    try
                    {
						var colIdx = reader.TryGetOrdinal(property.Name);
						if(colIdx > 0)
                        {
							if (!reader.IsDBNull(colIdx))
							{
								
							}
						}
					}
					catch(IndexOutOfRangeException ex)
                    {
						// This property wasn't returned by the data reader.
						// Move to the next property.
                    }
					catch(Exception ex)
                    {

                    }
				}
				results.Add(item);
			}
			return results;
			
        }

		public static int TryGetOrdinal(this IDataReader reader, string columnName)
		{
			try
			{
				return reader.GetOrdinal(columnName);
			}
			catch (IndexOutOfRangeException)
			{
				return -1;
			}
		}

	}
}

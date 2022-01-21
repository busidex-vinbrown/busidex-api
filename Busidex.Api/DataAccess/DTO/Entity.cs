using System;

namespace Busidex.Api.DataAccess.DTO
{
    public static class Entity
    {

        //public static object Copy( object source, object destination )
        //{

        //    System.Reflection.PropertyInfo[] sourceProps = source.GetType().GetProperties();

        //    System.Reflection.PropertyInfo[] destinationProps = destination.GetType().GetProperties();

        //    foreach ( System.Reflection.PropertyInfo sourceProp in sourceProps )
        //    {

        //        ColumnAttribute column =
        //            Attribute.GetCustomAttribute( sourceProp, typeof ( ColumnAttribute ) ) as ColumnAttribute;

        //        if ( column != null && !column.IsPrimaryKey )
        //        {

        //            foreach ( System.Reflection.PropertyInfo destinationProp in destinationProps )
        //            {

        //                if ( sourceProp.Name == destinationProp.Name && destinationProp.CanWrite )
        //                {

        //                    destinationProp.SetValue( destination, sourceProp.GetValue( source, null ), null );

        //                    break;

        //                }

        //            }

        //        }

        //    }

        //    return destination;

        //}

    }
}

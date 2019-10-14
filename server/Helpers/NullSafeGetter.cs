using System.Data;

public static class NullSafeGetter {
    public static T GetValueOrDefault<T> (this IDataRecord row, string fieldName) where T : struct {
        int ordinal = row.GetOrdinal (fieldName);
        T value = row.GetValueOrDefault<T> (ordinal);
        return value;
    }

    #nullable enable
    public static T? GetValueOrNull<T> (this IDataRecord row, string fieldName) where T : class {
        int ordinal = row.GetOrdinal (fieldName);
        return row.GetValueOrNull<T> (ordinal);
    }

    public static T GetValueOrDefault<T> (this IDataRecord row, int ordinal) where T : struct {
        return (T) (row.IsDBNull (ordinal) ? default (T) : row.GetValue (ordinal));
    }

    public static T? GetValueOrNull<T> (this IDataRecord row, int ordinal) where T : class {
        return (T?) (row.IsDBNull (ordinal) ? null : row.GetValue (ordinal));
    }
}
Public Class Resource
    Inherits Databasic.Provider.Resource

    Public Overrides Function GetTableColumns(connection As Databasic.Connection, table As String) As List(Of String)
        Dim result As New List(Of String)
        Dim schema = "dbo"
        Dim dotPos = table.IndexOf("."c) ' ignore (screw) here all tables with dot contained in name, only realy dummy developers can use that form...:-(
        If dotPos > -1 Then
            schema = table.Substring(0, dotPos)
            table = table.Substring(dotPos + 1)
        End If
		Return Databasic.Statement.Prepare(
			"SELECT c.name
                FROM sys.columns AS c 
                WHERE
                    c.object_id = (
                        SELECT o.object_id
                        FROM sys.objects AS o 
                        WHERE 
                            o.name = @table AND 
                            o.schema_id = (
                                SELECT s.schema_id 
                                FROM sys.schemas AS s 
                                WHERE s.name = @schema
                            )
                    )
                    ORDER BY c.column_id",
			connection
		).FetchAll(New With {
			.schema = schema,
			.table = table
		}).ToList(Of String)()
	End Function

    Public Overrides Function GetLastInsertedId(transaction As Databasic.Transaction) As Object
		Return Databasic.Statement.Prepare("SELECT SCOPE_IDENTITY()", transaction).FetchOne().ToInstance(Of Object)()
	End Function

    Public Overrides Function GetAll(
        connection As Databasic.Connection,
        table As String,
        columns As String,
        Optional offset As Int64? = Nothing,
        Optional limit As Int64? = Nothing,
        Optional orderByStatement As String = Database.DEFAUT_UNIQUE_COLUMN_NAME
    ) As Databasic.Statement
        Dim sql = $"SELECT {columns} FROM {table}"
        offset = If(offset, 0)
        limit = If(limit, 0)
        If limit > 0 Then
            If connection.ProviderVersion.Major < 12 Then
                sql = $"SELECT {If(limit > 0, "TOP " + limit, "")} ____src.*
                    INTO #result 
                    FROM (
                        SELECT *, ROW_NUMBER() OVER (ORDER BY {orderByStatement}) AS ____rowNumber
                        FROM {table}
                    ) AS ____src 
                        WHERE ____src.____rowNumber > {offset} 
                    ORDER BY {orderByStatement}
                    ALTER TABLE #result
                    DROP COLUMN ____rowNumber
                    SELECT * FROM #result
                    DROP TABLE #result"
            Else
                sql += $" ORDER BY {orderByStatement} OFFSET {offset} ROWS"
                If limit > 0 Then sql += $" FETCH NEXT {limit} ROWS ONLY"
            End If
        End If
		Return Databasic.Statement.Prepare(sql, connection).FetchAll()
	End Function

End Class
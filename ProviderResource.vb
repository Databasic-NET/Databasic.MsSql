Public Class ProviderResource
	Inherits Databasic.ProviderResource

	Public Overrides Function GetTableColumns(table As String, connection As Databasic.Connection) As Dictionary(Of String, Boolean)
		Dim schema = "dbo"
		Dim dotPos = table.IndexOf("."c) ' ignore (screw) here all tables with dot contained in name, only realy dummy developers can use that form...:-(
		If dotPos > -1 Then
			schema = table.Substring(0, dotPos)
			table = table.Substring(dotPos + 1)
		End If
		Return Databasic.Statement.Prepare("
				SELECT 
					c.is_nullable,
					c.name
				FROM 
					sys.columns AS c 
				WHERE
					c.object_id = (
						SELECT 
							o.object_id
						FROM 
							sys.objects AS o 
						WHERE 
							o.name = @table AND 
							o.schema_id = (
								SELECT s.schema_id 
								FROM sys.schemas AS s 
								WHERE s.name = @schema
							)
					)
				ORDER BY 
					c.column_id
			", connection
		).FetchAll(New With {
			.schema = schema,
			.table = table
		}).ToDictionary(Of String, Boolean)("name")
	End Function

	Public Overrides Function GetLastInsertedId(ByRef transaction As Databasic.Transaction, Optional ByRef classMetaDescription As MetaDescription = Nothing) As Object
		If classMetaDescription.Tables.Length > 0 Then
			Return Databasic.Statement.Prepare(
				"SELECT IDENT_CURRENT(@table)", transaction
			).FetchOne(New With {
				.table = classMetaDescription.Tables(0)
			}).ToInstance(Of Object)()
		Else
			Return Databasic.Statement.Prepare(
				"SELECT @@IDENTITY", transaction
			).FetchOne().ToInstance(Of Object)()
		End If
	End Function

	'Public Overrides Function GetAll(
	'	connection As Databasic.Connection,
	'	table As String,
	'	columns As String,
	'	Optional offset As Int64? = Nothing,
	'	Optional limit As Int64? = Nothing,
	'	Optional orderByStatement As String = ""
	') As Databasic.Statement
	'	Dim sql = $"SELECT {columns} FROM {table}"
	'	offset = If(offset, 0)
	'	limit = If(limit, 0)
	'	If limit > 0 Then
	'		If connection.ProviderVersion.Major < 12 Then
	'			sql = $"SELECT {If(limit > 0, "TOP " + limit, "")} ____src.*
	'    INTO #result 
	'    FROM (
	'  SELECT *, ROW_NUMBER() OVER (ORDER BY {orderByStatement}) AS ____rowNumber
	'  FROM {table}
	'    ) AS ____src 
	'  WHERE ____src.____rowNumber > {offset} 
	'    ORDER BY {orderByStatement}
	'    ALTER TABLE #result
	'    DROP COLUMN ____rowNumber
	'    SELECT * FROM #result
	'    DROP TABLE #result"
	'		Else
	'			sql += $" ORDER BY {orderByStatement} OFFSET {offset} ROWS"
	'			If limit > 0 Then sql += $" FETCH NEXT {limit} ROWS ONLY"
	'		End If
	'	End If
	'	Return Databasic.Statement.Prepare(sql, connection).FetchAll()
	'End Function

End Class
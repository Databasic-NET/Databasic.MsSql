Public Class SqlError
	Inherits Databasic.SqlError

	Public Property Server As String
	Public Property Procedure As String
	Public Property Source As String
	Public Property LineNumber As Integer
	Public Property [Class] As Byte
	Public Property State As Byte

	Public Sub New(msSqlError As System.Data.SqlClient.SqlError)
		Me.Message = msSqlError.Message
		Me.Code = msSqlError.Number

		Me.Server = msSqlError.Server
		Me.Procedure = msSqlError.Procedure
		Me.Source = msSqlError.Source
		Me.LineNumber = msSqlError.LineNumber
		Me.[Class] = msSqlError.[Class]
		Me.State = msSqlError.State
	End Sub
End Class

Imports System.Data.Common
Imports System.Data.SqlClient

Public Class Connection
	Inherits Databasic.Connection

	Public Overrides ReadOnly Property Provider As DbConnection
		Get
			Return Me._provider
		End Get
	End Property
	Private _provider As SqlConnection

	Public Overrides ReadOnly Property ProviderResource As System.Type = GetType(ProviderResource)

	Public Overrides ReadOnly Property ClientName As String = "System.Data.SqlClient"

	Public Overrides ReadOnly Property Statement As System.Type = GetType(Statement)

	Public Overrides Sub Open(dsn As String)
		Me._provider = New SqlConnection(dsn)
		Me._provider.Open()
		AddHandler Me._provider.InfoMessage, AddressOf Connection.errorHandler
	End Sub

	Public Overrides Function CreateAndBeginTransaction(Optional transactionName As String = "", Optional isolationLevel As IsolationLevel = IsolationLevel.Unspecified) As Databasic.Transaction
        Return New Transaction() With {
            .ConnectionWrapper = Me,
            .Instance = Me._provider.BeginTransaction(isolationLevel, transactionName.Substring(0, Math.Min(transactionName.Length - 1, 32)))
        }
    End Function

End Class
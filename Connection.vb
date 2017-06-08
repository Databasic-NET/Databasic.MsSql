Imports System.Data.Common
Imports System.Data.SqlClient

Public Class Connection
    Inherits Databasic.Connection

    Public Shared Shadows ClientName As String = "System.Data.SqlClient"

    Public Overrides Property Provider As DbConnection
        Get
            Return Me._provider
        End Get
        Set(value As DbConnection)
            Me._provider = value
        End Set
    End Property
    Private _provider As SqlConnection

    Public Overrides ReadOnly Property ProviderVersion As Version
        Get
            If Me._providerVersion Is Nothing Then
                Me._providerVersion = New Version(Me.Provider.ServerVersion)
            End If
            Return Me._providerVersion
        End Get
    End Property
    Private _providerVersion As Version = Nothing

    Public Overrides Property StatementType As System.Type
        Get
            Return Me._statementType
        End Get
        Set(value As System.Type)
            Me._statementType = value
        End Set
    End Property
    Private _statementType As System.Type = GetType(Statement)

    Public Overrides Property ResourceType As System.Type
        Get
            Return Me._resourceType
        End Get
        Set(value As System.Type)
            Me._resourceType = value
        End Set
    End Property
    Private _resourceType As System.Type = GetType(Resource)

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
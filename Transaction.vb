Imports System.Data.Common
Imports System.Data.SqlClient

Public Class Transaction
    Inherits Databasic.Transaction

    Public Overrides Property Instance As DbTransaction
        Get
            Return Me._instance
        End Get
        Set(value As DbTransaction)
            Me._instance = value
        End Set
    End Property
    Private _instance As SqlTransaction

    Public Shadows Sub Rollback(transactionName As String)
        Try
            Me._instance.Rollback(transactionName)
            Me.ConnectionWrapper.OpenedTransaction = Nothing
        Catch ex As Exception
            Me.ConnectionWrapper.OpenedTransaction = Nothing
            Throw ex
        End Try
    End Sub

    Public Sub Save(savePointName As String)
        Try
            Me._instance.Save(savePointName)
            Me.ConnectionWrapper.OpenedTransaction = Nothing
        Catch ex As Exception
            Me.ConnectionWrapper.OpenedTransaction = Nothing
            Throw ex
        End Try
    End Sub

End Class
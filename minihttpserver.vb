Imports System
Imports System.Collections.Specialized
Imports System.Net
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Text
Imports System.IO
Imports System.Threading
Imports System.Data
Imports System.Data.OracleClient
Imports System.Data.OleDb
Imports System.Collections
Imports System.Collections.Generic
Imports System.Web.Serialization

Public Class MiniHttpServer
    Private listener As HttpListener
    Private serverThread As Thread

    Public Sub New()
        listener = New HttpListener()
        listener.Prefixes.Add("http://localhost:8080/")
    End Sub

    Public Sub Start()
        Threading.ThreadPool.QueueUserWorkItem(
            Sub()
                listener.Start()
                While listener.IsListening
                    Try
                        Dim context As HttpListenerContext = listener.GetContext()
                        Dim request As HttpListenerRequest = context.Request
                        Dim response As HttpListenerResponse = context.Response

                        Dim responseData As String = ""

                        Select Case request.HttpMethod
                            Case "GET"
                                Dim filename As String = request.Url.LocalPath.TrimStart("/")
                                Dim parameters As NameValueCollection = request.QueryString
                                responseData = HandleGetRequest(filename, parameters)
                            Case "POST"
                                Dim filename As String = request.Url.LocalPath.TrimStart("/")
                                Dim parameters As NameValueCollection = New NameValueCollection()
                                If request.HasEntityBody Then
                                    Using reader As New StreamReader(request.InputStream, request.ContentEncoding)
                                        Dim body As String = reader.ReadToEnd()
                                        parameters = ParseQueryString(body)
                                    End Using
                                End If
                                responseData = HandlePostRequest(filename, parameters)
                            Case Else
                                response.StatusCode = 405 ' Method Not Allowed
                                responseData = "Method Not Allowed"
                        End Select

                        Dim buffer As Byte() = Encoding.UTF8.GetBytes(responseData)
                        response.ContentLength64 = buffer.Length
                        response.OutputStream.Write(buffer, 0, buffer.Length)
                        response.OutputStream.Close()
                    Catch ex As HttpListenerException When ex.ErrorCode = 995 ' Error code for I/O aborted
                        Console.WriteLine("I/O operation aborted.")
                        Exit While
                    Catch ex As Exception
                        Console.WriteLine("Exception: " & ex.Message)
                    End Try
                End While
            end sub
        )

    End Sub

    Public Sub [Stop]()
        listener.Stop()
    End Sub

    Private Function HandleGetRequest(filename As String, parameters As NameValueCollection) As String
        ' Here you can handle the GET request
        ' You can use filename and parameters as needed
        ' For example:
        ' Return some HTML content based on the requested filename and parameters
        Return "<html><head><title>GET Response</title></head><body><h1>GET Request</h1><p>Filename: " & filename & "</p><p>Parameters: " & parameters.ToString() & "</p></body></html>"
    End Function

    Private Function HandlePostRequest(filename As String, parameters As NameValueCollection) As String
        ' Here you can handle the POST request
        ' You can use filename and parameters as needed
        ' For example:
        ' Return some HTML content based on the requested filename and parameters
        Return "<html><head><title>POST Response</title></head><body><h1>POST Request</h1><p>Filename: " & filename & "</p><p>Parameters: " & parameters.ToString() & "</p></body></html>"
    End Function

    Private Function ParseQueryString(queryString As String) As NameValueCollection
        Dim parameters As New NameValueCollection()
        Dim pairs As String() = queryString.Split("&"c)
        For Each pair As String In pairs
            Dim keyValue As String() = pair.Split("="c)
            If keyValue.Length = 2 Then
                Dim key As String = Uri.UnescapeDataString(keyValue(0))
                Dim value As String = Uri.UnescapeDataString(keyValue(1))
                parameters.Add(key, value)
            End If
        Next
        Return parameters
    End Function
End Class

Public Module MainModule
    Private WithEvents notifyIcon As New NotifyIcon()

    Dim server As New MiniHttpServer()
    Public Sub Main()
        server.Start()
        dim customIcon as new Icon(".\ico\logo.ico")
        notifyIcon.Icon = customIcon  'SystemIcons.Application
        notifyIcon.Visible = True
        notifyIcon.Text = "Mini HTTP Server is running"

        Dim contextMenu As New ContextMenu()
        Dim menuItem As New MenuItem("Exit", AddressOf Exit_Click)
        contextMenu.MenuItems.Add(menuItem)

        notifyIcon.ContextMenu = contextMenu

        Application.Run()
    End Sub

    Private Sub Exit_Click(sender As Object, e As EventArgs)
        notifyIcon.Visible = False ' Hide the tray icon before exiting
        Application.Exit()
    End Sub
End Module

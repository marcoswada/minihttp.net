Imports System
Imports System.Net
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Text
Imports System.IO
Imports System.Threading

Public Class MiniHttpServer
    Private listener As HttpListener
    Private serverThread As Thread

    Public Sub New()
        listener = New HttpListener()
        listener.Prefixes.Add("http://localhost:8080/")
    End Sub

    Public Sub Start()
        serverThread = New Thread(AddressOf Listen)
        serverThread.Start()
    End Sub

    Public Sub [Stop]()
        listener.Stop()
        serverThread.Join()
    End Sub

    Private Sub Listen()
        listener.Start()
        While listener.IsListening
            Try
                Dim context As HttpListenerContext = listener.GetContext()
                Dim request As HttpListenerRequest = context.Request
                Dim response As HttpListenerResponse = context.Response
                Dim responseString As String = "<html><head><title>Mini HTTP Server</title></head><body><h1>Hello from Mini HTTP Server!</h1></body></html>"
                Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes(responseString)
                response.ContentLength64 = buffer.Length
                response.OutputStream.Write(buffer, 0, buffer.Length)
                response.OutputStream.Close()
                If request.HttpMethod = "GET" Then
                    HandleGetRequest(request, response)
                ElseIf request.HttpMethod = "POST" Then
                    HandlePostRequest(request, response)
                Else
                    ' Unsupported method
                    Dim errorMessage As String = "Unsupported HTTP method: " & request.HttpMethod
                    Dim errorBytes As Byte() = Encoding.UTF8.GetBytes(errorMessage)
                    response.StatusCode = 405 ' Method Not Allowed
                    response.StatusDescription = "Method Not Allowed"
                    response.OutputStream.Write(errorBytes, 0, errorBytes.Length)
                    response.Close()
                End If

            Catch ex As Exception
                Console.WriteLine("Exception: " + ex.Message)
            End Try
        End While
    End Sub
    Private Sub HandleGetRequest(ByVal request As HttpListenerRequest, ByVal response As HttpListenerResponse)
        ' Extract query parameters from the URL
        Dim queryParams As String = request.Url.Query
        Dim responseString As String = "<html><head><title>GET Request</title></head><body><h1>GET Request Received</h1><p>Query Parameters: " & queryParams & "</p></body></html>"
        Dim buffer As Byte() = Encoding.UTF8.GetBytes(responseString)

        response.ContentType = "text/html"
        response.ContentLength64 = buffer.Length

        Dim output As System.IO.Stream = response.OutputStream
        output.Write(buffer, 0, buffer.Length)
        output.Close()
    End Sub

    Private Sub HandlePostRequest(ByVal request As HttpListenerRequest, ByVal response As HttpListenerResponse)
        ' Read the request body
        Dim requestBody As String
        Using reader As New StreamReader(request.InputStream, request.ContentEncoding)
            requestBody = reader.ReadToEnd()
        End Using

        Dim responseString As String = "<html><head><title>POST Request</title></head><body><h1>POST Request Received</h1><p>Request Body: " & requestBody & "</p></body></html>"
        Dim buffer As Byte() = Encoding.UTF8.GetBytes(responseString)

        response.ContentType = "text/html"
        response.ContentLength64 = buffer.Length

        Dim output As System.IO.Stream = response.OutputStream
        output.Write(buffer, 0, buffer.Length)
        output.Close()
    End Sub


End Class

Public Module MainModule
    Dim server As New MiniHttpServer()
    Public Sub Main()
        server.Start()

        Dim notifyIcon As New NotifyIcon()
        notifyIcon.Icon = System.Drawing.SystemIcons.Information
        notifyIcon.Visible = True
        notifyIcon.Text = "Mini HTTP Server is running"

        Dim contextMenu As New ContextMenu()
        Dim menuItem As New MenuItem("Exit", AddressOf Exit_Click)
        contextMenu.MenuItems.Add(menuItem)

        notifyIcon.ContextMenu = contextMenu

        Application.Run()
    End Sub

    Private Sub Exit_Click(sender As Object, e As EventArgs)
        server.Stop()
        Application.Exit()
    End Sub
End Module

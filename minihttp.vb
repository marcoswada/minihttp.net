Imports System
Imports System.Net
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Text
Imports System.IO

Module MiniHttpServer
    Sub Main()
        Dim listener As New HttpListener()
        Dim contextMenu As New ContextMenu()
        Dim notifyIcon As New NotifyIcon()

        contextMenu.MenuItems.Add("Exit", AddressOf ExitApplication)

        notifyIcon.Icon = System.Drawing.SystemIcons.Information ' Specify System.Drawing.SystemIcons.Information
        notifyIcon.Text = "My Application"
        notifyIcon.ContextMenu = contextMenu
        notifyIcon.Visible = True

        MessageBox.Show("Application running in system tray. Right-click the icon to exit.", "System Tray Application", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Application.Run()

        listener.Prefixes.Add("http://localhost:8080/")
        listener.Start()
        Console.WriteLine("Listening for requests on http://localhost:8080/")

        While True
            Dim context As HttpListenerContext = listener.GetContext()
            Dim request As HttpListenerRequest = context.Request
            Dim response As HttpListenerResponse = context.Response

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

    Sub ExitApplication(ByVal sender As Object, ByVal e As EventArgs)
        Application.Exit()
    End Sub

End Module

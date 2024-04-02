Imports System
Imports System.Net
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing

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
                Dim response As HttpListenerResponse = context.Response
                Dim responseString As String = "<html><head><title>Mini HTTP Server</title></head><body><h1>Hello from Mini HTTP Server!</h1></body></html>"
                Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes(responseString)
                response.ContentLength64 = buffer.Length
                response.OutputStream.Write(buffer, 0, buffer.Length)
                response.OutputStream.Close()
            Catch ex As Exception
                Console.WriteLine("Exception: " + ex.Message)
            End Try
        End While
    End Sub
End Class

Public Module MainModule
    Public Sub Main()
        Dim server As New MiniHttpServer()
        server.Start()

        Dim notifyIcon As New NotifyIcon()
        notifyIcon.Icon = SystemIcons.Application
        notifyIcon.Visible = True
        notifyIcon.Text = "Mini HTTP Server is running"

        Dim contextMenu As New ContextMenu()
        Dim menuItem As New MenuItem("Exit", AddressOf Exit_Click)
        contextMenu.MenuItems.Add(menuItem)

        notifyIcon.ContextMenu = contextMenu

        Application.Run()
    End Sub

    Private Sub Exit_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub
End Module

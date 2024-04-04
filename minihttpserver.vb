Imports System
Imports System.Collections
Imports System.Collections.Generic
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
'Imports System.Web.Serialization

Namespace Global.Utils
Namespace Json

Public Class aboutForm : Inherits Form
    Private btnOpen as button
    Private btnOk as button
    Private lblMessage as Label
    Public Sub New()
        Me.Text="About..."
        Me.Size = New Size(415, 345)

        btnOpen = New Button()
        btnOpen.Location = New Point(45,265)
        btnOpen.Name = "btnOpen"
        btnOpen.Size = New Size(100,30)
        btnOpen.Text = "Open"
        AddHandler btnOpen.Click, New EventHandler(AddressOf btnOpen_Click)

        btnOk = New Button()
        btnOk.Location = New Point(150,265)
        btnOk.Name = "btnOk"
        btnOk.Size = New Size(100,30)
        btnOk.Text = "OK"
        AddHandler btnOk.Click, New EventHandler(AddressOf btnOk_Click)

        lblMessage = New Label()
        lblMessage.Location = New Point(5,5)
        lblMessage.Name = "lblMessage"
        lblMessage.Text = "sdjfbsijfk"
        lblMessage.Size = New Size(390, 255)
        lblMessage.BackColor = Color.FromARGB(255,0,128,255)

        Me.Controls.Add(btnOk)
        Me.Controls.Add(btnOpen)
        Me.Controls.Add(lblMessage)
    End Sub

    Private Sub btnOpen_Click()
        Shell ("explorer.exe http://localhost:8080/")
    End Sub

    Private Sub btnOk_Click()
        Me.Close()
    End Sub
End Class

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
                                Dim parameters As NameValueCollection = ParseQueryString(request.Url.Query.TrimStart("?"))  ' request.QueryString()
                                dim k
                                console.writeline("request.QueryString: " & request.QueryString().count)
                                console.writeline("request.Url.Query: " & request.Url.Query())
                                for each k in parameters
                                    console.writeline(k &  ":" & parameters(k))
                                Next
                                console.writeline("filename: " & filename)
                                if filename = "example.json" then
                                    console.writeline("content-type: json")
                                    response.ContentType="application/json"
                                Else
                                    response.ContentType = "text/html"
                                end if
                                responseData = HandleGetRequest(filename, parameters)
                            Case "POST"
                                Dim filename As String = request.Url.LocalPath.TrimStart("/")
                                Dim parameters As NameValueCollection = New NameValueCollection()
                                console.writeline(request.toString())
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
                        console.writeline("...: " & response.ContentType)
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
        dim sParameters as String = "{<br>"
        dim key
        for each key in parameters
            sParameters = sParameters & """" & key & """:" & parameters(key) & ",<br>" & vbcrlf 
        Next
        sParameters=sParameters & "}"
        dim sRetVal = "<html><head><title>GET Response</title></head><body><h1>GET Request</h1><p>Filename: " & filename & "</p><p>Parameters: " & sParameters & "</p><form method=""POST""><input type=""TEXT"" /><INPUT type=""SUBMIT"" ></form><button onclick=btn_onclick()>JSON</button><script>async function btn_onclick(){  fetch('example.json').then(response => {if (!response.ok) {throw new Error('Network response was not ok');} return response.json();}).then(data => {alert(JSON.stringify(data,null,2));}).catch(error => {console.error('There was a problem with the fetch operation:', error);});}</script></body></html>"
	if filename="example.json" then
		sRetVal="{""data"": [{""type"": ""articles"",""id"": ""1"",""attributes"": {""title"": ""JSON:API paints my bikeshed!"",""body"": ""The shortest article. Ever."",""created"": ""2015-05-22T14:56:29.000Z"",""updated"": ""2015-05-22T14:56:28.000Z""},""relationships"": {""author"": {""data"": {""id"": ""42"", ""type"": ""people""}}}}],""included"": [{""type"": ""people"",""id"": ""42"",""attributes"": {""name"": ""John"",""age"": 80,""gender"": ""male""}}]}"
	end if
    	Return sRetVal
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
'        msgbox(queryString)
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
        Dim menuItem1 As New MenuItem("About", AddressOf About_Click)
        Dim menuItem2 As New MenuItem("Exit", AddressOf Exit_Click)
        contextMenu.MenuItems.Add(menuItem1)
        contextMenu.MenuItems.Add(menuItem2)

        notifyIcon.ContextMenu = contextMenu

        Application.Run()
    End Sub

    Private Sub notifyIcon_DoubleClick() Handles notifyIcon.DoubleClick
        shell ("explorer.exe http://localhost:8080/")
    End Sub

    Private Sub About_Click(sender As Object, e As EventArgs)
        Dim frmAbout As New aboutForm
        frmAbout.Show()
    End Sub

    Private Sub Exit_Click(sender As Object, e As EventArgs)
        notifyIcon.Visible = False ' Hide the tray icon before exiting
        Application.Exit()
    End Sub
End Module

End Namespace
End Namespace

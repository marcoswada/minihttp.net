Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.IO.Text

Module Module1
    Sub Main()
        ' Define the icon data as a byte array
        Dim iconData As Byte() 

        ' read icon data from file and output as code
        iconData = My.Computer.FileSystem.ReadAllBytes(".\ico\logo.ico")

        ' Create an icon object from the byte array
        Dim icon As Icon
        Using memoryStream As New MemoryStream(iconData)
            icon = New Icon(memoryStream)
        End Using

        ' Use the icon as needed
        ' For example, set it as the form's icon
        'Dim form As New Form()
        'form.Icon = icon
        'form.ShowDialog()

        ' Dispose the icon object when done
        icon.Dispose()

        dim sOut as String = "Dim iconData as String = """ & Convert.ToBase64String(iconData) & """"
'Convert.FromBase64String(base64IconData) 
        
        my.computer.FileSystem.WriteAllText(".\iconData.vb", sOut, false)
    End Sub
End Module

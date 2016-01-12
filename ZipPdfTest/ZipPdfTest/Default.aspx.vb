Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Net.Mime
Imports System.Security.Cryptography
Imports System.Text
Imports ICSharpCode.SharpZipLib
Imports ICSharpCode.SharpZipLib.Zip

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        textboxFileName.Text = "test.pdf"
    End Sub

    Protected Sub buttonTestCompression_Click(sender As Object, e As EventArgs) Handles buttonTestCompression.Click
        ProtectedPdfDownloadMethod("Compressed")
    End Sub

    Protected Sub buttonTestNormal_Click(sender As Object, e As EventArgs) Handles buttonTestNormal.Click
        ProtectedPdfDownloadMethod("Normal")
    End Sub

    Protected Sub buttonDanTest_Click(sender As Object, e As EventArgs) Handles buttonDanTest.Click
        ProtectedPdfDownloadMethod("Dan")
    End Sub

    Private Sub ProtectedPdfDownloadMethod(action As String)
        Dim filePath As String = "C:\Projects\ZipPdfTest\ZipPdfTest\Files\" & textboxFileName.Text
        Dim fileName As String = textboxFileName.Text
        Dim fs As Stream = File.OpenRead(filePath)

        If action = "Compressed" Then
            DownloadZippedPdfFile(fs, fileName, Context)
            'DanTest(fs, fileName, Context)
        ElseIf action = "Normal" Then
            DownloadFile(fs, fileName, Context)
        Else
            DanTest(fs, fileName, Context)
        End If

    End Sub

    Public Shared Sub DownloadFile(fs As Stream, filename As String, ByRef Context As HttpContext)
        Try
            Dim bytes() As Byte
            ReDim bytes(fs.Length)
            fs.Read(bytes, 0, fs.Length)
            Context.Response.ClearHeaders()
            Context.Response.ClearContent()
            Context.Response.ContentType = MediaTypeNames.Application.Octet
            Context.Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(filename).Replace(" ", "_") + ".pdf")
            Context.Response.AppendHeader("content-length", fs.Length)
            Context.Response.BinaryWrite(bytes)
            Context.Response.Flush()
            Context.Response.End()
        Catch ex As Threading.ThreadAbortException
            'Swallow this exception as it is expected.
        Catch fEx As FileNotFoundException

            Throw fEx

        Catch ex As Exception

            Throw ex

        Finally
            If fs IsNot Nothing Then
                fs.Close()
                fs.Dispose()
            End If
        End Try

    End Sub

    Public Shared Sub DanTest(fs As Stream, filename As String, ByRef Context As HttpContext)

        ' Code adapted from this example: https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples#anchorCreateIIS

        Context.Response.ClearHeaders()
        Context.Response.ClearContent()

        Context.Response.ContentType = MediaTypeNames.Application.Zip
        Context.Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(filename).Replace(" ", "_") + ".zip")
        Context.Response.CacheControl = "Private"
        Context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(3))
        Dim buffer As Byte() = New Byte(4095) {}

        Dim zipOutputStream As New ZipOutputStream(Context.Response.OutputStream)
        zipOutputStream.SetLevel(5)     '0-9, 9 being the highest level of compression

        ' ALREADY DONE >> fs = File.OpenRead(filename)
        Dim zipEntry As ZipEntry = New ZipEntry(filename + ".pdf")
        'Dim zipEntry As New ZipEntry(zipEntry.CleanName(filename))

        zipEntry.Size = fs.Length
        ' Setting the Size provides WinXP built-in extractor compatibility,
        '  but if not available, you can set zipOutputStream.UseZip64 = UseZip64.Off instead.

        zipOutputStream.PutNextEntry(zipEntry)

        Dim count As Integer = fs.Read(buffer, 0, buffer.Length)
        While count > 0
            zipOutputStream.Write(buffer, 0, count)
            count = fs.Read(buffer, 0, buffer.Length)
            If Not Context.Response.IsClientConnected Then
                Exit While
            End If
            Context.Response.Flush()
        End While
        fs.Close()

        zipOutputStream.Close()

        Context.Response.Flush()
        Context.Response.End()
    End Sub

    Public Shared Sub DownloadZippedPdfFile(fs As Stream, filename As String, ByRef Context As HttpContext)
        Context.Response.ClearHeaders()
        Context.Response.ClearContent()

        Dim zipOutputStream As ZipOutputStream = New ZipOutputStream(Context.Response.OutputStream)
        zipOutputStream.SetLevel(5)
        Dim buffer(fs.Length) As Byte
        fs.Read(buffer, 0, buffer.Length)
        Dim zipEntry As ZipEntry = New ZipEntry(filename + ".pdf")
        zipOutputStream.PutNextEntry(zipEntry)
        zipOutputStream.Write(buffer, 0, buffer.Length)
        Dim length As Long = zipEntry.Size
        zipOutputStream.Finish()
        zipOutputStream.Close()

        Try
            Dim bytes() As Byte
            ReDim bytes(fs.Length)
            fs.Read(bytes, 0, fs.Length)
            Context.Response.ContentType = MediaTypeNames.Application.Zip
            Context.Response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName(filename).Replace(" ", "_") + ".zip")
            Context.Response.AppendHeader("content-length", fs.Length)
            Context.Response.BinaryWrite(bytes)
            Context.Response.Flush()
            Context.Response.End()
        Catch ex As Threading.ThreadAbortException
            'Swallow this exception as it is expected.
        Catch fEx As FileNotFoundException

            Throw fEx

        Catch ex As Exception

            Throw ex

        Finally
            If fs IsNot Nothing Then
                fs.Close()
                fs.Dispose()
            End If
        End Try

    End Sub

End Class
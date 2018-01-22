Imports Xm.Core
Imports System.IO
Imports System.Net
Imports System.Xml
Imports System.Threading

Namespace Library
    Public Class EightBall
        Const URL As String = "https://8ball.delegator.com/magic/XML/{0}"
        Const ACCEPT As String = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"
        Const USERAGENT As String = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.108 Safari/537.36"
        <Method(Types.Null, "eightball")>
        Public Shared Function EightBallQuery(question As String) As String
            Dim handle As New ManualResetEvent(False), result As String = String.Empty
            Dim request As HttpWebRequest = CType(WebRequest.Create(String.Format(EightBall.URL, WebUtility.UrlEncode(question))), HttpWebRequest)
            EightBall.EightBallQuerySend(request, handle, result)
            If (Not handle.WaitOne(3000)) Then
                Return "Could not complete API request due to a time out"
            End If
            Return result
        End Function
        Private Shared Sub EightBallQuerySend(req As HttpWebRequest, hnd As ManualResetEvent, ByRef result As String)
            Try
                req.Accept = EightBall.ACCEPT
                req.UserAgent = EightBall.USERAGENT
                req.Credentials = CredentialCache.DefaultCredentials
                Using response As WebResponse = req.GetResponse()
                    If (CType(response, HttpWebResponse).StatusCode = HttpStatusCode.OK) Then
                        Using stream As StreamReader = New StreamReader(response.GetResponseStream)
                            Dim doc As New XmlDocument()
                            doc.LoadXml(stream.ReadToEnd())
                            result = String.Format("({0}) {1}", doc.DocumentElement.SelectSingleNode("/magic/type").InnerText,
                                                                doc.DocumentElement.SelectSingleNode("/magic/answer").InnerText)
                        End Using
                    Else
                        result = String.Format("Could not complete API request due to a {0} response", CType(response, HttpWebResponse).StatusCode.ToString)
                    End If
                End Using
            Finally
                hnd.Set()
            End Try
        End Sub
    End Class
End Namespace
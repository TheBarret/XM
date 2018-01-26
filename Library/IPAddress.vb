Imports Xm.Core

Namespace Library
    Public Class IPAddress
        <Method(Types.String, "subnetof")>
        Public Shared Function SubnetOf(cidr As String, ip As String) As Boolean
            If (cidr.Contains("/")) Then
                Dim segments As String() = cidr.Split("/"c)
                If (Net.IPAddress.TryParse(segments.First, Nothing) AndAlso Net.IPAddress.TryParse(ip, Nothing)) Then
                    Dim address As Integer, base As Integer, mask As Integer
                    base = BitConverter.ToInt32(Net.IPAddress.Parse(segments.First).GetAddressBytes, 0)
                    address = BitConverter.ToInt32(Net.IPAddress.Parse(ip).GetAddressBytes, 0)
                    mask = Net.IPAddress.HostToNetworkOrder(-1 << (32 - Integer.Parse(segments(1))))
                    Return ((base And mask) = (address And mask))
                End If
            End If
            Return False
        End Function
    End Class
End Namespace
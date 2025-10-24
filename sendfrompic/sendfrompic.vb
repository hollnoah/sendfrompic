Imports System.IO.Ports

Public Class sendfrompic
    '8 BIT ASYNCH (FOSC/[16(N+1)]
    'BRG16 = 0
    'BRGH = 1
    'SYNC = 0
    'BAUD = 9600, ACTUAL = 9615
    '% ERROR = 0.16
    'SPBRG = 25

    'BAUDCTL(BAUD RATE CONTROL REGISTER)=00
    'INTCON= HEX 00
    'PIE1= HEX 00
    'PIR1= HEX 00
    'RCREG=NA
    'RCSTA(RECIEVE STATUS & CONTROL REGISTER)=80
    'SPBRG=DEC 25, HEX 19
    'SPBRGH= ?
    'TRISC=00 (CONFIGURE AS OUTPUTS)
    'TXREG=SEND "$"
    'TXSTA(TRANSMIT STATUS & CONTROL REGISTER)= HEX 2E

    ' Minimal COM port code: open COM6 at 9600,N,8,1 and report BytesToRead on DataReceived.
    ' Assumes a SerialPort component named SerialPort1 and a Button named ConnectButton exist in the Designer.

    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click
        Try
            If SerialPort1 IsNot Nothing AndAlso SerialPort1.IsOpen Then
                Console.WriteLine("Port already open.")
                Return
            End If

            ' Fixed to COM6 per your setup
            SerialPort1.PortName = "COM6"
            SerialPort1.BaudRate = 9600
            SerialPort1.Parity = Parity.None
            SerialPort1.DataBits = 8
            SerialPort1.StopBits = StopBits.One
            SerialPort1.Handshake = Handshake.None

            SerialPort1.DiscardInBuffer()
            SerialPort1.DiscardOutBuffer()

            SerialPort1.Open()
            Console.WriteLine($"Opened {SerialPort1.PortName}")
        Catch ex As Exception
            Console.WriteLine($"Failed to open port: {ex.Message}")
        End Try
    End Sub

    Private Sub SerialPort1_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Try
            ' Snapshot of how many bytes are currently available in the receive buffer
            Dim bytesAvailable = SerialPort1.BytesToRead
            Console.WriteLine($"DataReceived event: BytesToRead = {bytesAvailable}")

            ' Read the available bytes
            ReadData()
        Catch ex As Exception
            Console.WriteLine($"DataReceived handler error: {ex.Message}")
        End Try
    End Sub

    Private Sub ReadData()
        Try
            Dim count = SerialPort1.BytesToRead
            If count <= 0 Then Return

            Dim buffer(count - 1) As Byte
            Dim read = SerialPort1.Read(buffer, 0, count) ' read consumes bytes from the buffer

            Console.WriteLine($"Read {read} bytes:")
            For i = 0 To read - 1
                Dim b = buffer(i)
                Dim printable = If(b >= 32 AndAlso b <= 126, ChrW(b), ".")
                Console.WriteLine($"[{i}] 0x{b:X2} '{printable}'")
            Next
        Catch ex As Exception
            Console.WriteLine($"Read error: {ex.Message}")
        End Try
    End Sub

    Private Sub sendfrompic_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            If SerialPort1 IsNot Nothing AndAlso SerialPort1.IsOpen Then
                SerialPort1.Close()
            End If
        Catch
        End Try
    End Sub

End Class

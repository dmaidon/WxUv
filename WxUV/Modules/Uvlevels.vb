Namespace Modules
    Module Uvlevels

        Public Function GetUvLevel(lev As Double) As List(Of String)
            Dim UvLev As New List(Of String)
            ' Dim bc As Color = Color.Green
            Select Case lev
                Case 0 To 2.5
                    UvLev.Add("Low")
                    UvLev.Add("Low")
                    UvLev.Add("#558b2f")
                    UvLev.Add("Green")
                    Dim msg = <msg>
A UV Index reading of 0 to 2 means low danger from the sun's UV rays for the average person.

   - Wear sunglasses on bright days.
   - If you burn easily, cover up and use broad spectrum SPF 30+ sunscreen.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    UvLev.Add(msg)
                Case Is <= 5.5
                    UvLev.Add("Moderate")
                    UvLev.Add("Mod")
                    UvLev.Add("#f9a825")
                    UvLev.Add("Yellow")
                    Dim msg = <msg>
A UV Index reading of 3 to 5 means moderate risk of harm from unprotected sun exposure.

   - Stay in shade near midday when the sun is strongest.
   - If outdoors, wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                             </msg>.Value
                    UvLev.Add(msg)
                Case Is <= 7.5
                    UvLev.Add("High")
                    UvLev.Add("High")
                    UvLev.Add("#ef6c00")
                    UvLev.Add("Orange")
                    Dim msg = <msg>
A UV Index reading of 6 to 7 means high risk of harm from unprotected sun exposure. Protection against skin and eye damage is needed.

   - Reduce time in the sun between 10 a.m. and 4 p.m.
   - If outdoors, seek shade and wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    UvLev.Add(msg)
                Case <= 10.5
                    UvLev.Add("Very High")
                    UvLev.Add("VHigh")
                    UvLev.Add("#b71c1c")
                    UvLev.Add("Red")
                    Dim msg = <msg>
A UV Index reading of 8 to 10 means very high risk of harm from unprotected sun exposure. Take extra precautions because unprotected skin and eyes will be damaged and can burn quickly.

   - Minimize sun exposure between 10 a.m. and 4 p.m.
   - If outdoors, seek shade and wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    UvLev.Add(msg)
                Case Else
                    UvLev.Add("Extreme")
                    UvLev.Add("Extr")
                    UvLev.Add("#6a1b9a")
                    UvLev.Add("Violet")
                    Dim msg = <msg>
A UV Index reading of 11 or more means extreme risk of harm from unprotected sun exposure. Take all precautions because unprotected skin and eyes can burn in minutes.

   - Try to avoid sun exposure between 10 a.m. and 4 p.m.
   - If outdoors, seek shade and wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    UvLev.Add(msg)
            End Select
            Return UvLev
        End Function

    End Module
End Namespace
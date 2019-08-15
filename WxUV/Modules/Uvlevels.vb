Namespace Modules
    Module Uvlevels

        Public Function GetUvLevel(lev As Double) As List(Of String)
            Dim uvLev As New List(Of String)
            ' Dim bc As Color = Color.Green
            Select Case lev
                Case 0 To 2.5
                    uvLev.Add("Low")
                    [uvLev].Add("Low")
                    uvLev.Add("#558b2f")
                    uvLev.Add("Green")
                    Dim msg = <msg>
A UV Index reading of 0 to 2 means low danger from the sun's UV rays for the average person.

   - Wear sunglasses on bright days.
   - If you burn easily, cover up and use broad spectrum SPF 30+ sunscreen.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    uvLev.Add(msg)
                Case Is <= 5.5
                    uvLev.Add("Moderate")
                    uvLev.Add("Mod")
                    uvLev.Add("#f9a825")
                    uvLev.Add("Yellow")
                    Dim msg = <msg>
A UV Index reading of 3 to 5 means moderate risk of harm from unprotected sun exposure.

   - Stay in shade near midday when the sun is strongest.
   - If outdoors, wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                             </msg>.Value
                    uvLev.Add(msg)
                Case Is <= 7.5
                    uvLev.Add("High")
                    [uvLev].Add("High")
                    uvLev.Add("#ef6c00")
                    uvLev.Add("Orange")
                    Dim msg = <msg>
A UV Index reading of 6 to 7 means high risk of harm from unprotected sun exposure. Protection against skin and eye damage is needed.

   - Reduce time in the sun between 10 a.m. and 4 p.m.
   - If outdoors, seek shade and wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    uvLev.Add(msg)
                Case <= 10.5
                    uvLev.Add("Very High")
                    uvLev.Add("VHigh")
                    uvLev.Add("#b71c1c")
                    uvLev.Add("Red")
                    Dim msg = <msg>
A UV Index reading of 8 to 10 means very high risk of harm from unprotected sun exposure. Take extra precautions because unprotected skin and eyes will be damaged and can burn quickly.

   - Minimize sun exposure between 10 a.m. and 4 p.m.
   - If outdoors, seek shade and wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    uvLev.Add(msg)
                Case Else
                    uvLev.Add("Extreme")
                    uvLev.Add("Extr")
                    uvLev.Add("#6a1b9a")
                    uvLev.Add("Violet")
                    Dim msg = <msg>
A UV Index reading of 11 or more means extreme risk of harm from unprotected sun exposure. Take all precautions because unprotected skin and eyes can burn in minutes.

   - Try to avoid sun exposure between 10 a.m. and 4 p.m.
   - If outdoors, seek shade and wear protective clothing, a wide-brimmed hat, and UV-blocking sunglasses.
   - Generously apply broad spectrum SPF 30+ sunscreen every 2 hours, even on cloudy days, and after swimming or sweating.
   - Watch out for bright surfaces, like sand, water and snow, which reflect UV and increase exposure.
                            </msg>.Value
                    uvLev.Add(msg)
            End Select
            Return uvLev
        End Function

    End Module
End Namespace
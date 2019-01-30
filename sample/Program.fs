open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open GameCore.GameModel
open GameCore.ImGui.Model
open GameCore.ImGui.GameRunner
open ImGuiNET


type Model = {
    Button1: bool
    Button2: bool
    Text: string
    Checked: bool
}

[<EntryPoint>]
let main _ =
    
    let config = {
        clearColour = Some (new Color (50, 50, 50))
        resolution = Windowed (800, 600)
        assetsToLoad = [
            Texture ("avatar", "image.png")
        ]
        fpsFont = None
        mouseVisible = true
    }
    
    let advanceModel runState _ model = 
        if GameCore.GameModel.wasJustPressed Keys.Escape runState then None
        else
            match model with
            | None -> Some ()
            | _ -> model

    let getView _ _ = [ ]

    let ui = [
        yield Direct (fun _ ->
            let style = ImGui.GetStyle ()
            style.WindowRounding <- 0.f
            ImGui.StyleColorsLight ()
        )

        let win1Config = { title = Some "window 1"; pos = Some (10, 10); size = Some (200, 200); flags = standardFlags }
        yield window win1Config [
            yield text "hello world"
            yield button "this is a test" (fun m b -> { m with Button1 = b })
            yield row [
                text "row 1"
                text "row 2"
                text "row 3"
            ]
            yield multilineinput (fun _ -> "test") 100 (50, 50) (fun m _ -> m)
            yield checkbox "check" (fun m -> m.Checked) (fun m b -> { m with Checked = b })
        ]

        let win2Config = { title = Some "window 2"; pos = Some (300, 10); size = None; flags = standardFlags }
        yield window win2Config [
            text "test two"
            text "test three"
            textinput (fun m -> m.Text) 10 (fun m us -> { m with Text = us })
            button "test two" (fun m b -> { m with Button2 = b })
        ]

        let win3Config = { title = None; pos = Some (300, 300); size = Some (200, 200); flags = { standardFlags with noTitleBar = true } }
        yield window win3Config [

            yield text "line 1"
            yield text "line 2"
            yield DirectUpdate (fun model -> 
                let mutable buffer = model.Text
                ImGui.InputText("Text input", &buffer, 100ul) |> ignore
                { model with Text = buffer })
            yield Direct (fun model ->
                ImGui.Text model.Text)
        ]

        let win5config = { title = Some "image"; pos = Some (10, 300); size = Some (200, 200); flags = standardFlags }
        yield window win5config [
            yield image "avatar" 150 150
        ]
    ]

    let getUI _ =
        let startModel = { Button1 = false; Button2 = false; Text = "test"; Checked = true }
        startModel, ui

    runImGuiGame config advanceModel getView getUI

    0

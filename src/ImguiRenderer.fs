module ImguiRenderer

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open ImGuiNET
open System
open Microsoft.Xna.Framework.Input

type ImGuiRenderer (game : Game) =
    
    let game = game
    let graphicsDevice = game.GraphicsDevice

    let effect = new BasicEffect(graphicsDevice)
    let rasterizerState = 
        new RasterizerState
            (
                CullMode = CullMode.None,
                DepthBias = 0.f,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0.f
            )

    let mutable vertexData : byte [] = [||]
    let mutable vertexBuffer = Unchecked.defaultof<VertexBuffer>
    let mutable vertexBufferSize = 0

    let mutable indexData : byte [] = [||]
    let mutable indexBuffer = Unchecked.defaultof<IndexBuffer>
    let mutable indexBufferSize = 0

    // Textures
    let mutable loadedTextures = Map.empty<IntPtr, Texture2D>

    let mutable textureId = 0
    let mutable fontTextureId = Unchecked.defaultof<IntPtr>

    // Input
    let mutable scrollWheelValue = 0;

    let io = ImGui.GetIO ()

    let keys = 
        let mappings = [
            ImGuiKey.Tab, Keys.Tab
            ImGuiKey.LeftArrow, Keys.Left
            ImGuiKey.RightArrow, Keys.Right
            ImGuiKey.UpArrow, Keys.Up
            ImGuiKey.DownArrow, Keys.Down
            ImGuiKey.PageUp, Keys.PageUp
            ImGuiKey.PageDown, Keys.PageDown
            ImGuiKey.Home, Keys.Home
            ImGuiKey.End, Keys.End
            ImGuiKey.Delete, Keys.Delete
            ImGuiKey.Backspace, Keys.Back
            ImGuiKey.Enter, Keys.Enter
            ImGuiKey.Escape, Keys.Escape
            ImGuiKey.A, Keys.A
            ImGuiKey.C, Keys.C
            ImGuiKey.V, Keys.V
            ImGuiKey.X, Keys.X
            ImGuiKey.Y, Keys.Y
            ImGuiKey.Z, Keys.Z
        ]
        mappings |> List.iter (fun (dest, source) -> io.KeyMap.[int dest] <- int source)
        mappings |> List.map (snd >> int)

    do 
        let context = ImGui.CreateContext ()
        ImGui.SetCurrentContext context

        game.Window.TextInput.AddHandler(new EventHandler<TextInputEventArgs> (fun _ evt ->
            if evt.Character <> '\t' then 
                io.AddInputCharacter (uint16 evt.Character);
            ))
        io.Fonts.AddFontDefault () |> ignore
        
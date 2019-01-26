module GameCore.ImGui.GameLoop

open GameCore.GameLoop
open ImGuiNET
open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

type ImGuiGameLoop<'TModel, 'TUIModel> (config, updateModel, getView, startUIModel, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, (fun runState model -> updateModel runState this.CurrentUIModel model), getView)

    let mutable uiModel: 'TUIModel = startUIModel

    let mutable loadedTextures = Map.empty
    let mutable lastTextureId = 0

    let io = ImGui.GetIO ()

    let rasteriserState = 
        new RasterizerState
            (CullMode = CullMode.None, 
            DepthBias = 0.f, 
            FillMode = FillMode.Solid, 
            MultiSampleAntiAlias = false, 
            ScissorTestEnable = true, 
            SlopeScaleDepthBias = 0.f)

    do
        ImGui.CreateContext ()
        |> ImGui.SetCurrentContext
    
    let keys =
        let keyMap = [ 
            ImGuiKey.Tab, Keys.Tab; ImGuiKey.LeftArrow, Keys.Left; ImGuiKey.RightArrow, Keys.Right
            ImGuiKey.UpArrow, Keys.Up; ImGuiKey.DownArrow, Keys.Down; ImGuiKey.PageUp, Keys.PageUp
            ImGuiKey.PageDown, Keys.PageDown; ImGuiKey.Home, Keys.Home; ImGuiKey.End, Keys.End
            ImGuiKey.Delete, Keys.Delete; ImGuiKey.Backspace, Keys.Back; ImGuiKey.Enter, Keys.Enter
            ImGuiKey.Escape, Keys.Escape; ImGuiKey.A, Keys.A; ImGuiKey.C, Keys.C
            ImGuiKey.V, Keys.V; ImGuiKey.X, Keys.X; ImGuiKey.Y, Keys.Y; ImGuiKey.Z, Keys.Z ]
        keyMap 
        |> List.map (fun (dest, source) -> 
            io.KeyMap.[int dest] <- int source
            int source)
        
    do
        let handler = fun _ (evt: TextInputEventArgs) ->
            if evt.Character <> '\t' then io.AddInputCharacter (uint16 evt.Character)
        this.Window.TextInput.AddHandler (new EventHandler<TextInputEventArgs> (handler))
        io.Fonts.AddFontDefault () |> ignore

    let bindTexture texture = 
        let id = new IntPtr (lastTextureId)
        lastTextureId <- lastTextureId + 1
        loadedTextures <- Map.add id texture loadedTextures
        id
    
    let rebuildFontAtlas () =
        let (pixelData, width, height, bytesPerPixel) = io.Fonts.GetTexDataAsRGBA32 ()

        let pixelData = NativePtr.toNativeInt pixelData
        let pixels = Array.create<byte> (width * height * bytesPerPixel) 0uy
        Marshal.Copy(pixelData, pixels, 0, pixels.Length)

        let tex2d = new Texture2D(this.GraphicsDevice, width, height, false, SurfaceFormat.Color)
        tex2d.SetData(pixels)
        
        let newId = bindTexture tex2d
        io.Fonts.SetTexID newId
        io.Fonts.ClearTexData ()

    let updateInput io =
        ()

    let updateBuffers drawData = 
        ()

    let renderCommandLists drawData = 
        ()

    let renderDrawData (drawData: ImDrawDataPtr) =
        let lastViewport = this.GraphicsDevice.Viewport
        let lastScissorsBox = this.GraphicsDevice.ScissorRectangle

        this.GraphicsDevice.BlendFactor <- Color.White
        this.GraphicsDevice.BlendState <- BlendState.NonPremultiplied
        this.GraphicsDevice.RasterizerState <- rasteriserState
        this.GraphicsDevice.DepthStencilState <- DepthStencilState.DepthRead
        
        drawData.ScaleClipRects io.DisplayFramebufferScale

        let present = this.GraphicsDevice.PresentationParameters
        this.GraphicsDevice.Viewport <- new Viewport(0, 0, present.BackBufferWidth, present.BackBufferHeight)

        updateBuffers drawData
        renderCommandLists drawData

        this.GraphicsDevice.Viewport <- lastViewport
        this.GraphicsDevice.ScissorRectangle <- lastScissorsBox

        ()

    member __.CurrentUIModel
        with get () = uiModel
    
    override __.Initialize() = 
        rebuildFontAtlas ()
        base.Initialize ()

    override __.Draw (gameTime) = 
        base.Draw (gameTime)

        match base.CurrentModel with
        | Some model -> 
            io.DeltaTime <- float32 gameTime.ElapsedGameTime.TotalSeconds
            updateInput io
            ImGui.NewFrame ()

            uiModel <- getUI uiModel model
            
            ImGui.Render ()
            renderDrawData (ImGui.GetDrawData ())

        | _ -> ()
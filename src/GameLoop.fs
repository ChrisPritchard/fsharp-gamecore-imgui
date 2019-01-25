module GameCore.ImGui.GameLoop

open GameCore.GameLoop
open ImGuiNET
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Microsoft.Xna.Framework.Graphics
open System
open Microsoft.Xna.Framework

type ImGuiGameLoop<'TModel, 'TUIModel> (config, updateModel, getView, startUIModel, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, (fun runState model -> updateModel runState this.CurrentUIModel model), getView)

    let mutable uiModel: 'TUIModel = startUIModel

    let mutable loadedTextures = Map.empty
    let mutable lastTextureId = 0

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
    
    //do
        // setup input

    let bindTexture texture = 
        let id = new IntPtr (lastTextureId)
        lastTextureId <- lastTextureId + 1
        loadedTextures <- Map.add id texture loadedTextures
        id
    
    let rebuildFontAtlas () =
        let io = ImGui.GetIO ()
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

        let io = ImGui.GetIO ()
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
            
            let io = ImGui.GetIO()
            io.DeltaTime <- float32 gameTime.ElapsedGameTime.TotalSeconds
            updateInput io
            ImGui.NewFrame ()

            uiModel <- getUI uiModel model
            
            ImGui.Render ()
            renderDrawData (ImGui.GetDrawData ())

        | _ -> ()
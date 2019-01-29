module GameCore.ImGui.GameLoop

open GameCore.GameLoop
open ImGuiNET
open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

type Vector2 = System.Numerics.Vector2

type BufferSet<'TBuffer> = {
    data: byte []
    buffer: 'TBuffer
    size: int
}

type ImGuiGameLoop<'TModel, 'TUIModel> (config, updateModel, getView, startUIModel, getUI)
    as this = 
    inherit GameLoop<'TModel> (config, (fun runState model -> updateModel runState this.CurrentUIModel model), getView)

    let mutable uiModel: 'TUIModel = startUIModel

    let mutable loadedTextures = Map.empty
    let mutable lastTextureId = 0

    let mutable lastScrollWheel = 0

    let vertSize = sizeof<ImDrawVert>
    let vertDeclaration = 
        new VertexDeclaration (
            vertSize,
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        )

    let mutable vertexBuffer = { data = [||]; buffer = Unchecked.defaultof<VertexBuffer>; size = 0 }
    let mutable indexBuffer = { data = [||]; buffer = Unchecked.defaultof<IndexBuffer>; size = 0 }

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

    let updateInput (presentParams: PresentationParameters) =
        let mouse, keyboard = Mouse.GetState (), Keyboard.GetState ()
        keys |> List.iter (fun key -> io.KeysDown.[key] <- keyboard.IsKeyDown (enum<Keys>(key)))
        
        io.KeyShift <- keyboard.IsKeyDown Keys.LeftShift || keyboard.IsKeyDown Keys.RightShift
        io.KeyCtrl <- keyboard.IsKeyDown Keys.LeftControl || keyboard.IsKeyDown Keys.RightControl
        io.KeyAlt <- keyboard.IsKeyDown Keys.LeftAlt || keyboard.IsKeyDown Keys.RightAlt
        io.KeySuper <- keyboard.IsKeyDown Keys.LeftWindows || keyboard.IsKeyDown Keys.RightWindows

        io.DisplaySize <- new Vector2 (float32 presentParams.BackBufferWidth, float32 presentParams.BackBufferHeight)
        io.DisplayFramebufferScale <- new Vector2 (1.f, 1.f)

        io.MousePos <- new Vector2 (float32 mouse.X, float32 mouse.Y)
        [mouse.LeftButton;mouse.RightButton;mouse.MiddleButton] 
        |> List.iteri (fun i b -> io.MouseDown.[i] <- b = ButtonState.Pressed)

        let scrollDelta = mouse.ScrollWheelValue - lastScrollWheel
        io.MouseWheel <- if scrollDelta > 0 then 1.f elif scrollDelta < 0 then -1.f else 0.f
        lastScrollWheel <- scrollDelta

    let updateBuffers (drawData: ImDrawDataPtr) =
        if drawData.TotalVtxCount > vertexBuffer.size then
            if vertexBuffer.buffer <> null then vertexBuffer.buffer.Dispose ()
            let size = int (float drawData.TotalVtxCount * 1.5)
            vertexBuffer <- {
                size = size
                buffer = new VertexBuffer(this.GraphicsDevice, vertDeclaration, size, BufferUsage.None)
                data = Array.zeroCreate(size * vertSize)
            }

        if drawData.TotalIdxCount > indexBuffer.size then
            if indexBuffer.buffer <> null then indexBuffer.buffer.Dispose ()
            let size = int (float drawData.TotalIdxCount * 1.5)
            indexBuffer <- {
                size = size
                buffer = new IndexBuffer(this.GraphicsDevice, IndexElementSize.SixteenBits, size, BufferUsage.None)
                data = Array.zeroCreate(size * sizeof<uint16>)
            }
        
        let mutable vtxOffset, idxOffset = 0, 0
        for n = 0 to drawData.CmdListsCount - 1 do
            let cmdList = drawData.CmdListsRange.[n]
            let vtxDstPtr = NativePtr.toVoidPtr &&vertexBuffer.data.[vtxOffset * vertSize]
            Buffer.MemoryCopy (cmdList.VtxBuffer.Data.ToPointer (), vtxDstPtr, int64 vertexBuffer.data.Length, int64 cmdList.VtxBuffer.Size * int64 vertSize)
            let idxDstPtr = NativePtr.toVoidPtr &&indexBuffer.data.[idxOffset * sizeof<uint16>]
            Buffer.MemoryCopy (cmdList.IdxBuffer.Data.ToPointer (), idxDstPtr, int64 indexBuffer.data.Length, int64 cmdList.IdxBuffer.Size * int64 sizeof<uint16>)

            vtxOffset <- vtxOffset + cmdList.VtxBuffer.Size
            idxOffset <- idxOffset + cmdList.VtxBuffer.Size

        vertexBuffer.buffer.SetData (vertexBuffer.data, 0, drawData.TotalVtxCount * vertSize)
        vertexBuffer.buffer.SetData (indexBuffer.data, 0, drawData.TotalIdxCount * sizeof<uint16>)

    let renderCommandLists drawData = 
        ()

    let renderDrawData (presentParams: PresentationParameters) (drawData: ImDrawDataPtr) =
        let lastViewport = this.GraphicsDevice.Viewport
        let lastScissorsBox = this.GraphicsDevice.ScissorRectangle

        this.GraphicsDevice.BlendFactor <- Color.White
        this.GraphicsDevice.BlendState <- BlendState.NonPremultiplied
        this.GraphicsDevice.RasterizerState <- rasteriserState
        this.GraphicsDevice.DepthStencilState <- DepthStencilState.DepthRead
        
        drawData.ScaleClipRects io.DisplayFramebufferScale
        
        this.GraphicsDevice.Viewport <- new Viewport(0, 0, presentParams.BackBufferWidth, presentParams.BackBufferHeight)

        if drawData.TotalVtxCount > 0 then updateBuffers drawData
        renderCommandLists drawData

        this.GraphicsDevice.Viewport <- lastViewport
        this.GraphicsDevice.ScissorRectangle <- lastScissorsBox

    member __.CurrentUIModel
        with get () = uiModel
    
    override __.Initialize() = 
        rebuildFontAtlas ()
        base.Initialize ()
    
    //override __.LoadContent () =
    //    base.LoadContent ()
    //    grab loaded texture2ds and create a pointer map for them, so imgui can draw them

    override __.Draw (gameTime) = 
        base.Draw (gameTime)

        match base.CurrentModel with
        | Some model -> 
            io.DeltaTime <- float32 gameTime.ElapsedGameTime.TotalSeconds
            let presentParams = this.GraphicsDevice.PresentationParameters
            updateInput presentParams
            ImGui.NewFrame ()

            uiModel <- getUI uiModel model
            
            ImGui.Render ()
            renderDrawData presentParams (ImGui.GetDrawData ())

        | _ -> ()
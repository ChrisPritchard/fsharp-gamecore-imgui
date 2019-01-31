# fsharp-gamecore-imgui

Integrates ImGui.NET into FSharp-Gamecore. [ImGui.NET](https://github.com/mellinoe/ImGui.NET) is an excellent project by [mellinoe](https://github.com/mellinoe) that wraps [Dear ImGui](https://github.com/ocornut/imgui) by [ocornut](https://github.com/ocornut)

This project provides a custom game loop class (easily runnable via the game runner method [runImGuiGame](https://github.com/ChrisPritchard/fsharp-gamecore-imgui/blob/master/src/GameRunner.fs#L7)) that includes an integrated XNA/Monogame renderer for ImGui invocations.

It requires a getUI method, given the last game state model and expected to return a list of `uimodel -> uimodel` functions that can be used to invoke ImGui methods (e.g. `ImGui.Text "hello world"`), optionally derived from a custom UI model and optionally updating the same model, which is then passed as an additional argument to the standard fsharp-gamecore advance model function.

In this way, a UI can be generated based on the game's last model state, and can provide a state for the next game state's construction.

Available on Nuget at: <https://www.nuget.org/packages/fsharp-gamecore-imgui/0.0.2>

## Samples

Please see the the sample project for an example of use. There are a set of helpful wrapper methods in the Wrappers module, that provide functional invocation (and occasionally hide some nasty use of pointers) of common ImGui functions, e.g. Text, Rows, Inputs and Images.

For further examples of how ImGui can be used please see:

- The ImGui.NET sample XNA project's [sample app](https://github.com/mellinoe/ImGui.NET/blob/master/src/ImGui.NET.SampleProgram.XNA/SampleGame.cs#L75)
- The Dear ImGui's [demo source code](https://github.com/ocornut/imgui/blob/master/imgui_demo.cpp) (generally trivial to translate the C to the equivalent F#, thanks to mellinoe's excellent wrapping library)

## Use case

Dear ImGui is predominantly used to construct tooling, IDEs and similar - the base library does not provide the supreme level of customisation required for a fully bespoke game. 

However, a good use case (which is what I use it for) is to create a mock UI while developing, to be swapped out with a bespoke UI at a later date. Dear ImGui/ImGui.NET is good for rapid prototyping in this fashin.

The UI design philosophy that Dear ImGui implements is an excellent fit for both Monogame/XNA/Gameloop centric games, and for F# in my opinion.

## License

Provided under **MIT**. The default font for ImGui (Proggy, also under MIT) has been overridden by personal preference with Karla regular, one of the fonts shipped with Dear ImGui itself. Karla is under the **SIL Open Font License**.
using Silk.NET.Input;
using Silk.NET.Input.Extensions;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Runtime.InteropServices;
using Terrain.Controls;
using Terrain.Debug;
using Terrain.Generators;
using Shader = Terrain.Controls.Shader;

namespace Terrain
{
    public class Game : IDisposable
    {
        GL _GL;
        readonly IWindow _Window;
        IInputContext _Input;
        GUI gui;

        Matrix4X4<float> projection;

        Camera camera;
        Terrain3D terrain;
        Shader shader;
        readonly List<IDisposable> disposables = new();
        public Game(string title, int width, int height, int FPS)
        {
            var options = WindowOptions.Default;
            options.UpdatesPerSecond = FPS;
            options.FramesPerSecond = FPS;
            options.Size = new(width, height);
            options.Title = title;
            options.ShouldSwapAutomatically = false;

            _Window = Window.Create(options);
            _Window.Load += OnLoad;
            _Window.Update += OnUpdate;
            _Window.Render += OnRender;
            _Window.Closing += OnClose;
            _Window.Resize += OnResize;
        }
        private void OnLoad()
        {
            _GL = _Window.CreateOpenGL();
            _Input = _Window.CreateInput();
            _Input.Keyboards[0].KeyDown += OnKeyDown;
            gui = new(_GL, _Window, _Input);

            camera = new Camera(_Input, _Window.Size, float.Pi/4, 0.01f, 1000);
            projection = camera.Projection;
            _Input.Mice[0].Scroll += (_, scroll) => camera.ScrollInput(scroll);
            camera.ProjectionChanged += (_, matrix) => projection = matrix;

            _Window.Resize += camera.UpdateSize;

            _GL.Enable(EnableCap.DepthTest);

            shader = new(_GL, @"Shaders\LandShader.vert", @"Shaders\LandShader.frag");
            disposables.Add(_GL);
            disposables.Add(_Input);
            disposables.Add(shader);

            PerlinTexture perlinTexture = new(_GL, new Random().Next(), 64, 8);
            terrain = new(_GL, perlinTexture);
            disposables.Add(terrain);

            _GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
        }
        private void OnKeyDown(IKeyboard keyboard, Key key, int keyNum)
        {
            if (key == Key.Escape) _Window.Close();
            if (key == Key.P)
            {
                if (_Input.Mice[0].Cursor.CursorMode == CursorMode.Disabled)
                {
                    _Input.Mice[0].Cursor.CursorMode = CursorMode.Normal;
                }
                else
                {
                    _Input.Mice[0].Cursor.CursorMode = CursorMode.Disabled;
                }
            }
        }
        private void OnUpdate(double elapsedSeconds)
        {
            KeyboardState keyboard = _Input.Keyboards[0].CaptureState();
            camera.Update(elapsedSeconds);

            SetMatrices();
        }
        private void OnRender(double elapsedSeconds)
        {
            _GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Code Here
            gui.Update(elapsedSeconds);

            shader.Use();
            terrain.Draw(shader);
            gui.Draw(ref terrain.perlinScalar);
            //End Code

            _Window.SwapBuffers();
        }
        private void SetMatrices()
        {
            shader.SetUniform(camera.View, "View");
            shader.SetUniform(projection, "Projection");
        }
        private void OnResize(Vector2D<int> newSize)
        {
            _Window.Size = newSize;

            _GL.Viewport(newSize);
        }
        public void Run()
        {
            _Window.Run();
        }
        private void OnClose()
        {
            Dispose();
        }
        #region Dispose
        bool disposed = false;
        ~Game()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                disposables.ForEach(item => item.Dispose());
            }
            disposed = true;
        }
        #endregion
    }
}

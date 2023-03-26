using Silk.NET.Core.Contexts;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Terrain
{
    public class Game : IDisposable
    {
        IWindow _Window;
        GL _GL;
        IInputContext _Input;
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

            _GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
        }
        private void OnKeyDown(IKeyboard keyboard, Key key, int keyNum)
        {
            if (key == Key.Escape) _Window.Close();
        }
        private void OnUpdate(double elapsedSeconds)
        {

        }
        private void OnRender(double elapsedSeconds)
        {
            _GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code Here



            //End Code

            _Window.SwapBuffers();
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
        ~Game()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {

        }
        #endregion
    }
}

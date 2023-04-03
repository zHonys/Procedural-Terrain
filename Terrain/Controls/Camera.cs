using Silk.NET.Input;
using Silk.NET.Input.Extensions;
using Silk.NET.Maths;

namespace Terrain.Controls
{
    public class Camera : IDisposable
    {
        public Matrix4X4<float> View { get => _view; }
        private Matrix4X4<float> _view;
        public Matrix4X4<float> Projection { get => _projection; }
        private Matrix4X4<float> _projection;

        Vector3D<float> _position = Vector3D<float>.Zero;
        Vector3D<float> _Target =   -Vector3D<float>.UnitZ;
        Vector3D<float> _UpVector = Vector3D<float>.UnitY;

        float _speed = 5;

        Vector2D<float> _windowSize;
        float _FOV;
        float _nearPlane;
        float _farPlane;

        public Camera(Vector2D<int> windowSize, float FOV, float nearPlane, float farPlane)
        {
            _windowSize = (Vector2D<float>)windowSize;
            _FOV = FOV;
            _nearPlane = nearPlane;
            _farPlane = farPlane;

            UpdateView();
            UpdateProjection();
        }
        public void Update(double elapsedSeconds, KeyboardState keyboard)
        {
            MovementHandler((float)elapsedSeconds, keyboard.GetPressedKeys().ToArray());
        }
        private void MovementHandler(float elapsedSeconds, Key[] keys)
        {
            Vector3D<float> positionOffSet = Vector3D<float>.Zero;

            if (keys.Contains(Key.A)) positionOffSet += new Vector3D<float>(-1, 0, 0);
            if (keys.Contains(Key.D)) positionOffSet += new Vector3D<float>(1, 0, 0);
            if (keys.Contains(Key.W)) positionOffSet += new Vector3D<float>(0, 0, -1);
            if (keys.Contains(Key.S)) positionOffSet += new Vector3D<float>(0, 0, 1);
            if (positionOffSet == Vector3D<float>.Zero) return;
            _position += Vector3D.Normalize(positionOffSet) * _speed * elapsedSeconds;
            UpdateView();
        }
        #region View
        public void UpdateView()
        {
            _view =
            Matrix4X4.CreateLookAt(
                cameraPosition: _position,
                cameraTarget:   _Target + _position,
                cameraUpVector: _UpVector);
        }
        #endregion
        #region Projection
        public void UpdateSize(Vector2D<int> windowSize)
        {
            _windowSize = (Vector2D<float>)windowSize;
            UpdateProjection();
        }
        public void ScrollInput(ScrollWheel scroll)
        {
            float radians = MathF.PI / 180;
            float offSet = radians * scroll.Y;

            _FOV += offSet;

            if (_FOV >= radians * 90) _FOV = radians * 90;
            if (_FOV <= radians * 30) _FOV = radians * 30;

            UpdateProjection();
        }
        public event EventHandler<Matrix4X4<float>> ProjectionChanged;
        private void UpdateProjection()
        {
            _projection =
            Matrix4X4.CreatePerspectiveFieldOfView(
                fieldOfView:       _FOV,
                aspectRatio:       _windowSize.X/_windowSize.Y,
                nearPlaneDistance: _nearPlane,
                farPlaneDistance:  _farPlane);
            ProjectionChanged?.Invoke(this, _projection);
        }
        #endregion
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

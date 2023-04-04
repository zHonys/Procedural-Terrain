using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;
using Silk.NET.Maths;
using System.Formats.Tar;

namespace Terrain.Controls
{
    public class Camera : IDisposable
    {
        public Matrix4X4<float> View { get => _view; }
        private Matrix4X4<float> _view;
        public Matrix4X4<float> Projection { get => _projection; }
        private Matrix4X4<float> _projection;

        Vector3D<float> _position =  Vector3D<float>.Zero;
        Vector3D<float> _Target   = -Vector3D<float>.UnitZ;
        Vector3D<float> _worldUp  =  Vector3D<float>.UnitY;
        Vector3D<float> _cameraX  =  Vector3D<float>.UnitX;

        readonly float _speed = 50;

        Vector2D<float> _windowSize;
        float _FOV;
        readonly float _nearPlane;
        readonly float _farPlane;

        readonly IInputContext _input;
        readonly float sensibility = 1;

        float Pitch =  0;  // Y Axis
        float Yaw   =  0;  // Z Axis
        public Camera(IInputContext input, Vector2D<int> windowSize, float FOV, float nearPlane, float farPlane)
        {
            _input = input;

            _windowSize = (Vector2D<float>)windowSize;
            _FOV = FOV;
            _nearPlane = nearPlane;
            _farPlane = farPlane;

            UpdateView();
            UpdateProjection();
        }
        public void Update(double elapsedSeconds)
        {
            Inputhandler((float)elapsedSeconds);
        }
        #region View
        private void Inputhandler(float elapsedSeconds)
        {
            MouseHandler(elapsedSeconds, _input.Mice[0]);
            MovementHandler(elapsedSeconds, _input.Keyboards[0].CaptureState().GetPressedKeys().ToArray());
        }
        private Vector2D<float> lastPos = Vector2D<float>.Zero;
        private void MouseHandler(float elapsedSeconds, IMouse mouse)
        {
            if (mouse.Cursor.CursorMode != CursorMode.Disabled) return;
            Vector2D<float> currentPos = mouse.Position.ToGeneric();
            if (currentPos == lastPos) return;
            float changeRate = sensibility * elapsedSeconds * Scalar<float>.RadiansPerDegree;
            Vector2D<float> deltaPos   = (currentPos - lastPos) * changeRate;

            Yaw   +=  deltaPos.X;
            Pitch += -deltaPos.Y;

            if (Scalar.Abs(Pitch) >= Scalar.DegreesToRadians(85f)) Pitch = Scalar.DegreesToRadians(85f) * Scalar.Sign(Pitch);

            if (Scalar.Abs(Yaw - Scalar<float>.Pi) > Scalar<float>.Pi) Yaw += Scalar<float>.Tau * -Scalar.Sign(Yaw);

            _Target = new Vector3D<float>(
                x: Scalar.Cos(Yaw) * Scalar.Cos(Pitch),
                y: Scalar.Sin(Pitch),
                z: Scalar.Sin(Yaw) * Scalar.Cos(Pitch));

            _Target =  Vector3D.Normalize(_Target);

            _cameraX = Vector3D.Cross(_Target, _worldUp);

            Console.WriteLine($"Radians: {Yaw}  -   Degrees: {Scalar.RadiansToDegrees(Yaw)}");

            lastPos = currentPos;
            UpdateView();
        }
        private void MovementHandler(float elapsedSeconds, Key[] keys)
        {
            Vector3D<float> positionOffSet = Vector3D<float>.Zero;

            if (keys.Contains(Key.A)) positionOffSet += -_cameraX;
            if (keys.Contains(Key.D)) positionOffSet +=  _cameraX;
            if (keys.Contains(Key.W)) positionOffSet +=  _Target;
            if (keys.Contains(Key.S)) positionOffSet += -_Target;

            if (positionOffSet == Vector3D<float>.Zero) return;
            //Console.WriteLine(positionOffSet);
            _position += Vector3D.Normalize(positionOffSet) * _speed * elapsedSeconds;
            UpdateView();
        }
        public void UpdateView()
        {
            _view =
            Matrix4X4.CreateLookAt(
                cameraPosition: _position,
                cameraTarget:   _Target + _position,
                cameraUpVector: _worldUp);
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

            _FOV -= offSet;

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

using Silk.NET.OpenGL;

public class Texture2D : IDisposable
{
    GL _GL;
    public uint Handle;
    public string UniformName;
    public Texture2D(GL gl, uint handle, string uniformName)
    {
        _GL = gl;
        Handle = handle;
        UniformName = uniformName;
    }
    public void setTexture(Terrain.Controls.Shader shader, uint textureUnit = 33984)
    {
        _GL.ActiveTexture((GLEnum)textureUnit);
        _GL.BindTexture(TextureTarget.Texture2D, Handle);
        shader.SetUniform(textureUnit - 33984, UniformName);
    }
    static public void setTextures(IEnumerable<Texture2D> textures, Terrain.Controls.Shader shader)
    {
        foreach (var item in textures.Select((texture, index) => { return (index, texture); }))
        {
            item.texture.setTexture(shader, (uint)(item.index + 33984));
        }
    }

    #region Dispose
    public void Dispose()
    {
        if (Handle == 0) return;

        _GL.DeleteTexture(Handle);
        Handle = 0;
        _GL = null;
    }
    #endregion
}
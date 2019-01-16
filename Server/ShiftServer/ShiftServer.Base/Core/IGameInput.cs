using System.Numerics;


namespace ShiftServer.Base.Core
{
    public interface IGameInput
    {
        MSPlayerEvent evt { get; set; }
        Vector3 vector3 { get; set; }
        float sensivity { get; set; }
    }

}

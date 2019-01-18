using System.Numerics;


namespace ShiftServer.Base.Core
{
    public interface IGameInput
    {
        MSPlayerEvent EventType { get; set; }
        Vector3 Vector { get; set; }
        int SequenceID { get; set; }
    }

}

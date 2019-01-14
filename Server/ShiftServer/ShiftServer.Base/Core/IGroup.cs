using ShiftServer.Base.Auth;
using Telepathy;

namespace ShiftServer.Base.Core
{
    public interface IGroup
    {
        string Id { get; set; }
        SafeDictionary<int, ShiftClient> Clients { get; set; }
        int MaxPlayer { get; set; }
        void OnInvite(ShiftClient client, IGameObject gameObject);
        void OnAccept(ShiftClient client, IGameObject gameObject);
        void OnKick(ShiftClient client, IGameObject gameObject);
        void AddPlayer(ShiftClient client);
        void RemovePlayer(ShiftClient client);
        void Leave(ShiftClient client);
    }
}

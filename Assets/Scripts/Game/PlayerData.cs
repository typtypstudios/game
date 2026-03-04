using Unity.Netcode;
using Unity.Collections;
using System.Text;

[System.Serializable]
public struct PlayerData : INetworkSerializable
{
    /// <summary>
    /// ID del cliente
    /// </summary>
    public ulong ClientID;

    /// <summary>
    /// Nombre del jugador
    /// </summary>
    public FixedString32Bytes PlayerName;

    /// <summary>
    /// Cartas del mazo (IDs)
    /// </summary>
    public int[] Deck;

    const int MAX_DECK_SIZE = 64;

    public PlayerData(ulong clientID, FixedString32Bytes playerName, int[] deck)
    {
        ClientID = clientID;
        PlayerName = playerName;
        Deck = deck;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref PlayerName);

        int length = Deck != null ? Deck.Length : 0;

        serializer.SerializeValue(ref length);

        if (length > MAX_DECK_SIZE)
            throw new System.Exception($"Deck size too large: {length}");

        if (serializer.IsReader)
            Deck = new int[length];

        for (int i = 0; i < length; i++)
        {
            serializer.SerializeValue(ref Deck[i]);
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("PlayerData { ");
        sb.Append("ClientID: ").Append(ClientID);
        sb.Append(", Name: ").Append(PlayerName);
        sb.Append(", Deck: [");

        if (Deck != null)
        {
            for (int i = 0; i < Deck.Length; i++)
            {
                sb.Append(Deck[i]);
                if (i < Deck.Length - 1)
                    sb.Append(", ");
            }
        }

        sb.Append("] }");

        return sb.ToString();
    }
}
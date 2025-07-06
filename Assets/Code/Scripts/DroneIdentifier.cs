[System.Serializable]
public struct DroneIdentifier
{
    public string sceneName;
    public int droneID;

    public override bool Equals(object obj)
    {
        return obj is DroneIdentifier other &&
               sceneName == other.sceneName &&
               droneID == other.droneID;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (sceneName != null ? sceneName.GetHashCode() : 0);
            hash = hash * 23 + droneID.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(DroneIdentifier a, DroneIdentifier b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(DroneIdentifier a, DroneIdentifier b)
    {
        return !a.Equals(b);
    }
}